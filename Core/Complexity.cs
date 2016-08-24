//------------------------------------------------------------------------------
// <copyright file="Complexity.cs" company="Zebedee Mason">
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
    using Dependent;

    public class Complexity
    {
        public int N { get;  }

        public int EPN { get; }

        public double EPNN
        {
            get
            {
                if (this.N == 0)
                {
                    return 0.0;
                }

                return this.EPN / (1.0 * this.N);
            }
        }

        public Dependency Branch { get; }

        public Complexity(int n, int epn, Dependency branch)
        {
            this.N = n;
            this.EPN = epn;
            this.Branch = branch;
        }

        public string[] StatusStrings(string sep)
        {
            return new string[] { this.Branch.Path(sep), this.N.ToString(), this.EPN.ToString(), string.Format("{0:0.00}", this.EPNN) };
        }
    }
}
