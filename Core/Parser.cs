//------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Zebedee Mason">
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

using System.Collections.Generic;

namespace DeepEnds.Core
{
    public class Parser
    {
        public DeepEnds.Core.Linked.Dependencies Dependencies { get; }

        public DeepEnds.Core.Sources Sources { get; }

        private Dictionary<DeepEnds.Core.Dependent.Dependency, HashSet<string>> sets;

        private Dictionary<string, DeepEnds.Core.Dependent.Dependency> leaves;

        public Parser(DeepEnds.Core.Linked.Dependencies dependencies, DeepEnds.Core.Sources sources)
        {
            this.Dependencies = dependencies;
            this.Sources = sources;
            this.sets = new Dictionary<Dependent.Dependency, HashSet<string>>();
            this.leaves = new Dictionary<string, Dependent.Dependency>();
        }

        public DeepEnds.Core.Dependent.Dependency Create(string name, string fullName, DeepEnds.Core.Dependent.Dependency parent)
        {
            var leaf = this.Dependencies.Create(name, parent);
            parent.AddChild(leaf);
            this.leaves[fullName] = leaf;
            return leaf;
        }

        public void AddDependency(DeepEnds.Core.Dependent.Dependency leaf, string dependency)
        {
            if (!this.sets.ContainsKey(leaf))
            {
                this.sets[leaf] = new HashSet<string>();
            }

            this.sets[leaf].Add(dependency);
        }

        public void Finalise(string sep)
        {
            foreach (var leaf in this.sets.Keys)
            {
                var path = leaf.Path(sep);
                foreach (var item in this.sets[leaf])
                {
                    if (item == path)
                    {
                        continue;
                    }

                    DeepEnds.Core.Dependent.Dependency dep = null;
                    if (this.leaves.ContainsKey(item))
                    {
                        dep = this.leaves[item];
                    }

                    leaf.AddDependency(item, dep);
                }
            }
        }
    }
}
