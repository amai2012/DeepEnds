//------------------------------------------------------------------------------
// <copyright file="Dependency.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core.Dependent
{
    using System.Collections.Generic;

    public class Dependency
    {
        public string Name { get; }

        public Dependency Parent { get; set; }

        public System.Collections.Generic.List<Dependency> Children { get; }

        public System.Collections.Generic.List<Dependency> Dependencies { get; }

        public int LOC { get; set; }

        private List<string> missing;

        public Dependency(string name, Dependency parent)
        {
            this.Name = name;
            this.Parent = parent;
            this.Children = new System.Collections.Generic.List<Dependency>();
            this.Dependencies = new List<Dependency>();
            this.LOC = 0;
            this.missing = new List<string>();
        }

        public void AddChild(Dependency child)
        {
            this.Children.Add(child);
        }

        public Dependency[] Matches(string[] selected)
        {
            var branches = new List<Dependency>();
            foreach (var child in this.Children)
            {
                foreach (var name in selected)
                {
                    if (child.Name != name)
                    {
                        continue;
                    }

                    branches.Add(child);
                }
            }

            return branches.ToArray();
        }

        public Dependency Find(string path, string sep)
        {
            var name = path;
            if (path.Contains(sep))
            {
                var i = path.IndexOf(sep);
                name = path.Substring(0, i);
                path = path.Substring(i + sep.Length);
            }
            else
            {
                path = string.Empty;
            }

            foreach (var child in this.Children)
            {
                if (child.Name == name)
                {
                    if (path.Length == 0)
                    {
                        return child;
                    }

                    return child.Find(path, sep);
                }
            }

            return null;
        }

        public void AddDependency(string fullName, Dependency dep)
        {
            if (dep == null)
            {
                if (!this.missing.Contains(fullName))
                {
                    this.missing.Add(fullName);
                }
            }
            else
            {
                if (!this.Dependencies.Contains(dep))
                {
                    this.Dependencies.Add(dep);
                }
            }
        }

        public string Path(string sep)
        {
            if (this.Parent == null)
            {
                return string.Empty;
            }

            var path = this.Parent.Path(sep);
            if (path.Length == 0)
            {
                return this.Name;
            }

            return path + sep + this.Name;
        }

        public Dependency FindChild(Dependency parent)
        {
            if (this.Parent == null)
            {
                return null;
            }

            if (this.Parent == parent)
            {
                return this;
            }

            return this.Parent.FindChild(parent);
        }

        public void Branches(List<Dependency> branches)
        {
            if (this.Children.Count > 0)
            {
                branches.Add(this);
            }

            foreach (var dep in this.Children)
            {
                dep.Branches(branches);
            }
        }
    }
}
