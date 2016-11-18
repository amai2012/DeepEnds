//------------------------------------------------------------------------------
// <copyright file="CalculateUsage.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core.Dependent
{
    using System.Collections.Generic;

    internal class CalculateUsage : DependencyWalker
    {
        private Dependency leaf;

        public Dictionary<Dependency, int> Counts { get; }

        internal CalculateUsage(Dependency leaf)
        {
            this.leaf = leaf;
            this.Counts = new Dictionary<Dependency, int>();
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);

            int count = 0;
            foreach (var child in dependency.Children)
            {
                if (child.Dependencies.Contains(this.leaf))
                {
                    ++count;
                }

                if (this.Counts.ContainsKey(child))
                {
                    count += this.Counts[child];
                }
            }

            this.Counts[dependency] = count;
        }

        public static int[] Get(Dependency root, Dependency leaf, List<Dependency> levels)
        {
            var walker = new CalculateUsage(leaf);
            walker.Visit(root);

            var counts = new int[levels.Count];
            for (int i = 0; i < levels.Count; ++i)
            {
                counts[i] = walker.Counts[levels[i]];
            }

            return counts;
        }
    }
}
