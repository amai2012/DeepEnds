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
    using System.Linq;
    using System.Xml;

    public class ParseVS
    {
        private System.IO.TextWriter logger;
        private Dictionary<string, List<string>> projects;

        private IParse parse;

        public ParseVS(DeepEnds.Core.Parser parser, Dictionary<string, string> options, System.IO.TextWriter logger)
        {
            this.logger = logger;
            this.projects = new Dictionary<string, List<string>>();
            if (options["parser"] == "libclang")
            {
                this.parse = new Clang.ParseClang(parser, logger);
            }
            else
            {
                this.parse = new Include.Parse(parser, logger);
            }
        }

        private void ReadProject(string project)
        {
            if (!File.Exists(project + ".filters"))
            {
                this.logger.Write("! Cannot find associated filter file of");
                this.logger.WriteLine(project);
                return;
            }

            this.logger.Write(" Parsing");
            this.logger.WriteLine(project);
            this.projects[project] = new List<string>();
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

                        this.logger.Write("  Appended ");
                        this.logger.WriteLine(DeepEnds.Core.Utilities.Combine(direc, filename));
                        var fullName = DeepEnds.Core.Utilities.Combine(direc, filename);
                        this.parse.AddFile(fullName, filter);
                        this.projects[project].Add(fullName);
                    }
                }
            }
        }

        private void SelectNodes(System.Xml.XmlElement root, string name, List<System.Xml.XmlElement> list)
        {
            foreach (var node in root.ChildNodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                if (element.Name == name)
                {
                    list.Add(element);
                }
                else
                {
                    this.SelectNodes(element, name, list);
                }
            }
        }

        private List<string> Includes(string project, string slndir)
        {
            var includes = new HashSet<string>();
            var direc = System.IO.Path.GetDirectoryName(project);

            var doc = new System.Xml.XmlDocument();
            doc.Load(project);
            var root = doc.DocumentElement;

            var nodes = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "AdditionalIncludeDirectories", nodes);
            this.SelectNodes(root, "NMakeIncludeSearchPath", nodes);
            this.SelectNodes(root, "IncludePath", nodes);

            foreach (var node in nodes)
            {
                var line = node.InnerText;
                foreach (var inc in line.Split(';'))
                {
                    if (inc == "%(AdditionalIncludeDirectories)" || inc.Length == 0)
                    {
                        continue;
                    }

                    var include = inc;
                    include = include.Replace("$(ProjectDir)", direc + "\\");
                    if (slndir.Length > 0)
                    {
                        include = include.Replace("$(SolutionDir)", slndir + "\\");
                    }

                    if (include.Length > 1 && include.Substring(0, 2) == "..")
                    {
                        include = System.IO.Path.Combine(direc, include);
                    }

                    include = System.IO.Path.GetFullPath(include);
                    includes.Add(include);
                }
            }

            return includes.ToList();
        }

        public void Read(string project, string solutionDirec)
        {
            this.ReadProject(project);
            var includes = this.Includes(project, solutionDirec);
            foreach (var fullName in this.projects[project])
            {
                this.parse.ReadFile(fullName, includes);
            }
        }

        public void Finalise()
        {
            this.parse.Finalise();
        }
    }
}
