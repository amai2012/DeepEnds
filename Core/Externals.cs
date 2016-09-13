//------------------------------------------------------------------------------
// <copyright file="Externals.cs" company="Zebedee Mason">
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
    using Dependent;
    using System.Collections.Generic;

    public class Externals
    {
        private Dependency dependency;

        public List<Dependency> Merged { get; }

        public int MaxInTree { get; set; }

        public Externals(Dependency dependency)
        {
            this.dependency = dependency;
            this.Merged = new List<Dependency>();
        }

        public void SetMax()
        {
            this.MaxInTree = this.Merged.Count;
        }

        public void Add(Dependency dep)
        {
            if (this.Merged.Contains(dep))
            {
                return;
            }

            if (dep.FindChild(this.dependency) != null)
            {
                return;
            }

            this.Merged.Add(dep);
        }

        public void Add(Externals externals)
        {
            foreach (var dep in externals.Merged)
            {
                this.Add(dep);
            }

            var num = externals.MaxInTree;
            if (this.MaxInTree < num)
            {
                this.MaxInTree = num;
            }
        }

        public static void Assemble(Dependency dependency, Dictionary<Dependency, Externals> externals)
        {
            foreach (var dep in dependency.Dependencies)
            {
                externals[dependency].Add(dep);
            }

            externals[dependency].SetMax();

            foreach (var child in dependency.Children)
            {
                externals[dependency].Add(externals[child]);
            }
        }
    }
}
