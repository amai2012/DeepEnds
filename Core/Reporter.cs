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

        public string LineBegin { get; set; }

        public string Link { get; set; }

        public string ListBegin { get; set; }

        public string ListEnd { get; set; }

        public string ListItem { get; set; }

        public string ParagraphBegin { get; set; }

        public string ParagraphEnd { get; set; }

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
            this.LineBegin = string.Empty;
            this.Link = string.Empty;
            this.ListBegin = string.Empty;
            this.ListEnd = string.Empty;
            this.ListItem = string.Empty;
            this.RightArrow = "->";
            this.ParagraphBegin = string.Empty;
            this.ParagraphEnd = string.Empty;
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

        public void Write(string value)
        {
            this.file.Write(this.LineBegin);
            this.file.Write(value);
        }

        public void WriteLine(string line)
        {
            this.file.Write(this.LineBegin);
            this.file.WriteLine(line);
        }

        public void TableTopText(int topIndex)
        {
            this.Write(string.Format(this.SectionBegin, "Top", "Summary of graph complexity"));
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Section", "Section"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The node label of the graph (with the obvious exception of \"Top level\") with a hyperlink to it.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Cycle", "Cycle"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("If a cycle in the graph (circular dependency) occurs then the word cycle will appear as the value otherwise it ");
            this.WriteLine("is left blank.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Cyclomatic", "Cyclomatic Number"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The larger the value of (E + P) / N then the more complex the directed graph is, where");
            this.file.Write(this.ParagraphEnd);
            this.WriteLine(string.Empty);
            this.file.Write(this.ListBegin);
            this.Write(string.Format(this.ListItem, "E: Number of edges"));
            this.Write(string.Format(this.ListItem, "P: Number of parts"));
            this.Write(string.Format(this.ListItem, "N: Number of nodes"));
            this.Write(this.ListEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Externals", "Externals"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("Externals refers to the number of dependencies which aren't children.");
            this.WriteLine("The following max refers to the maximum of that value and the value at any child nodes.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "SLOC", "SLOC"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("SLOC stands for source lines of code; whilst reading the source an attempt may have ");
            this.WriteLine("been made not to count blank or comment lines. The max and sum are calculated over ");
            this.WriteLine("the child nodes recursively down to the leaf nodes.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Probability", "Probability of SLOC"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("An attempt has been made to fit the log-normal distribution to SLOC which has then ");
            this.WriteLine("been used to calculate the expected file size, this is not displayed if there is only ");
            this.WriteLine("one value to fit (so as to avoid domination of the statistic by boilerplate). This ");
            this.WriteLine("value is bracketed by the lower and upper limits of the 90% confidence interval if ");
            this.WriteLine("the upper limit is less than the maximal value of SLOC on a leaf, otherwise it is ");
            this.WriteLine("left blank. The following max refers to the maximum of the expected value and the ");
            this.WriteLine("value at any child nodes.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.SubsectionEnd);
            this.WriteLine(string.Empty);
            this.Write(string.Format(this.SubsectionBegin, "Table", "Table"));
            this.file.Write(this.ParagraphBegin);
            this.Write("Skip to ");
            this.file.WriteLine(string.Format(this.Link, topIndex, "Top level"));
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The following table is sorted on its first column, subsequent instances of this table type are sorted on ");
            this.WriteLine("the final column. Hover over the table headers to make tool tips appear.");
            this.file.Write(this.ParagraphEnd);
            this.WriteLine(string.Empty);
        }

        private void TableTop(bool forceVisible)
        {
            this.Write(string.Format(this.TableBegin, " id=\"main\""));
            this.Write(this.TableHeadBegin);
            this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " rowspan=\"2\" title=\"The label of the graph node\"", "Section"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " rowspan=\"2\" title=\"Whether a cycle occurs\"", "Cycle"));

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"3\" title=\"Cyclomatic number normalised by the number of nodes\"", "(E + P - N) / N"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"3\" title=\"Cyclomatic number\"", "E + P - N"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"3\" title=\"Number of nodes\"", "N"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"2\" title=\"Number of leaf nodes not contained by this node that are depended upon\"", "Externals"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"2\" title=\"Source lines of code\"", "SLOC"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " colspan=\"4\" title=\"Log-normal distribution\"", "Probability of SLOC"));
            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(this.TableRowEnd);
            this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
            if (forceVisible)
            {
                this.file.Write(this.TableHeadBegin);
            }

            if (forceVisible)
            {
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Empty, string.Empty));
            }

            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Value at the node\"", "Val"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Value at the node\"", "Count"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the node and child nodes recursively\"", "Max"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Sum of node value and child node values recursively\"", "Sum"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Lower bound of the 90% confidence interval for leaf size\"", "Lower"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Expected leaf size given a log-normal distribution\"", "Exp"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Upper bound of the 90% confidence interval for leaf size\"", "Upper"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Maximum value of the expected leaf size at the node and child nodes recursively\"", "Max"));

            this.file.Write(this.TableRowEnd);
            this.Write(this.TableHeadEnd);
            this.Write(this.TableBodyBegin);
        }

        private void TableRow(Complexity row, int index, bool forceVisible)
        {
            var name = row.Branch.Path(this.options["sep"]);
            if (name.Length == 0)
            {
                name = "Top level";
            }

            var branch = row.Branch;

            this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format(this.Link, index, name)));
            if (this.dependencies.Assembled.Structures[branch].HasCycle)
            {
                this.file.Write(string.Format(this.TableBodyItem, " id=\"alert\"", "Cycle"));
            }
            else
            {
                this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Empty));
            }

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
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", sloc.MaxInTree));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", sloc.SumOverTree));

            var lower = string.Empty;
            var upper = string.Empty;
            var expected = string.Empty;
            var expectedMax = string.Empty;
            if (sloc.Expected > 0)
            {
                if (sloc.MaxInTree > sloc.Upper)
                {
                    lower = sloc.Lower.ToString();
                    upper = sloc.Upper.ToString();
                }
                else if (forceVisible)
                {
                    lower = "0";
                    upper = sloc.MaxInTree.ToString();
                }

                expected = sloc.Expected.ToString();
                expectedMax = sloc.ExpectedMax.ToString();
            }
            else if (forceVisible)
            {
                expected = sloc.MaxInTree.ToString();
                expectedMax = expected;
                lower = expected;
                upper = expected;
            }

            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", lower));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", expected));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", upper));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", expectedMax));

            this.file.Write(this.TableRowEnd);
        }

        private void TableBottom()
        {
            this.Write(this.TableBodyEnd);
            this.Write(this.TableEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
        }

        private void Section(Dependency branch, int index, Dictionary<Dependency, int> mapping)
        {
            var name = branch.Path(this.options["sep"]);
            if (name.Length == 0)
            {
                name = "Top level";
            }

            this.Write(string.Format(this.SectionBegin,  index, name));

            if (branch.Parent == null)
            {
                return;
            }

            index = mapping[branch.Parent];
            name = branch.Parent.Path(this.options["sep"]);
            if (name.Length == 0)
            {
                name = "Top level";
            }

            this.Write("Up to ");
            this.file.WriteLine(string.Format(this.Link, index, name));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
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
                this.Write(string.Format(this.TableBegin, string.Empty));
                this.Write(this.TableHeadBegin);
                this.Write(string.Format(this.TableRowBegin, " id=\"main\"", " title=\"Leaf of this node\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, "Dependency"));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, " title=\"Number of lines of source code\"", "SLOC"));
                this.file.Write(this.TableRowEnd);
                this.Write(this.TableHeadEnd);
                this.Write(this.TableBodyBegin);
                foreach (var pair in locs.OrderByDescending(o => o.Value))
                {
                    this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
                    this.file.Write(string.Format(this.TableBodyItem, string.Empty, pair.Key));
                    this.file.Write(string.Format(this.TableBodyItem, " align=\"right\"", pair.Value));
                    this.file.Write(this.TableRowEnd);
                }

                this.Write(this.TableBodyEnd);
                this.Write(this.TableEnd);
                this.file.Write(this.ParagraphBegin);
                this.WriteLine(string.Empty);
                this.file.Write(this.ParagraphEnd);
            }
        }

        private void UsageTable(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            var leaves = new List<Dependency>();
            foreach (var child in branch.Children)
            {
                if (child.Children.Count != 0)
                {
                    continue;
                }

                leaves.Add(child);
            }

            if (leaves.Count == 0)
            {
                return;
            }

            var levels = CalculateUsage.Levels(branch);

            this.Write(string.Format(this.TableBegin, string.Empty));
            this.Write(this.TableHeadBegin);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", string.Empty));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " rowspan=\"2\" title=\"Leaf of this node\"", "Dependency"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Format(" colspan=\"{0}\" title=\"Number of times leaf is referenced beneath or blank if the maximum has been reached\"", levels.Count), "Sum at this level"));
            this.file.Write(this.TableRowEnd);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", string.Empty));
            foreach (var item in levels)
            {
                var name = item.Name;
                if (item.Parent == null)
                {
                    name = "Top Level";
                }

                var index = mapping[item];
                this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " title=\"Number of times leaf is referenced beneath or blank if the maximum has been reached\"", string.Format(this.Link, index, name)));
            }

            this.file.Write(this.TableRowEnd);
            this.Write(this.TableHeadEnd);
            this.Write(this.TableBodyBegin);
            foreach (var leaf in leaves.OrderBy(o => o.Name))
            {
                this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", leaf.Name));
                var counts = this.dependencies.Assembled.Usages[leaf];
                var max = counts[levels.Count - 1];
                var found = false;
                foreach (var count in counts)
                {
                    if (found)
                    {
                        this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Empty));
                    }
                    else
                    {
                        found = count == max;
                        this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", count));
                    }
                }

                this.file.Write(this.TableRowEnd);
            }

            this.Write(this.TableBodyEnd);
            this.Write(this.TableEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
        }

        private void InterfaceTable(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            if (!this.dependencies.Assembled.Interfaces.ContainsKey(branch))
            {
                return;
            }

            var list = new List<string>();
            foreach (var dep in this.dependencies.Assembled.Interfaces[branch])
            {
                list.Add(dep.Path(this.options["sep"]));
            }

            this.Listing(list, "Leaf nodes contained by this node that are depended upon at no higher level", "Dependencies not used at a higher level");
        }

        private void ExternalsTable(Dependency branch)
        {
            if (this.dependencies.Assembled.ExternalDependencies[branch].Merged.Count > 0)
            {
                var list = new List<string>();
                foreach (var dep in this.dependencies.Assembled.ExternalDependencies[branch].Merged)
                {
                    list.Add(dep.Path(this.options["sep"]));
                }

                this.Listing(list, "Leaf nodes not contained by this node that are depended upon", "External Dependencies");
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

            if (set.Count == 0)
            {
                return;
            }

            var list = set.ToList();
            this.Listing(list, "Dependencies that cause the edges of the graph to be formed", "Internal Dependencies");
        }

        private void Listing(List<string> list, string title, string heading)
        {
            list.Sort();

            this.Write(string.Format(this.TableBegin, string.Empty));
            this.Write(this.TableHeadBegin);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", string.Format(" title=\"{0}\"", title)));
            this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, heading));
            this.file.Write(this.TableRowEnd);
            this.Write(this.TableHeadEnd);
            this.Write(this.TableBodyBegin);
            foreach (var item in list)
            {
                this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
                this.file.Write(string.Format(this.TableBodyItem, string.Empty, item));
                this.file.Write(this.TableRowEnd);
            }

            this.Write(this.TableBodyEnd);
            this.Write(this.TableEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
            this.WriteLine(string.Empty);
        }

        private void LinksTable(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            this.Write(string.Format(this.TableBegin, string.Empty));
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

                    this.Write(this.TableHeadBegin);
                    this.Write(string.Format(this.TableRowBegin, " id=\"main\"", " title=\"Dependencies that cause this edge of the graph to be formed\""));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, first));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, this.RightArrow));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, second));
                    this.file.Write(this.TableRowEnd);
                    this.Write(this.TableHeadEnd);
                    this.Write(this.TableBodyBegin);
                    var found = FindLinks.Get(child, dep, this.options["sep"]);
                    if (found.Count == 0)
                    {
                        continue;
                    }

                    foreach (var link in FindLinks.Get(child, dep, this.options["sep"]))
                    {
                        this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, link.Key));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, this.RightArrow));
                        this.file.Write(string.Format(this.TableBodyItem, string.Empty, link.Value));
                        this.file.Write(this.TableRowEnd);
                    }

                    this.Write(this.TableBodyEnd);
                }
            }

            this.Write(this.TableBodyEnd);
            this.Write(this.TableEnd);
        }

        private void Matrix(Dependency branch)
        {
            var list = this.dependencies.Assembled.Structures[branch].Extract();

            if (list.Count < 2)
            {
                return;
            }

            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
            this.Write(string.Format(this.TableBegin, string.Empty));
            this.Write(this.TableBodyBegin);
            foreach (var item in list)
            {
                this.Write(string.Format(this.TableRowBegin, string.Empty, " style=\"height: 8px\" title=\"Node of the graph followed by dependencies (structure matrix)\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, item.Key));
                foreach (var c in item.Value)
                {
                    this.file.Write(string.Format(this.TableBodyItem, " style=\"width: 8px;\"", c));
                }

                this.file.Write(this.TableRowEnd);
            }

            this.Write(this.TableBodyEnd);
            this.Write(this.TableEnd);
        }

        private void DotFile(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            this.WriteLine("\\dot");
            this.WriteLine("digraph solution {");
            this.WriteLine("	subgraph cluster_0 {");
            this.Write("		label=\"");
            this.file.Write(branch.Name);
            this.file.WriteLine("\";");

            var links = this.dependencies.Assembled.Linkings;
            foreach (var child in branch.Children)
            {
                var index = branch.Children.IndexOf(child);

                this.Write("		N");
                this.file.Write(index);
                this.file.Write(" [label=\"");
                this.file.Write(child.Name);
                this.file.Write("\"");

                if (mapping.ContainsKey(child))
                {
                    this.file.Write(", URL=\"\\ref DeepEnds");
                    this.file.Write(mapping[child]);
                    this.file.Write("\"");
                }

                this.file.WriteLine("];");

                foreach (var dep in links[child].Interlinks)
                {
                    var other = branch.Children.IndexOf(dep);
                    this.Write("		N");
                    this.file.Write(index);
                    this.file.Write(" -> N");
                    this.file.Write(other);
                    this.file.WriteLine(";");
                }
            }

            this.WriteLine("	}");
            this.WriteLine("}");
            this.WriteLine("\\enddot");
        }

        private List<Complexity> TableRows()
        {
            return Complexities.Factory(this.dependencies.Root, this.dependencies.Assembled.Linkings);
        }

        private void Table(List<Complexity> rows, bool forceVisible)
        {
            this.TableTop(forceVisible);
            for (int i = 0; i < rows.Count; ++i)
            {
                this.TableRow(rows[i], i, forceVisible);
            }

            this.TableBottom();
        }

        public void TableOnly()
        {
            this.Table(this.TableRows(), true);
        }

        public void Report(bool writeDot)
        {
            bool forceVisible = false;

            var rows = this.TableRows();

            var mapping = new Dictionary<Dependency, int>();
            for (int i = 0; i < rows.Count; ++i)
            {
                mapping[rows[i].Branch] = i;
            }

            this.TableTopText(mapping[this.dependencies.Root]);

            this.Table(rows, forceVisible);

            this.file.Write(this.SubsectionEnd);

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Branch;

                this.Section(branch, i, mapping);

                if (writeDot)
                {
                    this.DotFile(branch, mapping);
                }

                this.TableTop(forceVisible);
                this.TableRow(rows[i], i, forceVisible);
                foreach (var child in branch.Children.OrderBy(o => o.Name))
                {
                    if (!mapping.ContainsKey(child))
                    {
                        continue;
                    }

                    var index = mapping[child];
                    this.TableRow(rows[index], index, forceVisible);
                }

                this.TableBottom();

                this.DependencyTable(branch);

                this.UsageTable(branch, mapping);

                this.InterfaceTable(branch, mapping);

                this.ExternalsTable(branch);

                this.InternalsTable(branch, mapping);

                this.LinksTable(branch, mapping);

                this.Matrix(branch);
            }
        }
    }
}
