//------------------------------------------------------------------------------
// <copyright file="SLOC.cs" company="Zebedee Mason">
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
    using Dependent;
    using System.Collections.Generic;

    public class SLOC
    {
        public double SumLog { get; set; }

        public double SumLog2 { get; set; }

        public int MaxInTree { get; set; }

        public int SumOverTree { get; set; }

        public int NumFiles { get; set; }

        public SLOC()
        {
            this.SumLog = 0.0;
            this.SumLog2 = 0.0;
            this.MaxInTree = 0;
            this.SumOverTree = 0;
            this.NumFiles = 0;
        }

        public void Stats(out int lower, out int upper, out int expected)
        {
            if (this.NumFiles <= 1)
            {
                lower = upper = expected = this.SumOverTree;
                return;
            }

            var mean = this.SumLog / this.NumFiles;
            var variance = (this.SumLog2 - this.SumLog * mean) / (this.NumFiles - 1.0);
            var sd = System.Math.Sqrt(variance);

            lower = (int)System.Math.Pow(10, mean - 1.645 * sd);
            upper = (int)System.Math.Pow(10, mean + 1.645 * sd);
            expected = (int)System.Math.Pow(10, mean);
        }

        private void Append(int loc)
        {
            if (loc == 0)
                return;

            if (this.MaxInTree < loc)
            {
                this.MaxInTree = loc;
            }

            this.SumOverTree += loc;
            ++this.NumFiles;

            var logSLOC = System.Math.Log10(loc);
            this.SumLog += logSLOC;
            this.SumLog2 += logSLOC * logSLOC;
        }

        private void Append(SLOC loc)
        {
            if (this.MaxInTree < loc.MaxInTree)
            {
                this.MaxInTree = loc.MaxInTree;
            }

            this.SumOverTree += loc.SumOverTree;
            this.NumFiles += loc.NumFiles;

            this.SumLog += loc.SumLog;
            this.SumLog2 += loc.SumLog2;
        }

        public static void Assemble(Dependency dependency, Dictionary<Dependency, SLOC> slocs)
        {
            var sloc = new SLOC();
            slocs[dependency] = sloc;

            sloc.Append(dependency.LOC);
            foreach (var child in dependency.Children)
            {
                sloc.Append(slocs[child]);
            }
        }
    }
}
