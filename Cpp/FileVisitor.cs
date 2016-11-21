//------------------------------------------------------------------------------
// <copyright file="FileVisitor.cs" company="Zebedee Mason">
//     Copyright (c) 2016 Zebedee Mason.
//
//      The author's copyright is expressed through the following notice, thus
//      giving effective rights to copy and use this software to anyone, as shown
//      in the license text.
//
//     NOTICE:
//      This software is released under the terms of the GNU Lesser General Public
//      license; a copy of the text has been released with this package (see file
//      LICENSE, where the license text also appears), and can be found on the
//      GNU web site, at the following address:
//
//           http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
//
//      Please refer to the license text for any license information. This notice
//      has to be considered part of the license, and should be kept on every copy
//      integral or modified, of the source files. The removal of the reference to
//      the license will be considered an infringement of the license itself.
// </copyright>
//------------------------------------------------------------------------------

namespace DeepEnds.Cpp.Clang
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClangSharp;

    internal sealed class FileVisitor
    {
        private DeepEnds.Core.Parser parser;

        private Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves;

        private Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> links;

        private HashSet<string> visited;

        private string filePath;

        private readonly TextWriter logger;

        private string namespaces;

        private DeepEnds.Core.Dependent.Dependency current;

        public FileVisitor(DeepEnds.Core.Parser parser, Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves, Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> links, string filePath, TextWriter logger)
        {
            this.parser = parser;
            this.leaves = leaves;
            this.links = links;
            this.visited = new HashSet<string>();
            this.filePath = filePath;
            this.logger = logger;
            this.namespaces = string.Empty;
            this.current = null;
        }

        public CXChildVisitResult VisitClass(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            CXCursorKind curKind = clang.getCursorKind(cursor);

            var displayName = clang.getCursorDisplayName(cursor).ToString();
            var spelling = clang.getCursorSpelling(cursor).ToString();

            if (curKind == CXCursorKind.CXCursor_CXXMethod || curKind == CXCursorKind.CXCursor_Constructor || curKind == CXCursorKind.CXCursor_Destructor)
            {
                if (this.current != null)
                {
                    var lines = FileVisitor.NumLines(cursor);
                    this.current.LOC += lines;
                }
            }
            else if (curKind == CXCursorKind.CXCursor_TypeRef || curKind == CXCursorKind.CXCursor_CXXBaseSpecifier || curKind == CXCursorKind.CXCursor_TemplateRef)
            {
                displayName = displayName.Replace("class ", string.Empty).Replace("enum ", string.Empty).Replace("struct ", string.Empty).Replace("union ", string.Empty);
                this.References(displayName);
            }
            else if (curKind == CXCursorKind.CXCursor_ClassTemplate)
            {
                spelling = spelling.Replace("class ", string.Empty).Replace("enum ", string.Empty).Replace("struct ", string.Empty).Replace("union ", string.Empty);
                this.References(spelling);
            }
            else if (curKind == CXCursorKind.CXCursor_EnumDecl)
            {
                if (this.current != null)
                {
                    var fullName = this.current.Path(".") + "." + spelling;
                    this.leaves[fullName] = this.current;
                }
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }

        public CXChildVisitResult FindCurrent(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            CXCursorKind curKind = clang.getCursorKind(cursor);

            if (curKind == CXCursorKind.CXCursor_TypeRef)
            {
                var displayName = clang.getCursorDisplayName(cursor).ToString();
                displayName = displayName.Replace("class ", string.Empty).Replace("enum ", string.Empty).Replace("struct ", string.Empty).Replace("union ", string.Empty).Replace("::", ".");
                if (this.leaves.ContainsKey(displayName))
                {
                    this.current = this.leaves[displayName];
                }
                else
                {
                    this.current = null;
                    this.logger.Write("Couldn't find current class, etc. : ");
                    this.logger.WriteLine(displayName);
                }

                return CXChildVisitResult.CXChildVisit_Break;
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        public CXChildVisitResult VisitFile(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            var location = clang.getCursorLocation(cursor);
            if (clang.Location_isInSystemHeader(location) != 0)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            if (true)
            {
                CXFile file;
                uint i, j, k;
                clang.getFileLocation(location, out file, out i, out j, out k);
                var fileName = clang.getFileName(file).ToString().Replace('/', System.IO.Path.PathSeparator).Replace('\\', System.IO.Path.PathSeparator);

                if (this.visited.Contains(fileName))
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                this.visited.Add(fileName);
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);

            var displayName2 = clang.getCursorDisplayName(cursor).ToString();
            var name2 = clang.getCursorSpelling(cursor).ToString();

            if (curKind == CXCursorKind.CXCursor_Namespace)
            {
                this.namespaces += "." + clang.getCursorSpelling(cursor).ToString();
                clang.visitChildren(cursor, this.VisitFile, new CXClientData(IntPtr.Zero));
                this.namespaces = this.namespaces.Substring(0, this.namespaces.LastIndexOf("."));
            }
            else if (curKind == CXCursorKind.CXCursor_ClassDecl || curKind == CXCursorKind.CXCursor_StructDecl || curKind == CXCursorKind.CXCursor_UnionDecl)
            {
                var name = clang.getCursorSpelling(cursor).ToString();

                var filter = this.namespaces;
                if (filter.Length > 0)
                {
                    filter = filter.Substring(1);
                }

                var fullName = filter + "." + name;
                if (!this.leaves.ContainsKey(fullName))
                {
                    var branch = this.parser.Dependencies.GetPath(filter, ".");
                    var fileName = System.IO.Path.GetFileName(this.filePath);
                    var leaf = this.parser.Create(name, fullName, branch);
                    this.parser.Sources.Create(leaf, new Core.SourceProvider(leaf, this.filePath));
                    this.leaves[fullName] = leaf;

                    this.current = leaf;
                    this.links[this.current] = new HashSet<string>();
                    clang.visitChildren(cursor, this.VisitClass, new CXClientData(IntPtr.Zero));
                    this.current = null;
                }
            }
            else if (curKind == CXCursorKind.CXCursor_CXXMethod || curKind == CXCursorKind.CXCursor_Constructor || curKind == CXCursorKind.CXCursor_Destructor)
            {
                clang.visitChildren(cursor, this.FindCurrent, new CXClientData(IntPtr.Zero));
                if (this.current != null)
                {
                    clang.visitChildren(cursor, this.VisitClass, new CXClientData(IntPtr.Zero));
                }

                this.current = null;
            }
            else if (curKind != CXCursorKind.CXCursor_UsingDirective && curKind != CXCursorKind.CXCursor_VarDecl)
            {
                this.logger.Write("Skipping ");
                this.logger.WriteLine(curKind.ToString());
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private void References(CXCursor cursor)
        {
            var displayName = clang.getCursorDisplayName(cursor).ToString();
            displayName = displayName.Substring(1 + displayName.IndexOf('('));
            displayName = displayName.Substring(0, displayName.LastIndexOf(')'));
            this.References(displayName);
        }

        private void References(string dependencies)
        {
            if (this.current == null)
            {
                return;
            }

            var dependency = this.links[this.current];
            dependencies = dependencies.Replace("const ", string.Empty).Replace("&", string.Empty).Replace("*", string.Empty).Replace("::", ".").Replace("<", ",").Replace(">", ",").Trim();
            if (dependencies.Length > 0)
            {
                var bits = dependencies.Split(',');
                foreach (var bit in bits)
                {
                    var reference = bit.Trim();
                    if (reference == string.Empty)
                    {
                        continue;
                    }

                    dependency.Add(reference);
                }
            }
        }

        private static int NumLines(CXCursor cursor)
        {
            CXSourceRange extent = clang.getCursorExtent(cursor);
            CXSourceLocation startLocation = clang.getRangeStart(extent);
            CXSourceLocation endLocation = clang.getRangeEnd(extent);

            uint startLine = 0, startColumn = 0;
            uint endLine = 0, endColumn = 0, offset;

            CXFile file;
            clang.getSpellingLocation(startLocation, out file, out startLine, out startColumn, out offset);
            clang.getSpellingLocation(endLocation, out file, out endLine, out endColumn, out offset);

            return 1 + (int)endLine - (int)startLine;
        }
    }
}
