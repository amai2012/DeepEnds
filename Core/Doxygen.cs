//------------------------------------------------------------------------------
// <copyright file="Doxygen.cs" company="Zebedee Mason">
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
    using DeepEnds.Core.Complex;
    using Dependent;

    using System.Collections.Generic;
    using System.Linq;

    public class Doxygen
    {
        private System.IO.StreamWriter file;

        private Dictionary<string, string> options;

        public Doxygen(Dictionary<string, string> options)
        {
            this.options = options;
        }

        public void Write(DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var fileName = this.options["doxygen"];
            this.file = new System.IO.StreamWriter(fileName);

            var reporter = new Reporter(this.file, this.options, dependencies);
            reporter.Link = "\\ref DeepEnds{0}";
            reporter.LineBegin = "//! ";
            reporter.ListEnd = "\n";
            reporter.ListItem = "- {0}\n";
            reporter.RightArrow = "&rarr;";
            reporter.SectionBegin = "\\page DeepEnds{0} {1}\n";
            reporter.SubsectionBegin = "\\section DeepEnds{0} {1}\n";
            reporter.TableBegin = "<table>\n";
            reporter.TableEnd = "</table>\n";
            reporter.TableBodyBegin = "\n";
            reporter.TableBodyEnd = "\n";
            reporter.TableBodyItem = "<td{0}>{1} ";
            reporter.TableHeadBegin = "\n";
            reporter.TableHeadEnd = "\n";
            reporter.TableHeadItem = "<th{0}>{1} ";
            reporter.TableRowBegin = "<tr{1}>";
            reporter.TableRowEnd = "\n";

            var fortran = new string[] { ".f", ".for", ".f90", ".f95", ".f03", ".f08" };
            var python = new string[] { ".py", ".pyw" };
            var vhdl = new string[] { ".vhd", ".vhdl", ".ucf", ".qsf" };

            var ext = System.IO.Path.GetExtension(fileName);
            if (fortran.Contains(ext))
            {
                this.file.WriteLine("!>");
                reporter.LineBegin = "!! ";
            }
            else if (python.Contains(ext) || ext == ".tcl")
            {
                this.file.WriteLine("##");
                reporter.LineBegin = "# ";
            }
            else if (vhdl.Contains(ext))
            {
                reporter.LineBegin = "--! ";
            }

            reporter.Report(true);

            this.file.Close();
        }
    }
}
