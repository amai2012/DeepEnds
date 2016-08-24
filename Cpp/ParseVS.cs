//------------------------------------------------------------------------------
// <copyright file="ParseVS.cs" company="Zebedee Mason">
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

namespace DeepEnds.Cpp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    public class ParseVS : Parse
    {
        private Dictionary<string, List<DeepEnds.Core.Dependent.Dependency>> projects;

        public ParseVS(DeepEnds.Core.Parser parser)
            : base(parser)
        {
            this.projects = new Dictionary<string, List<DeepEnds.Core.Dependent.Dependency>>();
        }

        private void ReadProject(string project, System.Text.StringBuilder messages)
        {
            if (!File.Exists(project + ".filters"))
            {
                messages.AppendFormat("! {0} - cannot find associated filter file\n", project);
                return;
            }

            messages.AppendFormat(" {0} - parsing\n", project);
            this.projects[project] = new List<DeepEnds.Core.Dependent.Dependency>();
            var direc = System.IO.Path.GetDirectoryName(project);
            foreach (var type in new string[] { "ClInclude", "ClCompile" })
            {
                var stream = new FileStream(project + ".filters", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var reader = XmlReader.Create(stream))
                {
                    while (reader.ReadToFollowing(type))
                    {
                        var filename = reader.GetAttribute("Include");
                        var filter = string.Empty;
                        if (reader.ReadToDescendant("Filter"))
                        {
                            filter = reader.ReadElementContentAsString();
                        }

                        messages.AppendFormat("  {0} - appended\n", DeepEnds.Core.Utilities.Combine(direc, filename));
                        this.AddFile(project, DeepEnds.Core.Utilities.Combine(direc, filename), filter, this.projects[project], messages);
                    }
                }
            }
        }

        private List<string> Includes(string project, string slndir)
        {
            var includes = new List<string>();
            var direc = System.IO.Path.GetDirectoryName(project);
            {
                var stream = new FileStream(project, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var reader = XmlReader.Create(stream))
                {
                    while (reader.ReadToFollowing("AdditionalIncludeDirectories"))
                    {
                        foreach (var inc in reader.ReadElementContentAsString().Split(';'))
                        {
                            if (inc == "%(AdditionalIncludeDirectories)")
                            {
                                continue;
                            }

                            var include = inc;
                            include = include.Replace("$(ProjectDir)", direc + "\\");
                            include = include.Replace("$(SolutionDir)", slndir + "\\");
                            if (!includes.Contains(include))
                            {
                                includes.Add(include);
                            }
                        }
                    }
                }
            }

            return includes;
        }

        public void Read(string project, string solutionDirec, System.Text.StringBuilder messages)
        {
            this.ReadProject(project, messages);
            var includes = this.Includes(project, solutionDirec);
            foreach (var node in this.projects[project])
            {
                this.ReadFile(node, includes);
            }
        }
    }
}
