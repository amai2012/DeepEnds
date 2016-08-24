//------------------------------------------------------------------------------
// <copyright file="Summed.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core.Complex
{
    public class Summed
    {
        public int SumN { get; set; }

        public int SumEPN { get; set; }

        public double SumEPNN { get; set; }

        public Complexity Complexity { get; }

        public Summed(Complexity complexity)
        {
            if (complexity != null)
            {
                this.SumN = complexity.N;
                this.SumEPN = complexity.EPN;
                this.SumEPNN = complexity.EPNN;
            }
            else
            {
                this.SumN = 0;
                this.SumEPN = 0;
                this.SumEPNN = 0.0;
            }

            this.Complexity = complexity;
        }

        public string[] StatusStrings(string sep)
        {
            var strings = this.Complexity.StatusStrings(sep);
            return new string[] { strings[0], strings[1], this.SumN.ToString(), strings[2], this.SumEPN.ToString(), strings[3], string.Format("{0:0.00}", this.SumEPNN) };
        }
    }
}
