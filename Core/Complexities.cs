//------------------------------------------------------------------------------
// <copyright file="Complexities.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core.Complex
{
    using Dependent;
    using System.Collections.Generic;
    using System.Linq;

    public class Complexities : DependencyWalker
    {
        public List<Summed> List { get; }

        private Dictionary<Dependency, Links> links;

        private Dictionary<Dependency, Summed> summed;

        public Complexities(Dictionary<Dependency, Links> links)
        {
            this.List = new List<Summed>();
            this.links = links;
            this.summed = new Dictionary<Dependency, Summed>();
        }

        public override void Visit(Dependency dependency)
        {
            base.Visit(dependency);
            if (dependency.Children.Count != 0)
            {
                var item = new Summed(Complexities.Complexity(dependency, this.links));
                this.List.Add(item);
                this.summed[dependency] = item;
                foreach (var child in dependency.Children)
                {
                    item.SumN += this.summed[child].SumN;
                    item.SumEPN += this.summed[child].SumEPN;
                    item.SumEPNN += this.summed[child].SumEPNN;
                }
            }
            else
            {
                this.summed[dependency] = new Summed(null);
            }
        }

        public static List<Summed> Factory(Dependency root, Dictionary<Dependency, Links> links)
        {
            var complexities = new Complexities(links);
            complexities.Visit(root);
            return complexities.List.OrderByDescending(o => o.Complexity.EPNN).ToList();
        }

        public static Complexity Complexity(Dependency branch, Dictionary<Dependency, Links> links)
        {
            int edges = 0;
            foreach (var node in branch.Children)
            {
                edges += links[node].Interlinks.Count;
            }

            var mapping = new Dictionary<Dependency, int>();
            int label = 0;

            List<int> parts;
            foreach (var node in branch.Children)
            {
                parts = new List<int>();
                if (mapping.Keys.Contains(node))
                {
                    parts.Add(mapping[node]);
                }

                foreach (var dep in links[node].Interlinks)
                {
                    if (mapping.Keys.Contains(dep) && !parts.Contains(mapping[dep]))
                    {
                        parts.Add(mapping[dep]);
                    }

                    mapping[dep] = label;
                }

                var keys = new List<Dependency>(mapping.Keys);
                foreach (var dep in keys)
                {
                    if (parts.Contains(mapping[dep]))
                    {
                        mapping[dep] = label;
                    }
                }

                mapping[node] = label;
                label += 1;
            }

            parts = new List<int>();
            foreach (var dep in mapping.Keys)
            {
                if (!parts.Contains(mapping[dep]))
                {
                    parts.Add(mapping[dep]);
                }
            }

            int n = branch.Children.Count;
            int epn = edges + parts.Count - n;
            return new Complexity(n, epn, branch);
        }

        public static void Report(string filePath, string sep, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var file = new System.IO.StreamWriter(filePath);
            file.Write(@"<!DOCTYPE html>
<html>
<head>
<title>Summary of graph complexity</title>
<style>
table, tr#main, th#main, td#main {
    border: 1px solid black;
    border-collapse: collapse;
}
th {
    font-weight: normal;
    background-color: LightGray;
	text-align: left;
}
</style>
</head>
<body>
<h1>Summary of graph complexity</h1>
<div><p>The larger the value of (E + P) / N then the more complex the directed graph is, where</p>
<ul>
<li>E: Number of edges</li>
<li>P: Number of parts</li>
<li>N: Number of nodes</li>
</ul>
<p>The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0.</p>
<p>The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively.</p>
<p>Externals refers to the number of dependencies which aren't childen.
The following Max refers to the maximum of that value and the value at any child nodes.</p>
<table id=""main"">
<tr><th id=""main"">(E + P - N) / N</th><th id=""main"">Sum</th>
<th id=""main"">E + P - N</th><th id=""main"">Sum</th>
<th id=""main"">N</th><th id=""main"">Sum</th>
<th id=""main"">Externals</th><th id=""main"">Max</th>
<th id=""main"">Section</th></tr>
");
            var mapping = new Dictionary<string, int>();

            var rows = Complexities.Factory(dependencies.Root, dependencies.Links);
            for (int i = 0; i < rows.Count; ++i)
            {
                var labels = rows[i].StatusStrings(sep);
                if (labels[0] == string.Empty)
                {
                    labels[0] = "Top level";
                }

                file.Write("<tr>");
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[5]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[6]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[3]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[4]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[1]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", labels[2]));
                file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[rows[i].Complexity.Branch].Merged.Count));
                file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[rows[i].Complexity.Branch].MaxInTree));
                file.Write(string.Format("<td id=\"main\"><a href=\"{0}#section{1}\">{2}</a></td>", fileName, i, labels[0]));
                file.Write("</tr>\n");
                mapping[labels[0]] = i;
            }

            file.Write(@" </table>
</div>
");

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Complexity.Branch;
                var name = branch.Path(sep);
                if (name == string.Empty)
                {
                    name = "Top level";
                }

                file.Write(string.Format("<h2><a id=\"section{0}\"></a>{1}</h2>\n", i, name));

                if (dependencies.Assembled.ExternalDependencies[branch].Merged.Count > 0)
                {
                    file.Write("<table>\n");
                    file.Write(string.Format("<tr id=\"main\"><th>External dependencies</th></tr>\n"));
                    foreach (var dep in dependencies.Assembled.ExternalDependencies[branch].Merged.OrderBy(o => o.Path(sep)))
                    {
                        file.Write(string.Format("<tr><td>{0}</td></tr>\n", dep.Path(sep)));
                    }

                    file.Write("</table>\n<p/>\n");
                }

                file.Write("<table>\n");
                foreach (var child in branch.Children)
                {
                    foreach (var dep in dependencies.Links[child].Interlinks)
                    {
                        var first = child.Path(sep);
                        if (mapping.Keys.Contains(first))
                        {
                            first = string.Format("<a href=\"{0}#section{1}\">{2}</a>", fileName, mapping[first], first);
                        }

                        var second = dep.Path(sep);
                        if (mapping.Keys.Contains(second))
                        {
                            second = string.Format("<a href=\"{0}#section{1}\">{2}</a>", fileName, mapping[second], second);
                        }

                        file.Write(string.Format("<tr id=\"main\"><th>{0}</th><th>&rarr;</th><th>{1}</th></tr>\n", first, second));
                        var found = FindLinks.Get(child, dep, sep);
                        if (found.Count == 0)
                        {
                            continue;
                        }

                        foreach (var link in FindLinks.Get(child, dep, sep))
                        {
                            file.Write(string.Format("<tr><td>{0}</td><td>&rarr;</td><td>{1}</td></tr>\n", link.Key, link.Value));
                        }
                    }
                }

                file.Write("</table>\n");
            }

            file.Write(@"</body>
</html>
");
            file.Close();
        }
    }
}
