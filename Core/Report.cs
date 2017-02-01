//------------------------------------------------------------------------------
// <copyright file="Report.cs" company="Zebedee Mason">
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

    public class Report
    {
        private Dictionary<string, string> options;

        public Report(Dictionary<string, string> options)
        {
            this.options = options;
        }

        private void HeaderAndFooter(Reporter reporter)
        {
            reporter.FileHeader = @"<!DOCTYPE html>
<html>
<head>
<title>Summary of graph complexity</title>
<style>
table, tr#main, th#main, td#main, td#alert {
    border: 1px solid black;
    border-collapse: collapse;
}
th#main {
    text-align: center;
}
th {
    font-weight: normal;
    background-color: LightGray;
    text-align: left;
}
td#alert {
    color:white;
    background-color:red;
}
</style>
</head>
<body>
";

            reporter.FileFooter = @"</body>
</html>
";
        }

        public void Write(DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var reporter = new Reporter(this.options["report"], this.options, dependencies);
            this.HeaderAndFooter(reporter);
            reporter.Link = "<a href=\"#section{0}\">{1}</a>";
            if (this.options["split"] != "false")
            {
                reporter.Link = string.Format("<a href=\"DeepEnds{{0}}{0}\">{{1}}</a>", System.IO.Path.GetExtension(this.options["report"]));
            }

            reporter.LinkExt = "<a href=\"{0}\">{1}</a>";
            reporter.ListBegin = "<ul>\n";
            reporter.ListEnd = "</ul>\n";
            reporter.ListItem = "<li>{0}</li>\n";
            reporter.ParagraphBegin = "<p>";
            reporter.ParagraphEnd = "</p>\n";
            reporter.RightArrow = "&rarr;";
            reporter.SectionBegin = "<h1><a id=\"section{0}\"></a>{1}</h1>\n";
            reporter.SubsectionBegin = "<h2>{1}</h2>\n<div>\n";
            reporter.SubsectionEnd = "</div>\n\n";
            reporter.SubSubsectionBegin = "<h3>{1}</h3>\n<div>\n";
            reporter.TableBegin = "<table{0}>\n";
            reporter.TableEnd = "</table>\n";
            reporter.TableBodyBegin = "<tbody>\n";
            reporter.TableBodyEnd = "</tbody>\n";
            reporter.TableBodyItem = "<td{0}>{1}</td>\n";
            reporter.TableHeadBegin = "<thead>\n";
            reporter.TableHeadEnd = "</thead>\n";
            reporter.TableHeadItem = "<th{0}{1}>{2}</th>\n";
            reporter.TableRowBegin = "<tr{0}{1}>\n";
            reporter.TableRowEnd = "</tr>\n";

            reporter.Report(false, false);
        }
    }
}
