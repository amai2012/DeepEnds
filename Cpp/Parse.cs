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

    public class Parse
    {
        private DeepEnds.Core.Parser parser;

        public Parse(DeepEnds.Core.Parser parser)
        {
            this.parser = parser;
        }

        protected void AddFile(string project, string filePath, string filter, List<DeepEnds.Core.Dependent.Dependency> nodes, System.IO.TextWriter logger)
        {
            if (!System.IO.File.Exists(filePath))
            {
                logger.Write("! Cannot find ");
                logger.WriteLine(filePath);
                return;
            }

            var branch = this.parser.Dependencies.GetPath(filter, "\\");
            var fileName = System.IO.Path.GetFileName(filePath);
            var leaf = this.parser.Create(fileName, filePath, branch);
            this.parser.Sources.Create(leaf, new SourceProvider(leaf, project, filePath));
            nodes.Add(leaf);
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

        protected void ReadFile(DeepEnds.Core.Dependent.Dependency node, List<string> includes)
        {
            var filePath = this.parser.Sources.AssociatedFilePath(node);
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
