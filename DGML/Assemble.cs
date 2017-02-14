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
    using DeepEnds.Core;
    using Microsoft.VisualStudio.GraphModel;
    using System.Collections.Generic;

    public class Assemble : DependencyWalker
    {
        private Graph graph;
        private Dictionary<Dependency, GraphNode> nodes;
        private GraphProperty size;
        private GraphProperty group;
        private GraphProperty reference;
        private GraphProperty loc;
        private int index = 0;

        private Sources sources;

        private Dictionary<string, string> options;

        private string sourcePath;

        internal Assemble(Dictionary<string, string> options, Sources sources)
        {
            this.options = options;
            this.sources = sources;
            this.graph = new Graph();
            this.nodes = new Dictionary<Dependency, GraphNode>();

            // set properties
            GraphPropertyCollection properties = this.graph.DocumentSchema.Properties;
            this.size = properties.AddNewProperty("Size", typeof(string));
            this.group = properties.AddNewProperty("Group", typeof(string));
            this.reference = properties.AddNewProperty("Reference", typeof(string));
            this.loc = properties.AddNewProperty("LOC", typeof(int));

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
            if (grouped.Length > 0)
            {
                node[this.group] = grouped;
            }

            node[this.loc] = dependency.LOC;

            if (this.sources != null)
            {
                var path = this.sources.AssociatedFilePath(dependency);
                if (path.Length > 0)
                {
                    if (this.sourcePath.Length > 0)
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
                if (child.Children.Count > 0)
                {
                    grouped = "Collapsed";
                }

                var node = this.Node(child, grouped);
                if (parent != null)
                {
                    this.graph.Links.GetOrCreate(parent, node, null, GraphCommonSchema.Contains);
                }
            }
        }

        public override void Visit(Dependency dependency)
        {
            this.Layer(dependency);
            base.Visit(dependency);
        }

        public void Raw()
        {
            foreach (var pair in this.nodes)
            {
                var leaf = pair.Key;
                var nodeA = pair.Value;
                foreach (var dep in leaf.Dependencies)
                {
                    if (dep == leaf)
                    {
                        continue;
                    }

                    var nodeB = this.nodes[dep];
                    this.graph.Links.GetOrCreate(nodeA, nodeB);
                }
            }
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

        public static Assemble Factory(Dictionary<string, string> options, Dependency root, Sources sources)
        {
            var assemble = new Assemble(options, sources);
            assemble.Node(root, "Expanded");
            assemble.Visit(root);
            assemble.Raw();

            return assemble;
        }
    }
}
