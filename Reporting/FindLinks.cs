//------------------------------------------------------------------------------
// <copyright file="FindLinks.cs" company="Zebedee Mason">
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
    using DeepEnds.Core.Dependent;
    using System.Collections.Generic;

    internal class FindLinks : DependencyWalker
    {
        private List<KeyValuePair<string, string>> Links { get; }

        private string linkTo;

        private string sep;

        internal FindLinks(Dependency linkTo, string sep)
        {
            this.Links = new List<KeyValuePair<string, string>>();
            this.linkTo = linkTo.Path(sep);
            this.sep = sep;
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);

            var name = string.Empty;
            if (dependency.Dependencies.Count > 0)
            {
                name = dependency.Path(this.sep);
            }

            foreach (var dep in dependency.Dependencies)
            {
                var depname = dep.Path(this.sep);
                if (depname.Length < this.linkTo.Length)
                {
                    continue;
                }

                if (depname == this.linkTo || (depname.Substring(0, this.linkTo.Length) == this.linkTo && depname.Length > this.linkTo.Length + this.sep.Length && depname.Substring(this.linkTo.Length, this.sep.Length) == this.sep))
                {
                    this.Links.Add(new KeyValuePair<string, string>(name, depname));
                }
            }
        }

        public static List<KeyValuePair<string, string>> Get(Dependency linkFrom, Dependency linkTo, string sep)
        {
            var walker = new FindLinks(linkTo, sep);
            walker.Visit(linkFrom);
            return walker.Links;
        }
    }
}
