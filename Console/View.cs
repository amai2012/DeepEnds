//------------------------------------------------------------------------------
// <copyright file="View.cs" company="Zebedee Mason">
//     Copyright 2016 Zebedee Mason.
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

namespace DeepEnds.Console
{
    using System;
    using System.Collections.Generic;
    using DeepEnds.Core;
    using DeepEnds.Core.Linked;

    public class View
    {
        private Dependencies dependencies;
        private Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> sets;
        private Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves;
        private Sources sources;
        private string sep;

        public System.Text.StringBuilder Messages { get; set; }

        public View()
        {
            this.dependencies = new Dependencies();
            this.sets = new Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>>();
            this.leaves = new Dictionary<string, DeepEnds.Core.Dependent.Dependency>();
            this.sources = new Sources();
            this.sep = ".";
            this.Messages = new System.Text.StringBuilder();
        }

        private void Files(string fileName, string direc, List<KeyValuePair<string, string>> fileNames, List<string> extensions, System.Text.StringBuilder messages)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            if (ext == ".vcxproj" || ext == ".csproj" || ext == ".vbproj" || ext == ".exe" || ext == ".dll")
            {
                messages.AppendFormat("  Appended for reading {0}\n", fileName);
                if (!extensions.Contains(ext))
                {
                    extensions.Add(ext);
                }

                fileNames.Add(new KeyValuePair<string, string>(fileName, direc));
            }
            else if (ext == ".sln")
            {
                messages.AppendFormat(" Reading projects from {0}\n", fileName);
                direc = System.IO.Path.GetDirectoryName(fileName);
                foreach (var name in DeepEnds.Core.Utilities.ReadVisualStudioSolution(fileName))
                {
                    this.Files(name, direc, fileNames, extensions, messages);
                }
            }
        }

        public bool Read(string[] lines)
        {
            this.Messages = new System.Text.StringBuilder("Reading\n");
            var fileNames = new List<KeyValuePair<string, string>>();
            var extensions = new List<string>();
            foreach (var fileName in lines)
            {
                this.Files(fileName, string.Empty, fileNames, extensions, this.Messages);
            }

            if (extensions.Count != 1 && extensions.Contains(".vcxproj"))
            {
                this.Messages.Append("Cannot mix vcxproj and others, have :");
                foreach (var ext in extensions)
                {
                    this.Messages.AppendFormat(" {0}", ext);
                }

                return false;
            }

            this.sources = new Sources();
            this.dependencies = new Dependencies();
            var parser = new Parser(this.dependencies, this.sources);

            this.sep = ".";
            if (extensions.Contains(".vcxproj"))
            {
                this.sep = "\\";
            }

            var csharp = new CSharp.Parse(parser);
            var vbasic = new VBasic.Parse(parser);
            var cpp = new Cpp.ParseVS(parser);
            var dotnet = new Decompile.Parse(parser);

            var dlls = new List<string>();
            foreach (var pair in fileNames)
            {
                var ext = System.IO.Path.GetExtension(pair.Key);

                if (ext == ".dll" || ext == ".exe")
                {
                    dlls.Add(pair.Key);
                    dotnet.Read(pair.Key, this.Messages);
                }
                else if (ext == ".csproj")
                {
                    csharp.ReadProject(pair.Key, this.Messages);
                }
                else if (ext == ".vbproj")
                {
                    vbasic.ReadProject(pair.Key, this.Messages);
                }
                else if (ext == ".vcxproj")
                {
                    cpp.Read(pair.Key, pair.Value, this.Messages);
                }
            }

            csharp.Finalise(dlls);
            vbasic.Finalise(dlls);
            parser.Finalise(this.sep);

            return true;
        }

        public void Write(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            if (ext == ".html" || ext == ".htm")
            {
                var report = new DeepEnds.Core.Report(fileName, this.sep);
                report.Write(this.dependencies);
                return;
            }

            if (ext == ".dgml")
            {
                var assemble = DeepEnds.DGML.Assemble.Factory(this.dependencies.Root, this.dependencies.Assembled.Linkings, this.sources, true);
                assemble.Save(fileName);
                return;
            }

            throw new Exception(string.Format("Extension({0}) not recognised", ext));
        }
    }
}
