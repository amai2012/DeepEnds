//------------------------------------------------------------------------------
// <copyright file="Sources.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public class Sources
    {
        private Dictionary<Dependency, SourceProvider> sources;

        public Sources()
            : base()
        {
            this.sources = new Dictionary<Dependency, SourceProvider>();
        }

        public void Create(Dependency node, SourceProvider source)
        {
            if (source != null)
            {
                this.sources[node] = source;
            }
        }

        public string AssociatedFilePath(Dependency node)
        {
            if (this.sources.Keys.Contains(node))
            {
                return this.sources[node].FilePath;
            }

            return string.Empty;
        }
    }
}
