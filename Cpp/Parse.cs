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

namespace DeepEnds.Cpp
{
    using System.Collections.Generic;

    public class Parse : IParse
    {
        private DeepEnds.Core.Parser parser;

        private System.IO.TextWriter logger;

        private Dictionary<string, DeepEnds.Core.Dependent.Dependency> nodes;

        public Parse(DeepEnds.Core.Parser parser, System.IO.TextWriter logger)
        {
            this.parser = parser;
            this.logger = logger;
            this.nodes = new Dictionary<string, DeepEnds.Core.Dependent.Dependency>();
        }

        public void AddFile(string filePath, string filter)
        {
            if (!System.IO.File.Exists(filePath))
            {
                this.logger.Write("! Cannot find ");
                this.logger.WriteLine(filePath);
                return;
            }

            var branch = this.parser.Dependencies.GetPath(filter, "\\");
            var fileName = System.IO.Path.GetFileName(filePath);
            var leaf = this.parser.Create(fileName, filePath, branch);
            this.parser.Sources.Create(leaf, new Core.SourceProvider(leaf, filePath));
            this.nodes[filePath] = leaf;
        }

        protected string Node(string direc, string fileName, List<string> includes)
        {
            var filePath = DeepEnds.Core.Utilities.Combine(direc, fileName);
            if (System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            foreach (var path in includes)
            {
                filePath = DeepEnds.Core.Utilities.Combine(path, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return string.Empty;
        }

        static private bool IsCode(string line)
        {
            var i = line.IndexOf("//");
            if (i != -1)
            {
                line = line.Substring(0, i);
            }

            line = line.Replace('{', ' ');
            line = line.Replace('}', ' ');
            line = line.TrimStart();

            return line != string.Empty;
        }

        public void ReadFile(string filePath, List<string> includes)
        {
            if (!this.nodes.ContainsKey(filePath))
            {
                return;
            }

            DeepEnds.Core.Dependent.Dependency node = this.nodes[filePath];
            var direc = System.IO.Path.GetDirectoryName(filePath);
            int loc = 0;
            foreach (var line in DeepEnds.Core.Utilities.ReadFile(filePath).Split('\n'))
            {
                try
                {
                    if (IsCode(line))
                    {
                        ++loc;
                    }

                    if (line.Length < 8 || line.Substring(0, 8) != "#include")
                    {
                        continue;
                    }

                    var fileName = line.Substring(8).Trim();
                    var first = fileName[0];
                    fileName = fileName.Substring(1);
                    var index = fileName.Length;
                    if (first == '"')
                    {
                        index = fileName.IndexOf('"');
                    }
                    else if (fileName[0] == '<')
                    {
                        index = fileName.IndexOf('>');
                    }

                    fileName = fileName.Substring(0, index);

                    var dep = this.Node(direc, fileName, includes);
                    if (dep == null)
                    {
                        foreach (var inc in includes)
                        {
                            dep = this.Node(inc, fileName, includes);
                            if (dep != string.Empty)
                            {
                                break;
                            }
                        }
                    }

                    this.parser.AddDependency(node, dep);
                }
                catch
                {
                    continue;
                }
            }

            node.LOC = loc;
        }
    }
}
