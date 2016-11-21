//------------------------------------------------------------------------------
// <copyright file="Printer.cs" company="Zebedee Mason">
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

namespace DeepEnds.Cpp.Clang
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClangSharp;

    public class Printer
    {
        private readonly TextWriter logger;

        private string indent;

        public Printer(TextWriter logger)
        {
            this.logger = logger;
            this.indent = "  ";
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);
            var displayName2 = clang.getCursorDisplayName(cursor).ToString();
            var name2 = clang.getCursorSpelling(cursor).ToString();
            this.logger.WriteLine(string.Format("{0}{1} {2} {3}", this.indent, curKind.ToString(), name2, displayName2));

            var old = this.indent;
            this.indent += " ";
            clang.visitChildren(cursor, this.Visit, new CXClientData(IntPtr.Zero));
            this.indent = old;
            return CXChildVisitResult.CXChildVisit_Continue;
        }
    }
}
