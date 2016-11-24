//------------------------------------------------------------------------------
// <copyright file="ParseClang.cs" company="Zebedee Mason">
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
    using System.Linq;
    using DeepEnds.Core.Dependent;
    using ClangSharp;

    public class ParseClang : IParse
    {
        private DeepEnds.Core.Parser parser;

        private TextWriter logger;

        private CXIndex createIndex;

        private Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves;

        private Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> links;

        public ParseClang(DeepEnds.Core.Parser parser, TextWriter logger)
        {
            this.parser = parser;
            this.logger = logger;
            this.createIndex = clang.createIndex(0, 0);
            this.leaves = new Dictionary<string, DeepEnds.Core.Dependent.Dependency>();
            this.links = new Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>>();
        }

        public void AddFile(string filePath, string filter)
        {
        }

        public void ReadFile(string filePath, List<string> includes)
        {
            this.logger.Write("  Parsing ");
            this.logger.WriteLine(filePath);

            string[] arr = { "-x", "c++" };
            arr = arr.Concat(includes.Select(x => "-I" + x)).ToArray();

            CXTranslationUnit translationUnit;
            CXUnsavedFile unsavedFile;
            var translationUnitError = clang.parseTranslationUnit2(this.createIndex, filePath, arr, arr.Length, out unsavedFile, 0, 0, out translationUnit);

            if (translationUnitError != CXErrorCode.CXError_Success)
            {
                this.logger.WriteLine("Error: " + translationUnitError);
                var numDiagnostics = clang.getNumDiagnostics(translationUnit);

                for (uint i = 0; i < numDiagnostics; ++i)
                {
                    var diagnostic = clang.getDiagnostic(translationUnit, i);
                    this.logger.WriteLine(clang.getDiagnosticSpelling(diagnostic).ToString());
                    clang.disposeDiagnostic(diagnostic);
                }
            }

            var structVisitor = new FileVisitor(this.parser, this.leaves, this.links, this.logger);
            clang.visitChildren(clang.getTranslationUnitCursor(translationUnit), structVisitor.VisitFile, new CXClientData(IntPtr.Zero));

            clang.disposeTranslationUnit(translationUnit);
        }

        public void Finalise()
        {
            HashSet<string> missing = new HashSet<string>();
            foreach (var key in this.links.Keys)
            {
                foreach (var link in this.links[key])
                {
                    if (!this.leaves.ContainsKey(link))
                    {
                        if (!missing.Contains(link))
                        {
                            missing.Add(link);
                            this.logger.Write("  Cannot link ");
                            this.logger.WriteLine(link);
                        }

                        continue;
                    }

                    key.AddDependency(link, this.leaves[link]);
                }
            }

            clang.disposeIndex(this.createIndex);
        }
    }
}
