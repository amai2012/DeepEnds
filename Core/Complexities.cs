//------------------------------------------------------------------------------
// <copyright file="Complexities.cs" company="Zebedee Mason">
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
    using System.Collections.Generic;
    using System.Linq;

    public class Complexities : DependencyWalker
    {
        public List<Summed> List { get; }

        private Dictionary<Dependency, Links> links;

        private Dictionary<Dependency, Summed> summed;

        public Complexities(Dictionary<Dependency, Links> links)
        {
            this.List = new List<Summed>();
            this.links = links;
            this.summed = new Dictionary<Dependency, Summed>();
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);
            if (dependency.Children.Count != 0)
            {
                var item = new Summed(Complexities.Complexity(dependency, this.links));
                this.List.Add(item);
                this.summed[dependency] = item;
                foreach (var child in dependency.Children)
                {
                    item.SumN += this.summed[child].SumN;
                    item.SumEPN += this.summed[child].SumEPN;
                    item.SumEPNN += this.summed[child].SumEPNN;
                }
            }
            else
            {
                this.summed[dependency] = new Summed(null);
            }
        }

        public static List<Summed> Factory(Dependency root, Dictionary<Dependency, Links> links)
        {
            var complexities = new Complexities(links);
            complexities.Visit(root);
            return complexities.List.OrderByDescending(o => o.Complexity.EPNN).ToList();
        }

        public static Complexity Complexity(Dependency branch, Dictionary<Dependency, Links> links)
        {
            int edges = 0;
            foreach (var node in branch.Children)
            {
                edges += links[node].Interlinks.Count;
            }

            var mapping = new Dictionary<Dependency, int>();
            int label = 0;

            List<int> parts;
            foreach (var node in branch.Children)
            {
                parts = new List<int>();
                if (mapping.Keys.Contains(node))
                {
                    parts.Add(mapping[node]);
                }

                foreach (var dep in links[node].Interlinks)
                {
                    if (mapping.Keys.Contains(dep) && !parts.Contains(mapping[dep]))
                    {
                        parts.Add(mapping[dep]);
                    }

                    mapping[dep] = label;
                }

                var keys = new List<Dependency>(mapping.Keys);
                foreach (var dep in keys)
                {
                    if (parts.Contains(mapping[dep]))
                    {
                        mapping[dep] = label;
                    }
                }

                mapping[node] = label;
                label += 1;
            }

            parts = new List<int>();
            foreach (var dep in mapping.Keys)
            {
                if (!parts.Contains(mapping[dep]))
                {
                    parts.Add(mapping[dep]);
                }
            }

            int n = branch.Children.Count;
            int epn = edges + parts.Count - n;
            return new Complexity(n, epn, branch);
        }
    }
}
