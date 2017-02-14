//------------------------------------------------------------------------------
// <copyright file="Splitter.cs" company="Zebedee Mason">
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

namespace DeepEnds.Console
{
    public class Splitter
    {
        private System.IO.TextWriter splitter;

        private int windowWidth;

        public Splitter(System.IO.TextWriter splitter, int windowWidth)
        {
            this.splitter = splitter;
            this.windowWidth = windowWidth;
        }

        public void WriteLine(string indent, string line)
        {
            var width = this.windowWidth - indent.Length;
            if (width < 10)
            {
                width = 10;
            }

            while (line.Length > 0)
            {
                if (width >= line.Length)
                {
                    this.splitter.Write(indent);
                    this.splitter.WriteLine(line);
                    return;
                }

                var length = line.LastIndexOf(' ', width);
                if (length == 0)
                {
                    this.splitter.Write(indent);
                    this.splitter.WriteLine(line);
                    return;
                }

                this.splitter.Write(indent);
                this.splitter.WriteLine(line.Substring(0, length));
                line = line.Substring(length);
            }
        }
    }
}
