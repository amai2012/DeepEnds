//------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Zebedee Mason">
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

namespace DeepEnds.Console
{
    using System.Collections.Generic;

    static public class Options
    {
        static public string[] Internals()
        {
            return new string[] { "filenames", "sep" };
        }

        static public Dictionary<string, string> Defaults()
        {
            var options = new Dictionary<string, string>();
            options["csv"] = string.Empty;
            options["doxygen"] = string.Empty;
            options["filenames"] = string.Empty;
            options["graph"] = string.Empty;
            options["report"] = string.Empty;
            options["sep"] = ".";
            options["source"] = string.Empty;
            return options;
        }

        static public Dictionary<string, string> Help()
        {
            var options = new Dictionary<string, string>();
            options["csv"] = "Write a csv file containing the main table";
            options["doxygen"] = "Write a source file for Doxygen to process, create dot files in the same directory";
            options["filenames"] = "a list of xml, sln, csproj, vcxproj, vbproj, dll and exe files\n  for parsing Doxygen XML output supply one xml file and set source";
            options["graph"] = "Write a DGML file (*.dgml) for Visual Studio";
            options["report"] = "Write a HTML file (*.html|*.htm) containing various statistics";
            options["source"] = "The directory containing the source (used by Doxygen XML input for DGML)";
            return options;
        }

        static public Dictionary<string, string> Types()
        {
            var options = new Dictionary<string, string>();
            options["csv"] = "fileOut";
            options["doxygen"] = "fileOut";
            options["filenames"] = "fileIn";
            options["graph"] = "fileOut";
            options["report"] = "fileOut";
            options["source"] = "directoryIn";
            return options;
        }

        static public Dictionary<string, string> Filters()
        {
            var options = new Dictionary<string, string>();
            options["csv"] = "comma separated variable (.csv)|*.csv";
            options["doxygen"] = "C# (.cs)|*.cs";
            options["filenames"] = "MS Visual Studio solution (.sln)|*.sln|C++ Project (.vcxproj)|*.vcxproj|C# Project (.csproj)|*.csproj|VB.NET Project (.vbproj)|*.vbproj|.NET assemblies (.dll)|*.dll|.NET executables (.exe)|*.exe|Doxygen XML output (.xml)|*.xml";
            options["graph"] = "Directed Graph Markup Language (.dgml)|*.dgml";
            options["report"] = "Report (.html, .htm)|*.html;*.htm";
            return options;
        }
    }
}
