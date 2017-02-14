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

namespace DeepEnds.Reporting
{
    using DeepEnds.Core;
    using System.Collections.Generic;

    internal class CalculateUsage
    {
        private Dependency root;

        public Dictionary<Dependency, int[]> Usages { get; }

        public Dictionary<Dependency, List<Dependency>> Interfaces { get; }

        public CalculateUsage(Dependency root, Dictionary<Dependency, int[]> usages, Dictionary<Dependency, List<Dependency>> interfaces)
        {
            this.root = root;
            this.Usages = usages;
            this.Interfaces = interfaces;
        }

        public int Count(Dependency dependency, Dependency leaf, List<Dependency> levels, int[] counts)
        {
            int count = 0;
            foreach (var child in dependency.Children)
            {
                if (child.Dependencies.Contains(leaf))
                {
                    ++count;
                }

                count += this.Count(child, leaf, levels, counts);
            }

            if (levels.Contains(dependency))
            {
                counts[levels.IndexOf(dependency)] = count;
            }

            return count;
        }

        public void Calculate(Dependency dependency)
        {
            if (dependency.Children.Count == 0 && dependency.Parent != null)
            {
                var levels = CalculateUsage.Levels(dependency.Parent);

                var counts = new int[levels.Count];
                var walker = this.Count(this.root, dependency, levels, counts);
                this.Usages[dependency] = counts;

                var max = counts[levels.Count - 1];
                for (int i = 0; i < levels.Count; ++i)
                {
                    if (counts[i] == max)
                    {
                        if (!this.Interfaces.ContainsKey(levels[i]))
                        {
                            this.Interfaces[levels[i]] = new List<Dependency>();
                        }

                        this.Interfaces[levels[i]].Add(dependency);
                        break;
                    }
                }
            }

            foreach (var child in dependency.Children)
            {
                this.Calculate(child);
            }
        }

        public static List<Dependency> Levels(Dependency branch)
        {
            var levels = new List<Dependency>();
            while (branch != null)
            {
                levels.Add(branch);
                branch = branch.Parent;
            }

            return levels;
        }
    }
}
