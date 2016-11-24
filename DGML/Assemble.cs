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

namespace DeepEnds.DGML
{
    using Core;
    using DeepEnds.Core.Dependent;
    using Microsoft.VisualStudio.GraphModel;
    using System.Collections.Generic;

    public class Assemble : DependencyWalker
    {
        private Graph graph;
        private Dictionary<Dependency, GraphNode> nodes;
        private GraphProperty size;
        private GraphProperty group;
        private GraphProperty reference;
        private int index = 0;

        private Dictionary<Dependency, Links> links;
        private Sources sources;
        private bool descend;

        private Dictionary<string, string> options;

        private string sourcePath;

        internal Assemble(Dictionary<string, string> options, Dictionary<Dependency, Links> links, Sources sources, bool descend)
        {
            this.options = options;
            this.links = links;
            this.sources = sources;
            this.descend = descend;
            this.graph = new Graph();
            this.nodes = new Dictionary<Dependency, GraphNode>();

            // set properties
            GraphPropertyCollection properties = this.graph.DocumentSchema.Properties;
            this.size = properties.AddNewProperty("Size", typeof(string));
            this.group = properties.AddNewProperty("Group", typeof(string));
            this.reference = properties.AddNewProperty("Reference", typeof(string));

            this.sourcePath = options["source"];
            var length = this.sourcePath.Length;
            if (length > 0 && this.sourcePath[length - 1] == System.IO.Path.DirectorySeparatorChar)
            {
                this.sourcePath = this.sourcePath.Substring(0, length - 2);
            }
        }

        public GraphNode Node(Dependency dependency, string grouped)
        {
            var node = this.graph.Nodes.GetOrCreate(this.index.ToString());
            this.index++;
            node.Label = dependency.Name;
            node[this.size] = "10";
            if (grouped != string.Empty)
            {
                node[this.group] = grouped;
            }

            if (this.sources != null)
            {
                var path = this.sources.AssociatedFilePath(dependency);
                if (path != string.Empty)
                {
                    if (this.sourcePath != string.Empty)
                    {
                        path = path.Replace(this.sourcePath, "$(Source)");
                    }

                    node[this.reference] = path;
                }
            }

            this.nodes[dependency] = node;
            return node;
        }

        public void Layer(Dependency dependency)
        {
            var parent = this.nodes[dependency];
            foreach (var child in dependency.Children)
            {
                var grouped = string.Empty;
                if (this.descend && child.Children.Count > 0)
                {
                    grouped = "Collapsed";
                }

                var node = this.Node(child, grouped);
                if (parent != null)
                {
                    this.graph.Links.GetOrCreate(parent, node, null, GraphCommonSchema.Contains);
                }
            }

            foreach (var child in dependency.Children)
            {
                var nodeA = this.nodes[child];
                foreach (var dep in this.links[child].Interlinks)
                {
                    var nodeB = this.nodes[dep];
                    this.graph.Links.GetOrCreate(nodeA, nodeB);
                }
            }
        }

        public override void Visit(Dependency dependency)
        {
            this.Layer(dependency);
            base.Visit(dependency);
        }

        public void Save()
        {
            var path = System.IO.Path.GetTempPath();
            var filePath = System.IO.Path.Combine(path, "deepends.dgml");
            this.graph.Save(filePath);

            bool found = false;
            var reader = new System.IO.StreamReader(filePath);
            var writer = new System.IO.StreamWriter(this.options["graph"]);
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLine();
                if (!found)
                {
                    if (line.Contains("<Nodes>"))
                    {
                        found = true;
                        writer.WriteLine("  <Paths>");
                        writer.WriteLine(string.Format("    <Path Id=\"Source\" Value=\"{0}\"/>", this.sourcePath));
                        writer.WriteLine("  </Paths>");
                    }
                }

                writer.WriteLine(line);
            }

            writer.Close();
            reader.Close();
        }

        public static Assemble Factory(Dictionary<string, string> options, Dependency root, Dictionary<Dependency, Links> links, Sources sources, bool descend)
        {
            var assemble = new Assemble(options, links, sources, descend);
            assemble.Node(root, "Expanded");
            if (descend)
            {
                assemble.Visit(root);
            }
            else
            {
                assemble.Layer(root);
            }

            return assemble;
        }
    }
}
