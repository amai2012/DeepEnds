//------------------------------------------------------------------------------
// <copyright file="OutputPane.cs" company="Zebedee Mason">
//     Copyright (c) 2016-2017 Zebedee Mason.
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

// See https://mhusseini.wordpress.com/2013/06/06/write-to-visual-studios-output-window/

namespace DeepEnds.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OutputPane : System.IO.TextWriter
    {
        private Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane pane;

        public OutputPane()
        {
            var outputWindow = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsOutputWindow)) as Microsoft.VisualStudio.Shell.Interop.IVsOutputWindow;
            var paneGuid = new System.Guid("c3296bde-a1a4-4157-aad5-b344de40d936");
            outputWindow.CreatePane(paneGuid, "DeepEnds", 1, 0);
            outputWindow.GetPane(paneGuid, out this.pane);
        }

        public void Show()
        {
            EnvDTE.DTE dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as EnvDTE.DTE;
            dte.ExecuteCommand("View.Output", string.Empty);
            this.pane.Clear();
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }

        public override void Write(string value)
        {
            this.pane.OutputString(value);
        }

        public override void WriteLine(string value)
        {
            this.pane.OutputString(value);
            this.pane.OutputString("\n");
        }
    }
}
