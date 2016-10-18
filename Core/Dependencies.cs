//------------------------------------------------------------------------------
// <copyright file="Dependencies.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core.Linked
{
    using DeepEnds.Core.Dependent;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Dependencies
    {
        public Assemble Assembled { get; set; }

        public Dependency Root { get; }

        public Dependencies()
        {
            this.Root = this.Create(string.Empty, null);
        }

        public Dependency Create(string name, Dependency parent)
        {
            var dep = new Dependency(name, parent);
            return dep;
        }

        public static Tuple<string, string> SplitPath(string path, string sep)
        {
            if (!path.Contains(sep))
            {
                return new Tuple<string, string>(string.Empty, path);
            }

            var i = path.LastIndexOf(sep);
            var parent = path.Substring(0, i);
            var name = path.Substring(i + sep.Length);
            return new Tuple<string, string>(parent, name);
        }

        public Dependency GetPath(string path, string sep)
        {
            var item = this.Root.Find(path, sep);
            if (item != null)
            {
                return item;
            }

            var name = path;
            var parent = this.Root;

            var bits = Dependencies.SplitPath(path, sep);
            if (bits.Item1 != string.Empty)
            {
                parent = this.GetPath(bits.Item1, sep);
                name = bits.Item2;
            }

            var branch = this.Create(name, parent);
            parent.AddChild(branch);
            return branch;
        }

        public List<Dependency> Branches()
        {
            var branches = new List<Dependency>();
            this.Root.Branches(branches);
            return branches.OrderBy(o => o.Path(".")).ToList();
        }

        public void Assemble()
        {
            this.Assembled = new Assemble();
            this.Assembled.Visit(this.Root);
        }

        public void Poach(Dependency branch, Dependency[] branches)
        {
            var clear = new ClearDependencies(this.Assembled.Linkings);
            clear.Visit(branch);
            foreach (var child in branches)
            {
                var parent = child.Parent;
                child.Parent = branch;
                clear.Visit(parent);

                parent.Children.Remove(child);
                branch.Children.Add(child);
            }
        }
    }
}
