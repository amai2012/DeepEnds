//------------------------------------------------------------------------------
// <copyright file="Structure.cs" company="Zebedee Mason">
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
    using Core;
    using System.Collections.Generic;

    public class Structure
    {
        private Dependency parent;

        private List<int>[] matrix;

        private int[] order;

        public bool HasCycle { get; set; }

        internal Structure(Dependency parent)
        {
            this.parent = parent;

            var size = parent.Children.Count;
            if (size == 0)
            {
                this.matrix = null;
                return;
            }

            this.matrix = new List<int>[size];
            this.order = new int[size];
            for (int i = 0; i < size; ++i)
            {
                this.matrix[i] = new List<int>();
                this.order[i] = i;
            }

            this.HasCycle = false;
        }

        public void Layer(Dictionary<Dependency, Links> links)
        {
            int index = 0;
            var nodes = new Dictionary<Dependency, int>();
            foreach (var child in this.parent.Children)
            {
                nodes[child] = index;
                ++index;
            }

            foreach (var child in this.parent.Children)
            {
                var nodeA = nodes[child];
                foreach (var dep in links[child].Interlinks)
                {
                    var nodeB = nodes[dep];
                    this.matrix[nodeA].Add(nodeB);
                }
            }
        }

        private void Order()
        {
            var length = this.parent.Children.Count;
            if (length == 0)
            {
                return;
            }

            var reorder = new List<int>();
            var remaining = new List<int>();

            // Add all nodes with no dependencies to reorder
            for (int i = 0; i < length; ++i)
            {
                if (this.matrix[i].Count == 0)
                {
                    reorder.Add(i);
                }
                else
                {
                    remaining.Add(i);
                }
            }

            // Try to remove more
            var left = remaining.Count + 1;
            while (remaining.Count > 0)
            {
                if (remaining.Count == left)
                {
                    this.HasCycle = true;
                    break;
                }

                left = remaining.Count;
                for (int i = 0; i < left; ++i)
                {
                    var test = remaining[i];
                    var covered = true;
                    foreach (var index in this.matrix[test])
                    {
                        if (reorder.Contains(index))
                        {
                            continue;
                        }

                        covered = false;
                        break;
                    }

                    if (covered)
                    {
                        reorder.Add(test);
                        remaining.Remove(test);
                        break;
                    }
                }
            }

            // Have broken early if there are any remaining
            reorder.AddRange(remaining);

            this.order = reorder.ToArray();
        }

        public List<KeyValuePair<string, string>> Extract()
        {
            var length = this.parent.Children.Count;

            var list = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < length; ++i)
            {
                var row = string.Empty;
                for (int j = 0; j < length; ++j)
                {
                    var entry = string.Empty;
                    if (i == j)
                    {
                        row += '\\';
                    }
                    else if (this.matrix[this.order[i]].Contains(this.order[j]))
                    {
                        row += '1';
                    }
                    else
                    {
                        row += ' ';
                    }
                }

                var pair = new KeyValuePair<string, string>(this.parent.Children[this.order[i]].Name, row);
                list.Add(pair);
            }

            return list;
        }

        public static void Assemble(Dependency dependency, Dictionary<Dependency, Links> links, Dictionary<Dependency, Structure> structures)
        {
            var structure = new Structure(dependency);
            structures[dependency] = structure;
            structure.Layer(links);
            structure.Order();
        }
    }
}
