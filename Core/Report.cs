//------------------------------------------------------------------------------
// <copyright file="Report.cs" company="Zebedee Mason">
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

namespace DeepEnds.Core
{
    using DeepEnds.Core.Complex;
    using Dependent;

    using System.Collections.Generic;
    using System.Linq;

    public class Report
    {
        private System.IO.StreamWriter file;

        private Dictionary<string, string> options;

        private string fileName;

        public Report(Dictionary<string, string> options)
        {
            this.options = options;
            this.fileName = System.IO.Path.GetFileName(options["report"]);
        }

        private void Top(int topIndex)
        {
            this.file.Write(@"<!DOCTYPE html>
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
td#alert {
    border: 1px solid black;
    border-collapse: collapse;
    color:white;
    background-color:red;
}
</style>
</head>
<body>
<h1>Summary of graph complexity</h1>
<h3>Cyclomatic Number</h3>
<div>
<p>The larger the value of (E + P) / N then the more complex the directed graph is, where</p>
<ul>
<li>E: Number of edges</li>
<li>P: Number of parts</li>
<li>N: Number of nodes</li>
</ul>
<p>The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0.</p>
<p>The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively.</p>
</div>

<h3>Externals</h3>
<div>
<p>Externals refers to the number of dependencies which aren't childen.
The following max refers to the maximum of that value and the value at any child nodes.</p>
</div>

<h3>SLOC</h3>
<div>
<p>SLOC stands for source lines of code; whilst reading the source an attempt has been made not to count blank or
comment lines. The sum is sum over the child nodes recursively down to the leaf nodes. An attempt has been made to fit 
the log-normal distribution to data which has then been used to calculate 
the expected value, this is expected to only make sense high up in the hierarchy. 
The following max refers to the maximum of that value and the value at any child nodes as long as there is more than
one item to fit so as to avoid domination of the statistic by boilerplate. These two values are bracketed by the lower 
and upper limits of the 90% confidence interval is the upper limit is less than the maximal value of SLOC on a leaf,
otherwise it is left blank.</p>
</div>

<h3>Cycle</h3>
<div>
<p>If a cycle in the graph (circular dependency) occurs then the word cycle will appear as the value otherwise it 
is left blank.</p>
</div>

<h3>Section</h3>
<div>
<p>The node label of the graph (with the obvious exception of ""Top level"") with a hyperlink to it.</p>
</div>

<h3>Table</h3>
");
            this.file.Write(string.Format("<p>Skip to <a href=\"{0}#section{1}\">Top level</a><p/>\n", this.fileName, topIndex));
            this.file.Write(@"
<p>The following table is sorted on its first column, subsequent instances of this table type are sorted on 
the final column. Hover over the table headers to make tool tips appear.</p>");
        }

        private void TableTop()
        {
            this.file.Write(@"<table id=""main"">
<thead>
<tr>
<th colspan=""3"" id=""main"" title=""Cyclomatic number normalised by the number of nodes"">(E + P - N) / N</th>
<th colspan=""3"" id=""main"" title=""Cyclomatic number"">E + P - N</th>
<th colspan=""3"" id=""main"" title=""Number of nodes"">N</th>
<th colspan=""2"" id=""main"" title=""Number of leaf nodes not contained by this node that are depended upon"">Externals</th>
<th colspan=""5"" id=""main"" title=""Source lines of code"">SLOC</th>
<th rowspan=""2"" id=""main"" title=""Whether a cycle occurs"">Cycle</th>
<th rowspan=""2"" id=""main"" title=""The label of the graph node"">Section</th>
</tr>
<tr>
<th id=""main"" title=""Value at the node"">Val</th>
<th id=""main"" title=""Maximum value of the node and child nodes recursively"">Max</th>
<th id=""main"" title=""Sum of node value and child node values recursively"">Sum</th>
<th id=""main"" title=""Value at the node"">Val</th>
<th id=""main"" title=""Maximum value of the node and child nodes recursively"">Max</th>
<th id=""main"" title=""Sum of node value and child node values recursively"">Sum</th>
<th id=""main"" title=""Value at the node"">Val</th>
<th id=""main"" title=""Maximum value of the node and child nodes recursively"">Max</th>
<th id=""main"" title=""Sum of node value and child node values recursively"">Sum</th>
<th id=""main"" title=""Value at the node"">Count</th>
<th id=""main"" title=""Maximum value of the node and child nodes recursively"">Max</th>
<th id=""main"" title=""Sum of node value and child node values recursively"">Sum</th>
<th id=""main"" title=""Lower bound of the 90% confidence interval for leaf size"">Lower</th>
<th id=""main"" title=""Expected leaf size given a log-normal distribution"">Exp</th>
<th id=""main"" title=""Maximum value of the expected leaf size at the node and child nodes recursively"">Max</th>
<th id=""main"" title=""Upper bound of the 90% confidence interval for leaf size"">Upper</th>
</tr>
</thead>
<tbody>
");
        }

        private void TableRow(Complexity row, int index, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var name = row.Branch.Path(this.options["sep"]);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            var branch = row.Branch;

            this.file.Write("<tr>");
            this.file.Write(string.Format("<td id=\"main\">{0:0.00}</td>", row.EPNNs.Value * 0.0001));
            this.file.Write(string.Format("<td id=\"main\">{0:0.00}</td>", row.EPNNs.MaxInTree * 0.0001));
            this.file.Write(string.Format("<td id=\"main\">{0:0.00}</td>", row.EPNNs.SumOverTree * 0.0001));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.EPNs.Value));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.EPNs.MaxInTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.EPNs.SumOverTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.Ns.Value));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.Ns.MaxInTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", row.Ns.SumOverTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[branch].Merged.Count));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[branch].MaxInTree));

            var sloc = dependencies.Assembled.SLOCs[branch];
            var lower = string.Empty;
            var upper = string.Empty;
            if (sloc.MaxInTree > sloc.Upper)
            {
                lower = sloc.Lower.ToString();
                upper = sloc.Upper.ToString();
            }

            this.file.Write(string.Format("<td id=\"main\">{0}</td>", sloc.SumOverTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", lower));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", sloc.Expected));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", sloc.ExpectedMax));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", upper));
            if (dependencies.Assembled.Structures[branch].HasCycle)
            {
                this.file.Write("<td id=\"alert\">Cycle</td>");
            }
            else
            {
                this.file.Write("<td id=\"main\"></td>");
            }

            this.file.Write(string.Format("<td id=\"main\"><a href=\"{0}#section{1}\">{2}</a></td>", this.fileName, index, name));
            this.file.Write("</tr>\n");
        }

        private void TableBottom()
        {
            this.file.Write(@" </tbody></table><p/>
");
        }

        private void Section(Dependency branch, int index, Dictionary<Dependency, int> mapping)
        {
            var name = branch.Path(this.options["sep"]);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            this.file.Write(string.Format("<h2><a id=\"section{0}\"></a>{1}</h2>\n", index, name));

            if (branch.Parent == null)
            {
                return;
            }

            index = mapping[branch.Parent];
            name = branch.Parent.Path(this.options["sep"]);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            this.file.Write(string.Format("Up to <a href=\"{0}#section{1}\">{2}</a><p/>", this.fileName, index, name));
        }

        private void DependencyTable(Dependency branch)
        {
            Dictionary<string, int> locs = new Dictionary<string, int>();
            foreach (var child in branch.Children)
            {
                if (child.LOC == 0)
                {
                    continue;
                }

                locs[child.Path(this.options["sep"])] = child.LOC;
            }

            if (locs.Count > 0)
            {
                this.file.Write("<table>\n<thead>\n");
                this.file.Write(string.Format("<tr id=\"main\" title=\"Leaf of this node\"><th>Dependency</th><th title=\"Number of lines of source code\">SLOC</th></tr>\n"));
                this.file.Write("</thead>\n<tbody>\n");
                foreach (var pair in locs.OrderByDescending(o => o.Value))
                {
                    this.file.Write(string.Format("<tr><td>{0}</td><td align=\"right\">{1}</td></tr>\n", pair.Key, pair.Value));
                }

                this.file.Write("</tbody>\n</table>\n<p/>\n");
            }
        }

        private void ExternalsTable(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            if (dependencies.Assembled.ExternalDependencies[branch].Merged.Count > 0)
            {
                this.file.Write("<table><thead>\n\n");
                this.file.Write(string.Format("<tr id=\"main\" title=\"Leaf nodes not contained by this node that are depended upon\"><th>External dependencies</th></tr>\n"));
                this.file.Write("</thead>\n<tbody>\n");
                foreach (var dep in dependencies.Assembled.ExternalDependencies[branch].Merged.OrderBy(o => o.Path(options["sep"])))
                {
                    this.file.Write(string.Format("<tr><td>{0}</td></tr>\n", dep.Path(this.options["sep"])));
                }

                this.file.Write("</tbody>\n</table>\n<p/>\n");
            }
        }

        private void InternalsTable(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies, Dictionary<Dependency, int> mapping)
        {
            var set = new HashSet<string>();
            foreach (var child in branch.Children)
            {
                foreach (var dep in dependencies.Assembled.Linkings[child].Interlinks)
                {
                    var found = FindLinks.Get(child, dep, this.options["sep"]);
                    if (found.Count == 0)
                    {
                        continue;
                    }

                    foreach (var link in FindLinks.Get(child, dep, this.options["sep"]))
                    {
                        set.Add(link.Value);
                    }
                }
            }

            var list = set.ToList();
            list.Sort();

            this.file.Write("<table>\n<thead>\n");
            this.file.Write("<tr id=\"main\" title=\"Dependencies that cause the edges of the graph to be formed\"><th>Internal Dependencies</th></tr>\n");
            this.file.Write("</thead>\n<tbody>\n");
            foreach (var item in list)
            {
                this.file.Write(string.Format("<tr><td>{0}</td></tr>\n", item));
            }

            this.file.Write("</tbody>\n</table>\n<p/>\n\n");
        }

        private void LinksTable(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies, Dictionary<Dependency, int> mapping)
        {
            this.file.Write("<table>\n");
            foreach (var child in branch.Children)
            {
                foreach (var dep in dependencies.Assembled.Linkings[child].Interlinks)
                {
                    var first = child.Path(this.options["sep"]);
                    if (mapping.Keys.Contains(child))
                    {
                        first = string.Format("<a href=\"{0}#section{1}\">{2}</a>", this.fileName, mapping[child], first);
                    }

                    var second = dep.Path(this.options["sep"]);
                    if (mapping.Keys.Contains(dep))
                    {
                        second = string.Format("<a href=\"{0}#section{1}\">{2}</a>", this.fileName, mapping[dep], second);
                    }

                    this.file.Write("<thead>\n");
                    this.file.Write(string.Format("<tr id=\"main\" title=\"Dependencies that cause this edge of the graph to be formed\"><th>{0}</th><th>&rarr;</th><th>{1}</th></tr>\n", first, second));
                    this.file.Write("</thead>\n<tbody>\n");
                    var found = FindLinks.Get(child, dep, this.options["sep"]);
                    if (found.Count == 0)
                    {
                        continue;
                    }

                    foreach (var link in FindLinks.Get(child, dep, options["sep"]))
                    {
                        this.file.Write(string.Format("<tr><td>{0}</td><td>&rarr;</td><td>{1}</td></tr>\n", link.Key, link.Value));
                    }

                    this.file.Write("</tbody>\n");
                }
            }

            this.file.Write("</tbody>\n</table>\n");
        }

        private void Matrix(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var list = dependencies.Assembled.Structures[branch].Extract();

            if (list.Count == 0)
            {
                return;
            }

            this.file.Write("<p/>\n<table>\n<tbody>\n");
            foreach (var item in list)
            {
                this.file.Write(string.Format("<tr style=\"height: 8px\" title=\"Node of the graph followed by dependencies (structure matrix)\">"));
                this.file.Write(string.Format("<th>{0}</th>\n", item.Key));
                foreach (var c in item.Value)
                {
                    this.file.Write(string.Format("<td style=\"width: 8px;\">{0}</td>\n", c));
                }

                this.file.Write(string.Format("</tr>\n"));
            }

            this.file.Write("</tbody>\n</table>\n");
        }

        private void Bottom()
        {
            this.file.Write(@"</body>
</html>
");
        }

        public void Write(DeepEnds.Core.Linked.Dependencies dependencies)
        {
            this.file = new System.IO.StreamWriter(this.options["report"]);

            var rows = Complexities.Factory(dependencies.Root, dependencies.Assembled.Linkings);

            var mapping = new Dictionary<Dependency, int>();
            for (int i = 0; i < rows.Count; ++i)
            {
                mapping[rows[i].Branch] = i;
            }

            this.Top(mapping[dependencies.Root]);

            this.TableTop();
            for (int i = 0; i < rows.Count; ++i)
            {
                this.TableRow(rows[i], i, dependencies);
            }

            this.TableBottom();

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Branch;

                this.Section(branch, i, mapping);

                this.TableTop();
                this.TableRow(rows[i], i, dependencies);
                foreach (var child in branch.Children.OrderBy(o => o.Name))
                {
                    if (!mapping.ContainsKey(child))
                    {
                        continue;
                    }

                    var index = mapping[child];
                    this.TableRow(rows[index], index, dependencies);
                }

                this.TableBottom();

                this.DependencyTable(branch);

                this.ExternalsTable(branch, dependencies);

                this.InternalsTable(branch, dependencies, mapping);

                this.LinksTable(branch, dependencies, mapping);

                this.Matrix(branch, dependencies);
            }

            this.Bottom();

            this.file.Close();
        }
    }
}
