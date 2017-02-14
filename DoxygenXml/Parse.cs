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

        private Dictionary<string, Core.Dependency> lookup;

        private Dictionary<Core.Dependency, List<string>> links;

        private List<string> compoundTypes;

        private List<string> memberTypes;

        private bool memberHide;

        static private List<string> SplitCSV(string option)
        {
            var list = new List<string>();
            foreach (var opt in option.Split(','))
            {
                list.Add(opt.Trim());
            }

            return list;
        }

        private void ReadCompoundDef(System.Xml.XmlElement root, System.IO.TextWriter logger)
        {
            Core.Dependency leaf = null;
            var nodes = root.SelectNodes("compoundname");
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var path = element.InnerText;
                leaf = this.parser.Dependencies.GetPath(path, "::");
            }

            this.ReadLoc(root, leaf);
            this.ReadReferences(root, leaf, logger);
            this.ReadMembers(root, leaf);
        }

        private void ReadMembers(System.Xml.XmlElement root, Core.Dependency leaf)
        {
            var list = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "memberdef", list);
            foreach (var element in list)
            {
                var kind = element.GetAttribute("kind");
                var id = element.GetAttribute("id");
                if (this.memberTypes.Contains(kind))
                {
                    var nodes = element.SelectNodes("name");
                    foreach (var node in nodes)
                    {
                        if (node.GetType() != typeof(System.Xml.XmlElement))
                        {
                            continue;
                        }

                        var name = node as System.Xml.XmlElement;
                        if (this.memberHide)
                        {
                            this.lookup[id] = leaf;
                            continue;
                        }

                        var child = new Core.Dependency(name.InnerText, leaf);
                        leaf.AddChild(child);
                        this.lookup[id] = child;
                        this.ReadLoc(element, child);
                        if (leaf.LOC > child.LOC)
                        {
                            leaf.LOC -= child.LOC;
                        }
                    }
                }
            }
        }

        private void ReadLoc(System.Xml.XmlElement root, Core.Dependency leaf)
        {
            int loc = 0;
            var fileName = string.Empty;
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
                fileName = element.GetAttribute("bodyfile");
                if (bodyend == "-1" || bodyend.Length == 0 || bodystart.Length == 0)
                {
                    continue;
                }

                loc += 1 + System.Convert.ToInt32(bodyend) - System.Convert.ToInt32(bodystart);
            }

            leaf.LOC = loc;

            fileName = "$(Source)/" + fileName;
            this.parser.Sources.Create(leaf, new DeepEnds.Core.SourceProvider(fileName));
        }

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

        private void ReadReferences(System.Xml.XmlElement root, Core.Dependency leaf, System.IO.TextWriter logger)
        {
            var id = root.GetAttribute("id");
            this.lookup[id] = leaf;

            var list = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "basecompoundref", list);
            this.SelectNodes(root, "ref", list);
            this.SelectNodes(root, "references", list);

            var links = new List<string>();
            this.links[leaf] = links;
            foreach (var element in list)
            {
                if (!element.HasAttribute("refid"))
                {
                    logger.Write("! No refid for ");
                    logger.Write(element.InnerText);
                    logger.Write(" whilst processing ");
                    logger.WriteLine(leaf.Path("."));
                    continue;
                }

                var refid = element.GetAttribute("refid");
                links.Add(refid);
            }
        }

        private void ReadFile(string fileName, System.IO.TextWriter logger)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            var root = doc.DocumentElement;

            // Ensure that spurious dependencies can't form from the documentation
            var descriptions = new List<System.Xml.XmlElement>();
            this.SelectNodes(root, "briefdescription", descriptions);
            this.SelectNodes(root, "detaileddescription", descriptions);
            this.SelectNodes(root, "inbodydescription", descriptions);
            foreach (var element in descriptions)
            {
                element.ParentNode.RemoveChild(element);
            }

            var nodes = root.SelectNodes("compounddef");
            var hasRead = false;
            foreach (var node in nodes)
            {
                if (node.GetType() != typeof(System.Xml.XmlElement))
                {
                    continue;
                }

                var element = node as System.Xml.XmlElement;
                var kind = element.GetAttribute("kind");
                if (!this.compoundTypes.Contains(kind))
                {
                    continue;
                }

                if (!hasRead)
                {
                    logger.Write("  Reading ");
                    logger.WriteLine(fileName);
                    hasRead = true;
                }

                this.ReadCompoundDef(element, logger);
            }
        }

        public Parse(DeepEnds.Core.Parser parser, Dictionary<string, string> options)
        {
            this.parser = parser;
            this.compoundTypes = Parse.SplitCSV(options["compoundtype"]);
            this.memberTypes = Parse.SplitCSV(options["membertype"]);
            this.lookup = new Dictionary<string, Core.Dependency>();
            this.links = new Dictionary<Core.Dependency, List<string>>();
            this.memberHide = false;
            if (options["memberhide"] != "false")
            {
                this.memberHide = true;
            }
        }

        public void Read(string directory, System.IO.TextWriter logger)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                logger.Write("Cannot find directory ");
                logger.WriteLine(directory);
                return;
            }

            var files = System.IO.Directory.GetFiles(directory);
            foreach (var fileName in files)
            {
                var ext = System.IO.Path.GetExtension(fileName);
                if (ext != ".xml")
                {
                    continue;
                }

                this.ReadFile(fileName, logger);
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
