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

        private string filePath;

        private string fileName;

        private string sep;

        public Report(string filePath, string sep)
        {
            this.filePath = filePath;
            this.fileName = System.IO.Path.GetFileName(filePath);
            this.sep = sep;
        }

        private void Top()
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
");
        }

        private void TableTop()
        {
            this.file.Write(@"<table id=""main"">
<tr><th id=""main"">(E + P - N) / N</th><th id=""main""></th>
<th id=""main"">E + P - N</th><th id=""main""></th>
<th id=""main"">N</th><th id=""main""></th>
<th id=""main"">Externals</th><th id=""main""></th>
<th id=""main"">SLOC</th><th id=""main""></th><th id=""main""></th>
<th id=""main"">Cycle</th>
<th id=""main"">Section</th></tr>
<tr><th id=""main"">Value</th><th id=""main"">Sum</th>
<th id=""main"">Value</th><th id=""main"">Sum</th>
<th id=""main"">Value</th><th id=""main"">Sum</th>
<th id=""main"">Count</th><th id=""main"">Max</th>
<th id=""main"">Sum</th><th id=""main"">Max</th><th id=""main"">Average</th>
<th id=""main""></th>
<th id=""main""></th></tr>
");
        }

        private void TableRow(Summed row, int index, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var labels = row.StatusStrings(this.sep);
            if (labels[0] == string.Empty)
            {
                labels[0] = "Top level";
            }

            this.file.Write("<tr>");
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[5]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[6]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[3]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[4]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[1]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", labels[2]));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[row.Complexity.Branch].Merged.Count));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.ExternalDependencies[row.Complexity.Branch].MaxInTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.SLOCs[row.Complexity.Branch].SumOverTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.SLOCs[row.Complexity.Branch].MaxInTree));
            this.file.Write(string.Format("<td id=\"main\">{0}</td>", dependencies.Assembled.SLOCs[row.Complexity.Branch].Average()));
            if (dependencies.Assembled.Structures[row.Complexity.Branch].HasCycle)
            {
                this.file.Write("<td id=\"main\">Cycle</td>");
            }
            else
            {
                this.file.Write("<td id=\"main\"></td>");
            }

            this.file.Write(string.Format("<td id=\"main\"><a href=\"{0}#section{1}\">{2}</a></td>", this.fileName, index, labels[0]));
            this.file.Write("</tr>\n");
        }

        private void TableBottom()
        {
            this.file.Write(@" </table>
</div>
");
        }

        private void Section(Dependency branch, int index)
        {
            var name = branch.Path(this.sep);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            this.file.Write(string.Format("<h2><a id=\"section{0}\"></a>{1}</h2>\n", index, name));
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

                locs[child.Path(this.sep)] = child.LOC;
            }

            if (locs.Count > 0)
            {
                this.file.Write("<table>\n");
                this.file.Write(string.Format("<tr id=\"main\"><th>Dependency</th><th>SLOC</th></tr>\n"));
                foreach (var pair in locs.OrderByDescending(o => o.Value))
                {
                    this.file.Write(string.Format("<tr><td>{0}</td><td align=\"right\">{1}</td></tr>\n", pair.Key, pair.Value));
                }

                this.file.Write("</table>\n<p/>\n");
            }
        }

        private void ExternalsTable(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            if (dependencies.Assembled.ExternalDependencies[branch].Merged.Count > 0)
            {
                this.file.Write("<table>\n");
                this.file.Write(string.Format("<tr id=\"main\"><th>External dependencies</th></tr>\n"));
                foreach (var dep in dependencies.Assembled.ExternalDependencies[branch].Merged.OrderBy(o => o.Path(this.sep)))
                {
                    this.file.Write(string.Format("<tr><td>{0}</td></tr>\n", dep.Path(this.sep)));
                }

                this.file.Write("</table>\n<p/>\n");
            }
        }

        private void LinksTable(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies, Dictionary<string, int> mapping)
        {
            this.file.Write("<table>\n");
            foreach (var child in branch.Children)
            {
                foreach (var dep in dependencies.Assembled.Linkings[child].Interlinks)
                {
                    var first = child.Path(this.sep);
                    if (mapping.Keys.Contains(first))
                    {
                        first = string.Format("<a href=\"{0}#section{1}\">{2}</a>", this.fileName, mapping[first], first);
                    }

                    var second = dep.Path(this.sep);
                    if (mapping.Keys.Contains(second))
                    {
                        second = string.Format("<a href=\"{0}#section{1}\">{2}</a>", this.fileName, mapping[second], second);
                    }

                    this.file.Write(string.Format("<tr id=\"main\"><th>{0}</th><th>&rarr;</th><th>{1}</th></tr>\n", first, second));
                    var found = FindLinks.Get(child, dep, this.sep);
                    if (found.Count == 0)
                    {
                        continue;
                    }

                    foreach (var link in FindLinks.Get(child, dep, this.sep))
                    {
                        this.file.Write(string.Format("<tr><td>{0}</td><td>&rarr;</td><td>{1}</td></tr>\n", link.Key, link.Value));
                    }
                }
            }

            this.file.Write("</table>\n");
        }

        private void Matrix(Dependency branch, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            var list = dependencies.Assembled.Structures[branch].Extract();

            if (list.Count == 0)
            {
                return;
            }

            this.file.Write("<p/>\n<table>\n");
            foreach (var item in list)
            {
                this.file.Write(string.Format("<tr>"));
                this.file.Write(string.Format("<th>{0}</th>\n", item.Key));
                foreach (var c in item.Value)
                {
                    this.file.Write(string.Format("<td style=\"width: 6px\">{0}</td>\n", c));
                }

                this.file.Write(string.Format("</tr>\n"));
            }

            this.file.Write("</table>\n");
        }

        private void Bottom()
        {
            this.file.Write(@"</body>
</html>
");
        }

        public void Write(DeepEnds.Core.Linked.Dependencies dependencies)
        {
            this.file = new System.IO.StreamWriter(this.filePath);

            this.Top();

            var rows = Complexities.Factory(dependencies.Root, dependencies.Assembled.Linkings);

            var mapping = new Dictionary<string, int>();
            for (int i = 0; i < rows.Count; ++i)
            {
                mapping[rows[i].Complexity.Branch.Path(this.sep)] = i;
            }

            this.TableTop();
            for (int i = 0; i < rows.Count; ++i)
            {
                this.TableRow(rows[i], i, dependencies);
            }

            this.TableBottom();

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Complexity.Branch;

                this.Section(branch, i);

                this.DependencyTable(branch);

                this.ExternalsTable(branch, dependencies);

                this.LinksTable(branch, dependencies, mapping);

                this.Matrix(branch, dependencies);
            }

            this.Bottom();

            this.file.Close();
        }
    }
}
