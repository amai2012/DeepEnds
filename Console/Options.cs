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
        /// <returns>Array of arguments ordered for display</returns>
        static public string[] Ordered()
        {
            return new string[] { "report", "csv", "doxygen", "graph", "filter", "parser", "compoundtype", "membertype", "memberhide", "source", "filenames" };
        }

        /// <returns>Default values for the arguments</returns>
        static public Dictionary<string, string> Defaults()
        {
            var options = new Dictionary<string, string>();
            options["compoundtype"] = "class,struct,union,interface";
            options["csv"] = string.Empty;
            options["doxygen"] = string.Empty;
            options["filenames"] = string.Empty;
            options["filter"] = "default";
            options["graph"] = string.Empty;
            options["membertype"] = "enum,function,enumvalue";
            options["memberhide"] = "true";
            options["parser"] = "default";
            options["report"] = string.Empty;
            options["sep"] = ".";
            options["source"] = string.Empty;
            return options;
        }

        /// <returns>Help for the arguments</returns>
        static public Dictionary<string, string> Help()
        {
            var options = new Dictionary<string, string>();
            options["compoundtype"] = "Comma separated list of supported values for the kind attribute of a Doxygen XML compounddef element. Types include class, struct, union, interface, protocol, category, exception, file, namespace, group, page, example, dir";
            options["csv"] = "Write a csv file containing the main table from the statistics";
            options["doxygen"] = "Write the statistics to a source file for Doxygen to process";
            options["filenames"] = "A list of sln, csproj, vcxproj, vbproj, dll and exe files or one example Doxygen XML file";
            options["filter"] = "Optional way of specifying the filter. For Visual C++ with default parser the option is \"directory\"";
            options["graph"] = "Write a DGML file (*.dgml) for Visual Studio";
            options["membertype"] = "Comma separated list of supported values for the kind attribute of a Doxygen XML memberdef element. Types include define, property, variable, typedef, enum, function, signal, prototype, friend, dcop, slot, enumvalue";
            options["memberhide"] = "Whether to produce a leaf for a Doxygen XML memberdef element. Values are true and false";
            options["parser"] = "Optional parser. For Visual C++ the option is \"libclang\"";
            options["report"] = "Write a HTML (*.html|*.htm) or markdown (*.md) file containing various statistics";
            options["source"] = "The directory containing the source (used for DGML and/or vcxproj.filter files)";
            return options;
        }

        /// <summary>
        /// Action on pressing a browse button
        /// </summary>
        public enum Browse
        {
            fileOut,
            fileIn,
            directoryIn
        }

        /// <returns>Type of action to occur when browse button is pressed</returns>
        static public Dictionary<string, Browse> Types()
        {
            var options = new Dictionary<string, Browse>();
            options["csv"] = Browse.fileOut;
            options["doxygen"] = Browse.fileOut;
            options["filenames"] = Browse.fileIn;
            options["graph"] = Browse.fileOut;
            options["report"] = Browse.fileOut;
            options["source"] = Browse.directoryIn;
            return options;
        }

        /// <returns>File filters for browse button</returns>
        static public Dictionary<string, string> Filters()
        {
            var options = new Dictionary<string, string>();
            options["csv"] = "comma separated variable (.csv)|*.csv";
            options["doxygen"] = "C/C++ (.dox, .txt, .doc, .c, .C, .cc, .CC, .cxx, .cpp, .c++, .ii, .ixx, .ipp, .i++, .inl, .h, .H, .hh, .HH, .hxx, .hpp, .h++, .mm)|*.dox;*.txt;*.doc;*.c;*.C;*.cc;*.CC;*.cxx;*.cpp;*.c++;*.ii;*.ixx;*.ipp;*.i++;*.inl;*.h;*.H;*.hh;*.HH;*.hxx;*.hpp;*.h++;*.mm|C# (.cs)|*.cs|D (.d)|*.d|Fortran (.f, .for, .f90, .f95, .f03, .f08)|*.f;*.for;*.f90;*.f95;*.f03;*.f08|IDL (.idl, .ddl, .odl)|*.idl;*.ddl;*.odl|Java (.java)|*.java|Objective-C (.m, .M)|*.m;*.M|PHP (.php, .php4, .php5, .inc, .phtml)|*.php;*.php4;*.php5;*.inc;*.phtml|Python (.py, .pyw)|*.py;*.pyw|TCL (.tcl)|*.tcl|VHDL (.vhd, .vhdl, .ucf, .qsf)|*.vhd;*.vhdl;*.ucf;*.qsf";
            options["filenames"] = "MS Visual Studio solution (.sln)|*.sln|C++ Project (.vcxproj)|*.vcxproj|C# Project (.csproj)|*.csproj|VB.NET Project (.vbproj)|*.vbproj|.NET assemblies (.dll)|*.dll|.NET executables (.exe)|*.exe|Doxygen XML output (.xml)|*.xml";
            options["graph"] = "Directed Graph Markup Language (.dgml)|*.dgml";
            options["report"] = "HTML (.html, .htm)|*.html;*.htm|Markdown (.md)|*.md";
            return options;
        }
    }
}
