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

#if false
        private HashSet<string> visited;
#endif

        private readonly TextWriter logger;

        private string namespaces;

        private DeepEnds.Core.Dependent.Dependency current;

        private bool hasChildren;

        public FileVisitor(DeepEnds.Core.Parser parser, Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves, Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> links, TextWriter logger)
        {
            this.parser = parser;
            this.leaves = leaves;
            this.links = links;
#if false
            this.visited = new HashSet<string>();
#endif
            this.logger = logger;
            this.namespaces = string.Empty;
            this.current = null;
        }

        public CXChildVisitResult VisitClass(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            CXCursorKind curKind = clang.getCursorKind(cursor);

            if (curKind == CXCursorKind.CXCursor_CXXMethod || curKind == CXCursorKind.CXCursor_Constructor || curKind == CXCursorKind.CXCursor_Destructor)
            {
                if (this.current != null)
                {
                    var lines = FileVisitor.NumLines(cursor);
                    this.current.LOC += lines;
                }
            }
            else if (curKind == CXCursorKind.CXCursor_TypeRef || curKind == CXCursorKind.CXCursor_VarDecl || curKind == CXCursorKind.CXCursor_CXXBaseSpecifier || curKind == CXCursorKind.CXCursor_TemplateRef || curKind == CXCursorKind.CXCursor_DeclRefExpr || curKind == CXCursorKind.CXCursor_ClassTemplate)
            {
                this.References(FullName(cursor));
            }
            else if (curKind == CXCursorKind.CXCursor_EnumDecl)
            {
                if (this.current != null)
                {
                    this.leaves[FullName(cursor)] = this.current;
                }
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }

        private static string FullName(CXCursor cursor)
        {
            return clang.getCursorType(cursor).ToString().Replace("::", ".");
        }

        public CXChildVisitResult VisitFile(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            var location = clang.getCursorLocation(cursor);
            if (clang.Location_isInSystemHeader(location) != 0)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

#if false
            if (false)
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
#endif

            CXCursorKind curKind = clang.getCursorKind(cursor);

            if (curKind == CXCursorKind.CXCursor_Namespace)
            {
                this.namespaces += "." + clang.getCursorSpelling(cursor).ToString();
                clang.visitChildren(cursor, this.VisitFile, new CXClientData(IntPtr.Zero));
                this.namespaces = this.namespaces.Substring(0, this.namespaces.LastIndexOf("."));
            }
            else if (curKind == CXCursorKind.CXCursor_ClassDecl || curKind == CXCursorKind.CXCursor_StructDecl || curKind == CXCursorKind.CXCursor_UnionDecl)
            {
                var fullName = FullName(cursor);

                if (!this.leaves.ContainsKey(fullName))
                {
                    var filter = string.Empty;
                    var name = fullName;
                    var i = fullName.LastIndexOf(".");
                    if (i != -1)
                    {
                        name = fullName.Substring(i + 1);
                        filter = fullName.Substring(0, i);
                    }

                    var branch = this.parser.Dependencies.GetPath(filter, ".");
                    var leaf = this.parser.Create(name, fullName, branch);
                    this.SetSourceProvider(leaf, cursor);
                    this.leaves[fullName] = leaf;

                    this.current = leaf;
                    this.links[this.current] = new HashSet<string>();
                }
                else
                {
                    var leaf = this.leaves[fullName];
                    this.current = leaf;
                    this.hasChildren = false;
                    clang.visitChildren(cursor, this.VisitChildren, new CXClientData(IntPtr.Zero));
                    if (this.hasChildren)
                    {
                        this.SetSourceProvider(leaf, cursor);
                    }
                }

                clang.visitChildren(cursor, this.VisitClass, new CXClientData(IntPtr.Zero));
                this.current = null;
            }
            else if (curKind == CXCursorKind.CXCursor_CXXMethod || curKind == CXCursorKind.CXCursor_Constructor || curKind == CXCursorKind.CXCursor_Destructor)
            {
                this.current = null;

                var fullName = FileVisitor.GetClass(cursor);
                if (this.namespaces.Length > 0)
                {
                    fullName = this.namespaces.Substring(1) + "." + fullName;
                }

                if (this.leaves.ContainsKey(fullName))
                {
                    this.current = this.leaves[fullName];
                    clang.visitChildren(cursor, this.VisitClass, new CXClientData(IntPtr.Zero));
                    this.current = null;
                }
                else
                {
                    this.logger.Write("Cannot find class ");
                    this.logger.WriteLine(fullName);
                }
            }
            else if (curKind != CXCursorKind.CXCursor_UsingDirective && curKind != CXCursorKind.CXCursor_VarDecl)
            {
                this.logger.Write("Skipping ");
                this.logger.WriteLine(curKind.ToString());
            }

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        private void SetSourceProvider(Core.Dependent.Dependency leaf, CXCursor cursor)
        {
            CXSourceRange extent = clang.getCursorExtent(cursor);
            CXSourceLocation startLocation = clang.getRangeStart(extent);

            uint startLine = 0, startColumn = 0, offset;

            CXFile file;
            clang.getSpellingLocation(startLocation, out file, out startLine, out startColumn, out offset);

            this.parser.Sources.Create(leaf, new Core.SourceProvider(leaf, clang.getFileName(file).ToString()));
        }

        private CXChildVisitResult VisitChildren(CXCursor cursor, CXCursor parent, IntPtr client_data)
        {
            CXCursorKind curKind = clang.getCursorKind(cursor);
            if (curKind == CXCursorKind.CXCursor_CXXAccessSpecifier)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            this.hasChildren = true;
            return CXChildVisitResult.CXChildVisit_Break;
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
                    if (reference.Length == 0)
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

        private static string GetClass(CXCursor cursor)
        {
            CXSourceRange extent = clang.getCursorExtent(cursor);
            CXSourceLocation startLocation = clang.getRangeStart(extent);
            CXSourceLocation endLocation = clang.getRangeEnd(extent);

            uint startLine = 0, startColumn = 0, offset;

            CXFile file;
            clang.getSpellingLocation(startLocation, out file, out startLine, out startColumn, out offset);

            var fp = new System.IO.StreamReader(clang.getFileName(file).ToString());
            var line = string.Empty;
            for (var i = 0; i < startLine; ++i)
            {
                line = fp.ReadLine();
            }

            fp.Close();

            int index;
            line = line.Substring(0, line.LastIndexOf('('));
            line = line.Substring(0, line.LastIndexOf(':') - 1);
            index = line.LastIndexOf(' ');
            if (index != -1)
            {
                line = line.Substring(index + 1);
            }

            index = line.LastIndexOf('\t');
            if (index != -1)
            {
                line = line.Substring(index + 1);
            }

            line = line.Trim();

            return line.Replace("::", ".");
        }
    }
}
