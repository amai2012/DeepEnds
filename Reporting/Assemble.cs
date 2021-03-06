﻿//------------------------------------------------------------------------------
// <copyright file="Assemble.cs" company="Zebedee Mason">
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

    public class Assemble : DependencyWalker
    {
        public Dictionary<Dependency, Links> Linkings { get; }

        public Dictionary<Dependency, Externals> ExternalDependencies { get; }

        public Dictionary<Dependency, SLOC> SLOCs { get; }

        public Dictionary<Dependency, Structure> Structures { get; }

        public Dictionary<Dependency, int[]> Usages { get; }

        public Dictionary<Dependency, List<Dependency>> Interfaces { get; }

        public Assemble()
        {
            this.Linkings = new Dictionary<Dependency, Links>();
            this.ExternalDependencies = new Dictionary<Dependency, Externals>();
            this.SLOCs = new Dictionary<Dependency, SLOC>();
            this.Structures = new Dictionary<Dependency, Structure>();
            this.Usages = new Dictionary<Dependency, int[]>();
            this.Interfaces = new Dictionary<Dependency, List<Dependency>>();
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);

            Externals.Assemble(dependency, this.ExternalDependencies);

            Links.Assemble(dependency, this.Linkings);

            SLOC.Assemble(dependency, this.SLOCs);

            Structure.Assemble(dependency, this.Linkings, this.Structures);
        }

        public void Usage(Dependency root)
        {
            var usages = new CalculateUsage(root, this.Usages, this.Interfaces);
            usages.Calculate(root);
        }
    }
}
