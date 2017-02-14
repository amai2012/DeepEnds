//------------------------------------------------------------------------------
// <copyright file="Parse.cs" company="Zebedee Mason">
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

namespace DeepEnds.VBasic
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.VisualBasic;
    using System.Collections.Generic;

    public class Parse
    {
        private DeepEnds.Core.Parser parser;

        private Dictionary<string, string> vbfiles;

        public Parse(DeepEnds.Core.Parser parser)
        {
            this.parser = parser;
            this.vbfiles = new Dictionary<string, string>();
        }

        public void ReadProject(string project, System.IO.TextWriter logger)
        {
            DeepEnds.Core.Utilities.ReadProject(project, this.vbfiles, logger);
        }

        public void Finalise(List<string> dlls)
        {
            var trees = new List<SyntaxTree>();
            foreach (var vbfile in this.vbfiles.Keys)
            {
                var contents = DeepEnds.Core.Utilities.ReadFile(vbfile);
                var tree = VisualBasicSyntaxTree.ParseText(contents, null, vbfile);
                trees.Add(tree);
                ParseTree.Process(this.parser, this.parser.Sources, tree, vbfile);
            }

            var libs = new PortableExecutableReference[dlls.Count + 1];
            for (int i = 0; i < dlls.Count; ++i)
            {
                libs[i] = MetadataReference.CreateFromFile(dlls[i]);
            }

            libs[dlls.Count] = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var compilation = VisualBasicCompilation.Create("Compilation", syntaxTrees: trees.ToArray(), references: libs);
            foreach (var tree in trees)
            {
                var model = compilation.GetSemanticModel(tree);
                ParseTree.Link(this.parser, tree, model);
            }
        }
    }
}
