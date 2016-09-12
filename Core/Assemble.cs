//------------------------------------------------------------------------------
// <copyright file="Assemble.cs" company="Zebedee Mason">
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
    using Dependent;
    using System.Collections.Generic;

    public class Assemble : DependencyWalker
    {
        private Dictionary<Dependency, Links> links;

        private Dictionary<Dependency, Externals> externals;

        public Assemble(Dictionary<Dependency, Links> links, Dictionary<Dependency, Externals> externals)
        {
            this.links = links;
            this.externals = externals;
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);

            if (this.links[dependency].Dependencies.Count != 0)
            {
                return;
            }

            foreach (var dep in dependency.Dependencies)
            {
                this.externals[dependency].Add(dep);
            }

            this.externals[dependency].SetMax();

            foreach (var child in dependency.Children)
            {
                this.externals[dependency].Add(this.externals[child]);
            }

            foreach (var child in dependency.Children)
            {
                foreach (var dep in child.Dependencies)
                {
                    this.links[child].Add(dep);
                }

                foreach (var dep in this.links[child].Dependencies)
                {
                    this.links[dependency].Add(dep);
                }
            }

            foreach (var child in dependency.Children)
            {
                foreach (var link in this.links[child].Dependencies)
                {
                    var dep = link.FindChild(dependency);
                    if (dep == null || dep == child)
                    {
                        continue;
                    }

                    var links = this.links[child];
                    if (links.Interlinks.Contains(dep))
                    {
                        continue;
                    }

                    links.Interlinks.Add(dep);
                }
            }
        }
    }
}
