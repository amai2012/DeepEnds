//------------------------------------------------------------------------------
// <copyright file="Reporter.cs" company="Zebedee Mason">
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

    public class Reporter
    {
        private System.IO.StreamWriter file;

        private Dictionary<string, string> options;

        private DeepEnds.Core.Linked.Dependencies dependencies;

        public string Destination { get; set; }
        
        public string Link { get; set; }

        public string ListBegin { get; set; }

        public string ListEnd { get; set; }

        public string ListItem { get; set; }

        public string Paragraph { get; set; }

        public string RightArrow { get; set; }

        public string SectionBegin { get; set; }

        public string SubsectionBegin { get; set; }

        public string SubsectionEnd { get; set; }

        public string TableBegin { get; set; }

        public string TableEnd { get; set; }

        public string TableBodyBegin { get; set; }

        public string TableBodyEnd { get; set; }

        public string TableBodyItem { get; set; }

        public string TableHeadBegin { get; set; }

        public string TableHeadEnd { get; set; }

        public string TableHeadItem { get; set; }

        public string TableRowBegin { get; set; }

        public string TableRowEnd { get; set; }

        public Reporter(System.IO.StreamWriter file, Dictionary<string, string> options, DeepEnds.Core.Linked.Dependencies dependencies)
        {
            this.file = file;
            this.options = options;
            this.dependencies = dependencies;

            this.Destination = string.Empty;
            this.Link = string.Empty;
            this.ListBegin = string.Empty;
            this.ListEnd = string.Empty;
            this.ListItem = string.Empty;
            this.RightArrow = "->";
            this.Paragraph = string.Empty;
            this.SectionBegin = string.Empty;
            this.SubsectionBegin = string.Empty;
            this.SubsectionEnd = string.Empty;
            this.TableBegin = string.Empty;
            this.TableEnd = string.Empty;
            this.TableBodyBegin = string.Empty;
            this.TableBodyEnd = string.Empty;
            this.TableBodyItem = string.Empty;
            this.TableHeadBegin = string.Empty;
            this.TableHeadEnd = string.Empty;
            this.TableHeadItem = string.Empty;
            this.TableRowBegin = string.Empty;
            this.TableRowEnd = string.Empty;
        }

        public void TableTopText(int topIndex)
        {
            this.file.Write(string.Format(this.SectionBegin, "Summary of graph complexity"));
            this.file.Write(string.Format(this.SubsectionBegin, "Cyclomatic Number"));
            this.file.Write(string.Format(this.Paragraph, "The larger the value of (E + P) / N then the more complex the directed graph is, where"));
            this.file.Write(this.ListBegin);
            this.file.Write(string.Format(this.ListItem, "E: Number of edges"));
            this.file.Write(string.Format(this.ListItem, "P: Number of parts"));
            this.file.Write(string.Format(this.ListItem, "N: Number of nodes"));
            this.file.Write(this.ListEnd);
            this.file.Write(string.Format(this.Paragraph, "The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0."));
            this.file.Write(string.Format(this.Paragraph, "The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively."));
            this.file.Write(this.SubsectionEnd);
            this.file.Write(string.Format(this.SubsectionBegin, "Externals"));
            this.file.Write(string.Format(this.Paragraph, @"Externals refers to the number of dependencies which aren't children.
The following max refers to the maximum of that value and the value at any child nodes."));
            this.file.Write(this.SubsectionEnd);
            this.file.Write(string.Format(this.SubsectionBegin, "SLOC"));
            this.file.Write(string.Format(this.Paragraph, @"SLOC stands for source lines of code; whilst reading the source an attempt has been made not to count blank or
comment lines. The sum is sum over the child nodes recursively down to the leaf nodes. An attempt has been made to fit 
the log-normal distribution to data which has then been used to calculate 
the expected value, this is expected to only make sense high up in the hierarchy. 
The following max refers to the maximum of that value and the value at any child nodes as long as there is more than
one item to fit so as to avoid domination of the statistic by boilerplate. These two values are bracketed by the lower 
and upper limits of the 90% confidence interval is the upper limit is less than the maximal value of SLOC on a leaf,
otherwise it is left blank."));
            this.file.Write(this.SubsectionEnd);
            this.file.Write(string.Format(this.SubsectionBegin, "Cycle"));
            this.file.Write(string.Format(this.Paragraph, @"If a cycle in the graph (circular dependency) occurs then the word cycle will appear as the value otherwise it 
is left blank."));
            this.file.Write(this.SubsectionEnd);
            this.file.Write(string.Format(this.SubsectionBegin, "Section"));
            this.file.Write(string.Format(this.Paragraph, "The node label of the graph (with the obvious exception of \"Top level\") with a hyperlink to it."));
            this.file.Write(this.SubsectionEnd);
            this.file.Write(string.Format(this.SubsectionBegin, "Table"));
            this.file.Write(string.Format(this.Paragraph, string.Format("Skip to {0}", string.Format(this.Link, topIndex, "Top level"))));
            this.file.Write(string.Format(this.Paragraph, @"The following table is sorted on its first column, subsequent instances of this table type are sorted on 
the final column. Hover over the table headers to make tool tips appear."));
        }

        private void TableTop()
        {
            this.file.Write(string.Format(this.TableBegin, " id=\"main\""));
            this.file.Write(this.TableHeadBegin);
            this.file.Write(string.Format(this.TableRowBegin, string.Empty));
            this.file.Write(string.Format(this.TableHeadItem, " colspan=\"3\" id=\"main\" title=\"Cyclomatic number normalised by the number of nodes\"", "(E + P - N) / N"));
            this.file.Write(string.Format(this.TableHeadItem, " colspan=\"3\" id=\"main\" title=\"Cyclomatic number\"", "E + P - N"));
            this.file.Write(string.Format(this.TableHeadItem, " colspan=\"3\" id=\"main\" title=\"Number of nodes\"", "N"));
            this.file.Write(string.Format(this.TableHeadItem, " colspan=\"2\" id=\"main\" title=\"Number of leaf nodes not contained by this node that are depended upon\"", "Externals"));
            this.file.Write(string.Format(this.TableHeadItem, " colspan=\"5\" id=\"main\" title=\"Source lines of code\"", "SLOC"));
            this.file.Write(string.Format(this.TableHeadItem, " rowspan=\"2\" id=\"main\" title=\"Whether a cycle occurs\"", "Cycle"));
            this.file.Write(string.Format(this.TableHeadItem, " rowspan=\"2\" id=\"main\" title=\"The label of the graph node\"", "Section"));
            this.file.Write(this.TableRowEnd);
            this.file.Write(string.Format(this.TableRowBegin, string.Empty));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Value at the node\"", "Count"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Lower bound of the 90% confidence interval for leaf size\"", "Lower"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Expected leaf size given a log-normal distribution\"", "Exp"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Maximum value of the expected leaf size at the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\" title=\"Upper bound of the 90% confidence interval for leaf size\"", "Upper"));
            this.file.Write(this.TableRowEnd);
            this.file.Write(this.TableHeadEnd);
            this.file.Write(this.TableBodyBegin);
        }

        private void TableRow(Complexity row, int index)
        {
            var name = row.Branch.Path(this.options["sep"]);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            var branch = row.Branch;

            this.file.Write(string.Format(this.TableRowBegin, string.Empty));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format("{0:0.00}", row.EPNNs.Value * 0.0001)));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format("{0:0.00}", row.EPNNs.MaxInTree * 0.0001)));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format("{0:0.00}", row.EPNNs.SumOverTree * 0.0001)));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.EPNs.Value));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.EPNs.MaxInTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.EPNs.SumOverTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.Ns.Value));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.Ns.MaxInTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", row.Ns.SumOverTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", this.dependencies.Assembled.ExternalDependencies[branch].Merged.Count));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", this.dependencies.Assembled.ExternalDependencies[branch].MaxInTree));

            var sloc = this.dependencies.Assembled.SLOCs[branch];
            var lower = string.Empty;
            var upper = string.Empty;
            if (sloc.MaxInTree > sloc.Upper)
            {
                lower = sloc.Lower.ToString();
                upper = sloc.Upper.ToString();
            }

            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", sloc.SumOverTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", lower));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", sloc.Expected));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", sloc.ExpectedMax));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", upper));
            if (this.dependencies.Assembled.Structures[branch].HasCycle)
            {
                this.file.Write(string.Format(this.TableBodyItem, " id=\"alert\"", "Cycle"));
            }
            else
            {
                this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Empty));
            }

            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format(this.Link, index, name)));
            this.file.Write(this.TableRowEnd);
        }

        private void TableBottom()
        {
            this.file.Write(this.TableBodyEnd);
            this.file.Write(this.TableEnd);
            this.file.Write(string.Format(this.Paragraph, string.Empty));
        }

        private void Section(Dependency branch, int index, Dictionary<Dependency, int> mapping)
        {
            var name = branch.Path(this.options["sep"]);
            if (name == string.Empty)
            {
                name = "Top level";
            }

            this.file.Write(string.Format(this.SectionBegin, string.Format(this.Destination, index, name)));

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

            this.file.Write(string.Format("Up to {0}\n", string.Format(this.Link, index, name)));
            this.file.Write(string.Format(this.Paragraph, string.Empty));
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
                this.file.Write(string.Format(this.TableBegin, string.Empty));
                this.file.Write(this.TableHeadBegin);
                this.file.Write(string.Format(this.TableRowBegin, " id=\"main\" title=\"Leaf of this node\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, "Dependency"));
                this.file.Write(string.Format(this.TableHeadItem, " title=\"Number of lines of source code\"", "SLOC"));
                this.file.Write(this.TableRowEnd);
                this.file.Write(this.TableHeadEnd + this.TableBodyBegin);
                foreach (var pair in locs.OrderByDescending(o => o.Value))
                {
                    this.file.Write(string.Format(this.TableRowBegin, string.Empty));
                    this.file.Write(string.Format(this.TableBodyItem, string.Empty, pair.Key));
                    this.file.Write(string.Format(this.TableBodyItem, " align=\"right\"", pair.Value));
                    this.file.Write(this.TableRowEnd);
                }

                this.file.Write(this.TableBodyEnd);
                this.file.Write(this.TableEnd);
                this.file.Write(string.Format(this.Paragraph, string.Empty));
            }
        }

        private void ExternalsTable(Dependency branch)
        {
            if (this.dependencies.Assembled.ExternalDependencies[branch].Merged.Count > 0)
            {
                this.file.Write(string.Format(this.TableBegin, string.Empty));
                this.file.Write(this.TableHeadBegin);
                this.file.Write(string.Format(this.TableRowBegin, " id=\"main\" title=\"Leaf nodes not contained by this node that are depended upon\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, "External dependencies"));
                this.file.Write(this.TableRowEnd);
                this.file.Write(this.TableHeadEnd + this.TableBodyBegin);
                foreach (var dep in this.dependencies.Assembled.ExternalDependencies[branch].Merged.OrderBy(o => o.Path(this.options["sep"])))
                {
                    this.file.Write(string.Format(this.TableRowBegin, string.Empty));
                    this.file.Write(string.Format(this.TableBodyItem, string.Empty, dep.Path(this.options["sep"])));
                    this.file.Write(this.TableRowEnd);
                }

                this.file.Write(this.TableBodyEnd);
                this.file.Write(this.TableEnd);
                this.file.Write(string.Format(this.Paragraph, string.Empty));
            }
        }

        private void InternalsTable(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            var set = new HashSet<string>();
            foreach (var child in branch.Children)
            {
                foreach (var dep in this.dependencies.Assembled.Linkings[child].Interlinks)
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

            this.file.Write(string.Format(this.TableBegin, string.Empty));
            this.file.Write(this.TableHeadBegin);
            this.file.Write(string.Format(this.TableRowBegin, " id=\"main\" title=\"Dependencies that cause the edges of the graph to be formed\""));
            this.file.Write(string.Format(this.TableHeadItem, string.Empty, "Internal Dependencies"));
            this.file.Write(this.TableRowEnd);
            this.file.Write(this.TableHeadEnd + this.TableBodyBegin);
            foreach (var item in list)
            {
                this.file.Write(string.Format(this.TableRowBegin, string.Empty));
                this.file.Write(string.Format(this.TableBodyItem, string.Empty, item));
                this.file.Write(this.TableRowEnd);
            }

            this.file.Write(this.TableBodyEnd);
            this.file.Write(this.TableEnd);
            this.file.Write(string.Format(this.Paragraph, string.Empty));
            this.file.Write("\n");
        }

        private void LinksTable(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            this.file.Write(string.Format(this.TableBegin, string.Empty));
            foreach (var child in branch.Children)
            {
                foreach (var dep in this.dependencies.Assembled.Linkings[child].Interlinks)
                {
                    var first = child.Path(this.options["sep"]);
                    if (mapping.Keys.Contains(child))
                    {
                        first = string.Format(string.Format(this.Link, mapping[child], first));
                    }

                    var second = dep.Path(this.options["sep"]);
                    if (mapping.Keys.Contains(dep))
                    {
                        second = string.Format(string.Format(this.Link, mapping[dep], second));
                    }

                    this.file.Write(this.TableHeadBegin);
                    this.file.Write(string.Format(this.TableRowBegin, " id=\"main\" title=\"Dependencies that cause this edge of the graph to be formed\""));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, first));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, this.RightArrow));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, second));
                    this.file.Write(this.TableRowEnd);
                    this.file.Write(this.TableHeadEnd);
                    this.file.Write(this.TableBodyBegin);
                    var found = FindLinks.Get(child, dep, this.options["sep"]);
                    if (found.Count == 0)
                    {
                        continue;
                    }

                    foreach (var link in FindLinks.Get(child, dep, this.options["sep"]))
                    {
                        this.file.Write(string.Format(this.TableRowBegin, string.Empty));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, link.Key));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, this.RightArrow));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, link.Value));
                        this.file.Write(this.TableRowEnd);
                    }

                    this.file.Write(this.TableBodyEnd);
                }
            }

            this.file.Write(this.TableBodyEnd);
            this.file.Write(this.TableEnd);
        }

        private void Matrix(Dependency branch)
        {
            var list = this.dependencies.Assembled.Structures[branch].Extract();

            if (list.Count == 0)
            {
                return;
            }

            this.file.Write(string.Format(this.Paragraph, string.Empty));
            this.file.Write(string.Format(this.TableBegin, string.Empty));
            this.file.Write(this.TableBodyBegin);
            foreach (var item in list)
            {
                this.file.Write(string.Format(this.TableRowBegin, " style=\"height: 8px\" title=\"Node of the graph followed by dependencies (structure matrix)\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, item.Key));
                foreach (var c in item.Value)
                {
                    this.file.Write(string.Format(this.TableBodyItem, " style=\"width: 8px;\"", c));
                }

                this.file.Write(this.TableRowEnd);
            }

            this.file.Write(this.TableBodyEnd);
            this.file.Write(this.TableEnd);
        }

        public List<Complexity> TableRows()
        {
            return Complexities.Factory(this.dependencies.Root, this.dependencies.Assembled.Linkings);
        }

        public void Table(List<Complexity> rows)
        {
            this.TableTop();
            for (int i = 0; i < rows.Count; ++i)
            {
                this.TableRow(rows[i], i);
            }

            this.TableBottom();
        }

        public void Report()
        {
            var rows = TableRows();

            var mapping = new Dictionary<Dependency, int>();
            for (int i = 0; i < rows.Count; ++i)
            {
                mapping[rows[i].Branch] = i;
            }

            this.TableTopText(mapping[this.dependencies.Root]);

            Table(rows);

            this.file.Write(this.SubsectionEnd);

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Branch;

                this.Section(branch, i, mapping);

                this.TableTop();
                this.TableRow(rows[i], i);
                foreach (var child in branch.Children.OrderBy(o => o.Name))
                {
                    if (!mapping.ContainsKey(child))
                    {
                        continue;
                    }

                    var index = mapping[child];
                    this.TableRow(rows[index], index);
                }

                this.TableBottom();

                this.DependencyTable(branch);

                this.ExternalsTable(branch);

                this.InternalsTable(branch, mapping);

                this.LinksTable(branch, mapping);

                this.Matrix(branch);
            }
        }
    }
}
