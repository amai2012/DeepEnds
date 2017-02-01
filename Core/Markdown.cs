//------------------------------------------------------------------------------
// <copyright file="Markdown.cs" company="Zebedee Mason">
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

    public class Markdown
    {
        private Dictionary<string, string> options;

        public Markdown(Dictionary<string, string> options)
        {
            this.options = options;
        }

        public void Write(DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var reporter = new Reporter(this.options["report"], this.options, dependencies);
            reporter.Link = "{1}";
            reporter.LinkExt = "[{1}]({0})";
            reporter.ListItem = "* {0}\n";
            reporter.ParagraphEnd = "\n";
            reporter.SectionBegin = "# {1}\n\n";
            reporter.SubsectionBegin = "## {1}\n\n";
            reporter.SubsectionEnd = "\n";
            reporter.TableBodyItem = "{1} |";
            reporter.TableDivItem = "-";
            reporter.TableHeadItem = "{2} |";
            reporter.TableRowBegin = "| ";
            reporter.TableRowEnd = "\n";

            reporter.Report(false, true);
        }
    }
}
