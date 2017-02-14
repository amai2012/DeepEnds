//------------------------------------------------------------------------------
// <copyright file="Complexities.cs" company="Zebedee Mason">
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

namespace DeepEnds.Reporting
{
    using DeepEnds.Core;
    using System.Collections.Generic;
    using System.Linq;

    public class Complexities : DependencyWalker
    {
        public List<Complexity> List { get; }

        private Dictionary<Dependency, Links> links;

        private Dictionary<Dependency, Complexity> summed;

        public Complexities(Dictionary<Dependency, Links> links)
        {
            this.List = new List<Complexity>();
            this.links = links;
            this.summed = new Dictionary<Dependency, Complexity>();
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);
            if (dependency.Children.Count != 0)
            {
                var item = Complexities.Complexity(dependency, this.links);
                this.List.Add(item);
                this.summed[dependency] = item;
                foreach (var child in dependency.Children)
                {
                    var complexity = this.summed[child];
                    if (complexity == null)
                    {
                        continue;
                    }

                    item.Ns.AddChild(complexity.Ns);
                    item.EPNs.AddChild(complexity.EPNs);
                    item.EPNNs.AddChild(complexity.EPNNs);
                }
            }
            else
            {
                this.summed[dependency] = new Complexity(0, 0, dependency);
            }
        }

        public static List<Complexity> Factory(Dependency root, Dictionary<Dependency, Links> links)
        {
            var complexities = new Complexities(links);
            complexities.Visit(root);
            return complexities.List.OrderByDescending(o => o.EPNNs.Value).ToList();
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
