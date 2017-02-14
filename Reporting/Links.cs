//------------------------------------------------------------------------------
// <copyright file="Links.cs" company="Zebedee Mason">
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

    public class Links
    {
        private Dependency dependency;

        public List<Dependency> Dependencies { get; }

        public List<Dependency> Interlinks { get; }

        public Links(Dependency dependency)
        {
            this.dependency = dependency;
            this.Dependencies = new List<Dependency>();
            this.Interlinks = new List<Dependency>();
        }

        public void Add(Dependency dependency)
        {
            if (!this.Dependencies.Contains(dependency))
            {
                this.Dependencies.Add(dependency);
            }
        }

        public static void Assemble(Dependency dependency, Dictionary<Dependency, Links> links)
        {
            links[dependency] = new Links(dependency);

            foreach (var child in dependency.Children)
            {
                foreach (var dep in child.Dependencies)
                {
                    links[child].Add(dep);
                }

                foreach (var dep in links[child].Dependencies)
                {
                    links[dependency].Add(dep);
                }
            }

            foreach (var child in dependency.Children)
            {
                var aChild = links[child];
                foreach (var link in aChild.Dependencies)
                {
                    var dep = link.FindChild(dependency);
                    if (dep == null || dep == child)
                    {
                        continue;
                    }

                    if (aChild.Interlinks.Contains(dep))
                    {
                        continue;
                    }

                    aChild.Interlinks.Add(dep);
                }
            }
        }
    }
}
