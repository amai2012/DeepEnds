//------------------------------------------------------------------------------
// <copyright file="Parse.cs" company="Zebedee Mason">
//     Copyright (c) 2017 Zebedee Mason.
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

namespace DeepEnds.DGML
{
    using System.Collections.Generic;

    public class Parse
    {
        private DeepEnds.Core.Parser parser;

        private Dictionary<string, string> paths;

        private Dictionary<string, Core.Dependent.Dependency> lookup;

        private Dictionary<Core.Dependent.Dependency, List<string>> links;

        private Dictionary<string, string> options;

        private void SelectNodes(System.Xml.XmlElement root, string name, List<System.Xml.XmlElement> list)
        {
            foreach (var node in root.ChildNodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                if (element.Name == name)
                {
                    list.Add(element);
                }
                else
                {
                    this.SelectNodes(element, name, list);
                }
            }
        }

        private void ReadPaths(System.Xml.XmlElement root)
        {
            var elements = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "Path", elements);
            foreach (var path in elements)
            {
                var id = string.Format("$({0})", path.GetAttribute("Id"));
                var value = path.GetAttribute("Value");

                this.paths[id] = value;
            }
        }

        private void ReadNodes(System.Xml.XmlElement root)
        {
            var elements = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "Node", elements);
            foreach (var node in elements)
            {
                DeepEnds.Core.Dependent.Dependency dep = null;
                var label = node.GetAttribute("Label");
                if (label.Length == 0)
                {
                    dep = this.parser.Dependencies.Root;
                }
                else
                {
                    dep = new DeepEnds.Core.Dependent.Dependency(label, null);
                }

                var id = node.GetAttribute("Id");
                this.lookup[id] = dep;

                string loc = string.Empty;
                if (node.HasAttribute("LOC"))
                {
                    loc = node.GetAttribute("LOC");
                    dep.LOC = System.Convert.ToInt32(loc);
                }

                string reference = string.Empty;
                if (node.HasAttribute("Reference"))
                {
                    reference = node.GetAttribute("Reference");
                    foreach (var key in this.paths.Keys)
                    {
                        reference = reference.Replace(key, this.paths[key]);
                    }

                    this.parser.Sources.Create(dep, new Core.SourceProvider(dep, reference));
                }
            }
        }

        private void ReadLinks(System.Xml.XmlElement root)
        {
            var elements = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "Link", elements);
            foreach (var link in elements)
            {
                var source = this.lookup[link.GetAttribute("Source")];
                var target = this.lookup[link.GetAttribute("Target")];

                if (link.HasAttribute("Category"))
                {
                    if (link.GetAttribute("Category") == "Contains")
                    {
                        source.AddChild(target);
                        target.Parent = source;
                        continue;
                    }
                }

                source.AddDependency(string.Empty, target);
            }
        }

        private void ReadFile(string fileName, System.IO.TextWriter logger)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            var root = doc.DocumentElement;

            this.ReadPaths(root);
            this.ReadNodes(root);
            this.ReadLinks(root);
        }

        public Parse(DeepEnds.Core.Parser parser, Dictionary<string, string> options)
        {
            this.parser = parser;
            this.options = options;
            this.paths = new Dictionary<string, string>();
            this.lookup = new Dictionary<string, Core.Dependent.Dependency>();
            this.links = new Dictionary<Core.Dependent.Dependency, List<string>>();
        }

        public void Read(string fileName, System.IO.TextWriter logger)
        {
            if (!System.IO.File.Exists(fileName))
            {
                logger.Write("Cannot find file ");
                logger.WriteLine(fileName);
                return;
            }

            var ext = System.IO.Path.GetExtension(fileName);
            if (ext != ".dgml")
            {
                return;
            }

            this.ReadFile(fileName, logger);
        }
    }
}
