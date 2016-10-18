//------------------------------------------------------------------------------
// <copyright file="Parse.cs" company="Zebedee Mason">
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

namespace DeepEnds.DoxygenXml
{
    using System.Collections.Generic;

    public class Parse
    {
        private DeepEnds.Core.Parser parser;

        private Dictionary<string, Core.Dependent.Dependency> lookup;

        private Dictionary<Core.Dependent.Dependency, List<string>> links;

        private void ReadClass(System.Xml.XmlElement root, string sep)
        {
            Core.Dependent.Dependency leaf = null;
            var nodes = root.SelectNodes("compoundname");
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var path = element.InnerText;
                leaf = this.parser.Dependencies.GetPath(path, sep);
            }

            this.ReadLoc(root, leaf);
            this.ReadReferences(root, leaf);
        }

        private void ReadLoc(System.Xml.XmlElement root, Core.Dependent.Dependency leaf)
        {
            int loc = 0;
            var nodes = root.SelectNodes("location");
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var bodystart = element.GetAttribute("bodystart");
                var bodyend = element.GetAttribute("bodyend");
                loc += 1 + System.Convert.ToInt32(bodyend) - System.Convert.ToInt32(bodystart);
            }

            leaf.LOC = loc;
        }

        private void ReadReferences(System.Xml.XmlNodeList nodes, List<string> refs)
        {
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var refid = element.GetAttribute("refid");
                refs.Add(refid);
            }
        }

        private void ReadReferences(System.Xml.XmlElement root, Core.Dependent.Dependency leaf)
        {
            var id = root.GetAttribute("id");
            this.lookup[id] = leaf;

            var links = new List<string>();
            this.links[leaf] = links;
            this.ReadReferences(root.SelectNodes("basecompoundref"), links);
            this.ReadReferences(root.SelectNodes("ref"), links);
        }

        private void ReadFile(string fileName)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            var root = doc.DocumentElement;
            var nodes = root.SelectNodes("compounddef");
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var language = element.GetAttribute("language");
                var kind = element.GetAttribute("kind");
                if (kind == "class")
                {
                    var sep = ".";
                    if (language == "C++")
                    {
                        sep = "::";
                    }

                    this.ReadClass(element, sep);
                }
            }
        }

        public Parse(DeepEnds.Core.Parser parser)
        {
            this.parser = parser;
            this.lookup = new Dictionary<string, Core.Dependent.Dependency>();
            this.links = new Dictionary<Core.Dependent.Dependency, List<string>>();
        }

        public void Read(string directory, System.Text.StringBuilder messages)
        {
            var files = System.IO.Directory.GetFiles(directory);
            foreach (var fileName in files)
            {
                var ext = System.IO.Path.GetExtension(fileName);
                if (ext != ".xml")
                {
                    continue;
                }

                this.ReadFile(fileName);
            }
        }

        public void Finalise()
        {
            foreach (var pair in this.links)
            {
                var leaf = pair.Key;
                foreach (var link in pair.Value)
                {
                    if (this.lookup.ContainsKey(link))
                    {
                        leaf.AddDependency(string.Empty, this.lookup[link]);
                    }
                }
            }
        }
    }
}
