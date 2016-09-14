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

namespace DeepEnds.Core
{
    using Core;
    using DeepEnds.Core.Dependent;
    using System.Collections.Generic;

    public class Structure
    {
        private Dependency parent;

        private int[,] matrix;

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

            this.matrix = new int[size, size];
            this.order = new int[size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    this.matrix[i, j] = 0;
                }

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
                    this.matrix[nodeA, nodeB] = 1;
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
            for (int i = 0; i < length; ++i)
            {
                var count = 0;
                for (int j = 0; j < length; ++j)
                {
                    count += this.matrix[i, j];
                }

                if (count == 0)
                {
                    reorder.Add(i);
                }
            }

            for (int i = reorder.Count; i < length; ++i)
            {
                var dummy = length;
                bool fine = true;
                for (int j = 0; j < length; ++j)
                {
                    fine = true;
                    if (reorder.Contains(j))
                    {
                        continue;
                    }

                    dummy = j;

                    for (int k = 0; k < length; ++k)
                    {
                        var val = this.matrix[j, k];
                        if (val == 0 || reorder.Contains(k))
                        {
                            continue;
                        }

                        fine = false;
                    }

                    if (fine)
                    {
                        break;
                    }
                }

                reorder.Add(dummy);
                if (!fine)
                {
                    this.HasCycle = true;
                }
            }

            this.order = reorder.ToArray();
        }

        public void Write(System.IO.StreamWriter file)
        {
            var length = this.parent.Children.Count;
            if (length == 0)
            {
                return;
            }

            file.Write("<p/>\n<table>\n");
            for (int i = 0; i < length; ++i)
            {
                file.Write(string.Format("<tr>"));
                file.Write(string.Format("<th>{0}</th>\n", this.parent.Children[this.order[i]].Name));
                for (int j = 0; j < length; ++j)
                {
                    var entry = string.Empty;
                    if (i == j)
                    {
                        entry = "\\";
                    }
                    else if (this.matrix[this.order[i], this.order[j]] == 1)
                    {
                        entry = "1";
                    }

                    file.Write(string.Format("<td>{0}</td>\n", entry));
                }

                file.Write(string.Format("</tr>\n"));
            }

            file.Write("</table>\n");
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
