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

        public View()
        {
            this.dependencies = new Dependencies();
            this.sets = new Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>>();
            this.leaves = new Dictionary<string, DeepEnds.Core.Dependent.Dependency>();
            this.sources = new Sources();
        }

        private void Files(System.IO.TextWriter logger, string fileName, string direc, List<KeyValuePair<string, string>> fileNames, List<string> extensions)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            if (ext == ".vcxproj" || ext == ".csproj" || ext == ".vbproj" || ext == ".exe" || ext == ".dll")
            {
                logger.Write("  Appended for reading ");
                logger.WriteLine(fileName);
                if (!extensions.Contains(ext))
                {
                    extensions.Add(ext);
                }

                fileNames.Add(new KeyValuePair<string, string>(fileName, direc));
            }
            else if (ext == ".sln")
            {
                logger.Write(" Reading projects from ");
                logger.WriteLine(fileName);
                direc = System.IO.Path.GetDirectoryName(fileName);
                foreach (var name in DeepEnds.Core.Utilities.ReadVisualStudioSolution(fileName))
                {
                    this.Files(logger, name, direc, fileNames, extensions);
                }
            }
            else if (ext == ".xml")
            {
                direc = System.IO.Path.GetDirectoryName(fileName);
                logger.Write(" Reading xml from ");
                logger.WriteLine(direc);
                fileNames.Add(new KeyValuePair<string, string>(fileName, direc));
            }
        }

        public bool Read(System.IO.TextWriter logger, Dictionary<string, string> options, string[] lines)
        {
            logger.WriteLine("Reading");
            var fileNames = new List<KeyValuePair<string, string>>();
            var extensions = new List<string>();
            foreach (var fileName in lines)
            {
                this.Files(logger, fileName, string.Empty, fileNames, extensions);
            }

            if (extensions.Count != 1 && extensions.Contains(".vcxproj"))
            {
                logger.Write("Cannot mix vcxproj and others, have :");
                foreach (var ext in extensions)
                {
                    logger.Write(" ");
                    logger.Write(ext);
                }

                return false;
            }

            this.sources = new Sources();
            this.dependencies = new Dependencies();
            var parser = new Parser(this.dependencies, this.sources);

            if (extensions.Contains(".vcxproj"))
            {
                options["sep"] = "\\";
            }

            var csharp = new CSharp.Parse(parser);
            var vbasic = new VBasic.Parse(parser);
            var cpp = new Cpp.ParseVS(parser);
            var dotnet = new Decompile.Parse(parser);
            var xml = new DoxygenXml.Parse(parser, options);

            var dlls = new List<string>();
            foreach (var pair in fileNames)
            {
                var ext = System.IO.Path.GetExtension(pair.Key);

                if (ext == ".dll" || ext == ".exe")
                {
                    dlls.Add(pair.Key);
                    dotnet.Read(pair.Key, logger);
                }
                else if (ext == ".csproj")
                {
                    csharp.ReadProject(pair.Key, logger);
                }
                else if (ext == ".vbproj")
                {
                    vbasic.ReadProject(pair.Key, logger);
                }
                else if (ext == ".vcxproj")
                {
                    cpp.Read(pair.Key, pair.Value, logger);
                }
                else if (ext == ".xml")
                {
                    xml.Read(pair.Value, logger);
                }
            }

            csharp.Finalise(dlls);
            vbasic.Finalise(dlls);
            xml.Finalise();
            parser.Finalise(options["sep"]);

            return true;
        }

        public void Write(System.IO.TextWriter logger, Dictionary<string, string> options)
        {
            if (this.dependencies.Root.Children.Count == 0)
            {
                logger.WriteLine("Writing nothing so exiting");
                return;
            }

            if (options["report"] != string.Empty)
            {
                logger.Write("Writing ");
                logger.WriteLine(options["report"]);
                var report = new DeepEnds.Core.Report(options);
                report.Write(this.dependencies);
            }

            if (options["doxygen"] != string.Empty)
            {
                logger.Write("Writing ");
                logger.WriteLine(options["doxygen"]);
                var report = new DeepEnds.Core.Doxygen(options);
                report.Write(this.dependencies);
            }

            if (options["csv"] != string.Empty)
            {
                logger.Write("Writing ");
                logger.WriteLine(options["csv"]);
                var report = new DeepEnds.Core.CSV(options);
                report.Write(this.dependencies);
            }

            if (options["graph"] != string.Empty)
            {
                logger.Write("Writing ");
                logger.WriteLine(options["graph"]);
                var assemble = DeepEnds.DGML.Assemble.Factory(options, this.dependencies.Root, this.dependencies.Assembled.Linkings, this.sources, true);
                assemble.Save();
            }
        }
    }
}
