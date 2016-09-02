//------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core
{
    using System.Collections.Generic;
    using System.IO;

    public class Utilities
    {
        public static string ReadFile(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static void ReadProject(string project, Dictionary<string, string> files, System.Text.StringBuilder messages)
        {
            var direc = System.IO.Path.GetDirectoryName(project);
            foreach (var line in DeepEnds.Core.Utilities.ReadFile(project).Split('\n'))
            {
                var trimmed = line.Trim();

                if (trimmed.Length < 8)
                {
                    continue;
                }

                if (trimmed.Substring(0, 8) != "<Compile")
                {
                    continue;
                }

                trimmed = trimmed.Substring(trimmed.IndexOf('"') + 1);
                trimmed = trimmed.Substring(0, trimmed.IndexOf('"'));
                var file = System.IO.Path.Combine(direc, trimmed);
                if (!System.IO.File.Exists(file))
                {
                    messages.AppendFormat("! Cannot find {0}\n", file);
                    continue;
                }

                messages.AppendFormat("  Appended {0}\n", file);
                files[file] = project;
            }
        }

        public static List<string> ReadVisualStudioSolution(string slnfilename)
        {
            List<string> projects = new List<string>();
            var direc = System.IO.Path.GetDirectoryName(slnfilename);
            foreach (var line in Utilities.ReadFile(slnfilename).Split('\n'))
            {
                if (line.Length <= 10)
                {
                    continue;
                }

                if (line.Substring(0, 10) != "Project(\"{")
                {
                    continue;
                }

                var bits = line.Split(',');
                var filename = bits[1].Trim().Substring(1);
                filename = filename.Substring(0, filename.Length - 1);
                var ext = System.IO.Path.GetExtension(filename);
                if (ext == ".csproj" || ext == ".vbproj" || ext == ".vcxproj")
                {
                    projects.Add(System.IO.Path.Combine(direc, filename));
                }
            }

            return projects;
        }

        public static string Combine(string direc, string fileName)
        {
            var filePath = System.IO.Path.Combine(direc, fileName).Replace("/", "\\");
            filePath = System.IO.Path.GetFullPath(filePath);
            return filePath;
        }
    }
}
