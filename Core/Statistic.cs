//------------------------------------------------------------------------------
// <copyright file="Statistic.cs" company="Zebedee Mason">
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
    public class Statistic
    {
        public int MaxInTree { get; set; }

        public int SumOverTree { get; set; }

        public int Value { get; set; }

        public Statistic(int value)
        {
            this.Value = value;
            this.SumOverTree = value;
            this.MaxInTree = value;
        }

        public void AddChild(Statistic child)
        {
            this.SumOverTree += child.SumOverTree;

            if (child.MaxInTree > this.MaxInTree)
            {
                this.MaxInTree = child.MaxInTree;
            }
        }
    }
}
