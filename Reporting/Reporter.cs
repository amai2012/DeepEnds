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

namespace DeepEnds.Reporting
{
    using DeepEnds.Reporting.Complex;
    using DeepEnds.Core.Dependent;

    using System.Collections.Generic;
    using System.Linq;

    public class Reporter
    {
        private DeepEnds.Reporting.Linked.Assemble assembled;

        private string filePath;

        private string filePathTemplate;

        private System.IO.StreamWriter file;

        private Dictionary<string, string> options;

        private DeepEnds.Core.Linked.Dependencies dependencies;

        public string FileHeader { get; set; }

        public string FileFooter { get; set; }

        public string LineBegin { get; set; }

        public string Link { get; set; }

        public string LinkExt { get; set; }

        public string ListBegin { get; set; }

        public string ListEnd { get; set; }

        public string ListItem { get; set; }

        public string ParagraphBegin { get; set; }

        public string ParagraphEnd { get; set; }

        public string RightArrow { get; set; }

        public string SectionBegin { get; set; }

        public string SubsectionBegin { get; set; }

        public string SubsectionEnd { get; set; }

        public string SubSubsectionBegin { get; set; }

        public string TableBegin { get; set; }

        public string TableEnd { get; set; }

        public string TableBodyBegin { get; set; }

        public string TableBodyEnd { get; set; }

        public string TableBodyItem { get; set; }

        public string TableDivItem { get; set; }

        public string TableHeadBegin { get; set; }

        public string TableHeadEnd { get; set; }

        public string TableHeadItem { get; set; }

        public string TableRowBegin { get; set; }

        public string TableRowEnd { get; set; }

        public Reporter(string filePath, Dictionary<string, string> options, DeepEnds.Core.Linked.Dependencies dependencies, DeepEnds.Reporting.Linked.Assemble assembled)
        {
            this.filePath = filePath;
            this.file = null;
            this.options = options;
            this.dependencies = dependencies;
            this.assembled = assembled;

            this.filePathTemplate = string.Empty;
            if (this.options["split"] != "false")
            {
                this.filePathTemplate = string.Format("{0}\\DeepEnds{{0}}{1}", System.IO.Path.GetDirectoryName(this.filePath), System.IO.Path.GetExtension(this.filePath));
            }

            this.FileHeader = string.Empty;
            this.FileFooter = string.Empty;
            this.LineBegin = string.Empty;
            this.Link = string.Empty;
            this.LinkExt = string.Empty;
            this.ListBegin = string.Empty;
            this.ListEnd = string.Empty;
            this.ListItem = string.Empty;
            this.RightArrow = "->";
            this.ParagraphBegin = string.Empty;
            this.ParagraphEnd = string.Empty;
            this.SectionBegin = string.Empty;
            this.SubsectionBegin = string.Empty;
            this.SubsectionEnd = string.Empty;
            this.SubSubsectionBegin = string.Empty;
            this.TableBegin = string.Empty;
            this.TableEnd = string.Empty;
            this.TableBodyBegin = string.Empty;
            this.TableBodyEnd = string.Empty;
            this.TableBodyItem = string.Empty;
            this.TableDivItem = string.Empty;
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

        public void PageBegin(string id, string label)
        {
            if (this.options["split"] != "false")
            {
                this.file = new System.IO.StreamWriter(string.Format(this.filePathTemplate, id));
                this.file.Write(this.FileHeader);
            }

            this.WriteLine(string.Format(this.SectionBegin, id, label));
        }

        public void PageEnd()
        {
            if (this.options["split"] != "false")
            {
                this.file.Write(this.FileFooter);
                this.file.Close();
                this.file = null;
            }
        }

        public void IntroText(bool visibleHeader)
        {
            if (this.Link.Contains("{0}"))
            {
                this.file.Write(this.ParagraphBegin);
                this.Write("Skip to ");
                this.file.Write(string.Format(this.Link, "Summary", "summary"));
                this.file.Write(".\n");
                this.WriteLine(this.ParagraphEnd);
            }

            this.file.Write(this.ParagraphBegin);
            this.WriteLine("This report was written by ");
            this.WriteLine(string.Format(this.LinkExt, "https://github.com/zebmason/deepends", "DeepEnds"));
            this.WriteLine(" which is distributed as both a ");
            this.WriteLine(string.Format(this.LinkExt, "https://marketplace.visualstudio.com/items?itemName=ZebM.DeepEnds", "Visual Studio extension"));
            this.WriteLine(" and as a ");
            this.WriteLine(string.Format(this.LinkExt, "https://www.nuget.org/packages/DeepEnds.Console/", "NuGet package."));
            this.WriteLine("Source code is available from GitHub, there are a number of articles, available online, about its usage:");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.ListBegin);
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "http://htmlpreview.github.com/?https://github.com/zebmason/DeepEnds/blob/master/Doc/userguide.html", "Dive into Architecture with DeepEnds")));
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "http://www.codeproject.com/Articles/1137021/Dependency-Analysis-in-Visual-Cplusplus", "Dependency Analysis in Visual C++")));
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "https://www.codeproject.com/Articles/1155619/Dependency-Analysis-with-Doxygen", "Dependency Analysis with Doxygen")));
            this.WriteLine(this.ListEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("There are also some articles on the motivation and theoretical foundations of the software:");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.ListBegin);
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "http://www.codeproject.com/Articles/1098935/As-Is-Software-Architecture", "As-Is Software Architecture")));
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "https://www.codeproject.com/Tips/1158303/Big-Design-Up-Front-or-Emergent-Design", "Big Design Up Front or Emergent Design?")));
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "https://www.codeproject.com/Tips/1116433/Why-Favour-the-Cyclomatic-Number", "Why Favour the Cyclomatic Number?")));
            this.WriteLine(string.Format(this.ListItem, string.Format(this.LinkExt, "https://www.codeproject.com/Tips/1136171/Counting-Lines-of-Code", "Counting Lines of Code")));
            this.WriteLine(this.ListEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("The following sections refer to the individual sections for each level.");
            this.WriteLine(this.ParagraphEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Table", "Table"));

            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("The table is grouped by columns. The summary table is sorted on the value of ");
            this.WriteLine("the value of (E+P-N)/N, subsequent instances are sorted on the section name.");
            this.WriteLine(this.ParagraphEnd);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "Section", "Section"));
            this.WriteLine(this.ParagraphBegin);
            this.Write("The node label of the graph (with the obvious exception of \"Top level\")");
            if (!visibleHeader)
            {
                this.file.Write(" with a hyperlink to it");
            }

            this.file.WriteLine(".");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "Cycle", "Cycle"));
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("If a cycle in the graph (circular dependency) occurs then the word cycle will appear as the value otherwise it ");
            this.WriteLine("is left blank.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "Cyclomatic", "Cyclomatic Number"));
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("The larger the value of (E + P) / N then the more complex the directed graph is, where");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(string.Empty);
            this.WriteLine(this.ListBegin);
            this.WriteLine(string.Format(this.ListItem, "E: Number of edges"));
            this.WriteLine(string.Format(this.ListItem, "P: Number of parts"));
            this.WriteLine(string.Format(this.ListItem, "N: Number of nodes"));
            this.WriteLine(this.ListEnd);
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "Externals", "Externals"));
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("Externals refers to the number of dependencies which aren't children.");
            this.WriteLine("The following max refers to the maximum of that value and the value at any child nodes.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "SLOC", "SLOC"));
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("SLOC stands for source lines of code; whilst reading the source an attempt may have ");
            this.WriteLine("been made not to count blank or comment lines. The max and sum are calculated over ");
            this.WriteLine("the child nodes recursively down to the leaf nodes.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubSubsectionBegin, "Probability", "Probability of SLOC"));
            this.WriteLine(this.ParagraphBegin);
            this.WriteLine("An attempt has been made to fit the log-normal distribution to SLOC which has then ");
            this.WriteLine("been used to calculate the expected file size, this is not displayed if there is only ");
            this.WriteLine("one value to fit (so as to avoid domination of the statistic by boilerplate). This ");
            this.WriteLine("value is bracketed by the lower and upper limits of the 90% confidence interval if ");
            this.WriteLine("the upper limit is less than the maximal value of SLOC on a leaf, otherwise it is ");
            this.WriteLine("left blank. The following max refers to the maximum of the expected value and the ");
            this.WriteLine("value at any child nodes.");
            this.WriteLine(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
            this.WriteLine(string.Empty);
        }

        public void TableTopText(bool visibleHeader)
        {
            if (this.Link.Contains("{0}"))
            {
                this.WriteLine(string.Format(this.SubsectionBegin, "Docs", "Documentation"));

                this.file.Write(this.ParagraphBegin);
                this.Write("Skip to ");
                this.file.Write(string.Format(this.Link, "Doc", "documentation"));
                this.file.Write(".");
                this.file.WriteLine(this.ParagraphEnd);
            }

            this.WriteLine(string.Empty);

            this.WriteLine(string.Format(this.SubsectionBegin, "Table", "Table"));

            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The following table is sorted on the value of (E+P-N)/N, subsequent instances of this table type are sorted on ");
            this.WriteLine("the section name.");
            if (!visibleHeader)
            {
                this.WriteLine("Hover over the table headers to make tool tips appear.");
            }

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
            this.TableHeadRowDiv(19);
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

        private void TableHeadRowDiv(int columns)
        {
            if (this.TableDivItem.Length == 0)
            {
                return;
            }

            this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
            for (int i = 0; i < columns; ++i)
            {
                this.file.Write(string.Format(this.TableBodyItem, string.Empty, this.TableDivItem));
            }

            this.file.Write(this.TableRowEnd);
        }

        private void TableRow(Complexity row, int index, bool forceVisible)
        {
            var branch = row.Branch;
            var name = this.BranchName(branch);

            this.Write(string.Format(this.TableRowBegin, string.Empty, string.Empty));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", string.Format(this.Link, index, name)));
            if (this.assembled.Structures[branch].HasCycle)
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
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", this.assembled.ExternalDependencies[branch].Merged.Count));
            this.file.Write(string.Format(this.TableBodyItem, " id=\"main\"", this.assembled.ExternalDependencies[branch].MaxInTree));

            var sloc = this.assembled.SLOCs[branch];
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

        public void OtherText(bool visibleHeader, bool writeDot)
        {
            if (writeDot)
            {
                this.WriteLine(string.Format(this.SubsectionBegin, "Graph", "Graph"));
                this.WriteLine("This image plots the graph of the dependencies of the level.");
                this.WriteLine(this.SubsectionEnd);
            }

            this.WriteLine(string.Format(this.SubsectionBegin, "Dependency", "Lines of code"));
            this.WriteLine("This table is a list of leaf nodes versus the");
            this.WriteLine("number of lines of code sorted on decreasing size.");
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Usage", "Usage"));
            this.WriteLine("This table is an alphabetically sorted list of the leaf nodes in this level");
            this.WriteLine("versus their usage at this level and all higher levels.");
            this.WriteLine("Once the maximal value has been achieved for a row the subsequent columns");
            this.WriteLine("are filled with blanks.");
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Interface", "Interface"));
            this.WriteLine("This table is an alphabetically sorted list of the leaf nodes that aren't used");
            this.WriteLine("at a higher level.");
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Externals", "Externals"));
            this.WriteLine("This table is an alphabetically sorted list of the leaf nodes that are dependencies");
            this.WriteLine("but aren't children of this level.");
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Internals", "Internals"));
            this.WriteLine("This table is an alphabetically sorted list of the leaf nodes that constitute the dependencies");
            this.WriteLine("from which the edges are formed. Note that dependent nodes are not included unless they");
            this.WriteLine("also happen to be depended upon.");
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "Links", "Edge definitions"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The edges are defined by underlying dependencies.");
            this.WriteLine("For each edge a table lists all the pairs of leaf nodes");
            this.WriteLine("that the edge is composed of.");
            this.file.Write(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);

            this.WriteLine(string.Format(this.SubsectionBegin, "StructureMatrix", "Structure Matrix"));
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The structure matrix represents the dependency of one node upon another in the level.");
            this.WriteLine("To work out what a node depends on read along a row until the value of 1 is encountered");
            this.WriteLine("then read vertically to the diagonal (represented by a backslash).");
            this.WriteLine("The row containing that diagonal is the corresponding dependency.");
            this.file.Write(this.ParagraphEnd);
            this.file.Write(this.ParagraphBegin);
            this.WriteLine("The rows of the matrix have been sorted in the attempt to ensure that the 1's are");
            this.WriteLine("below the diagonal. If this is not the case then a cycle exists in the corresponding");
            this.WriteLine("graph. i.e. a circular dependency exists.");
            this.file.Write(this.ParagraphEnd);
            this.WriteLine(this.SubsectionEnd);
        }

        private string BranchName(Dependency branch)
        {
            var name = branch.Path(this.options["sep"]);
            if (name.Length == 0)
            {
                name = "Top level";
            }

            return name;
        }

        private void WriteUp(Dependency branch, Dictionary<Dependency, int> mapping)
        {
            if (!this.Link.Contains("{0}"))
            {
                return;
            }

            var index = "Summary";
            var name = index;
            if (branch.Parent != null)
            {
                index = mapping[branch.Parent].ToString();
                name = this.BranchName(branch.Parent);
            }

            this.file.Write(this.ParagraphBegin);
            this.Write("Up to ");
            this.file.WriteLine(string.Format(this.Link, index, name));
            this.file.Write(this.ParagraphEnd);
        }

        private void DependencyTable(Dependency branch, int section)
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
                this.WriteLine(string.Format(this.SubsectionBegin, "Dependency" + section, "Lines of code"));

                this.Write(string.Format(this.TableBegin, string.Empty));
                this.Write(this.TableHeadBegin);
                this.Write(string.Format(this.TableRowBegin, " id=\"main\"", " title=\"Leaf of this node\""));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, "Dependency"));
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, " title=\"Number of lines of source code\"", "SLOC"));
                this.file.Write(this.TableRowEnd);
                this.TableHeadRowDiv(2);
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

                this.file.Write(this.SubsectionEnd);
            }
        }

        private void UsageTable(Dependency branch, Dictionary<Dependency, int> mapping, bool visibleHeader, int section)
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

            this.WriteLine(string.Format(this.SubsectionBegin, "Usage" + section, "Usage"));

            var levels = CalculateUsage.Levels(branch);

            this.Write(string.Format(this.TableBegin, string.Empty));
            this.Write(this.TableHeadBegin);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", string.Empty));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", " rowspan=\"2\" title=\"Leaf of this node\"", "Dependency"));
            this.file.Write(string.Format(this.TableHeadItem, " id=\"main\"", string.Format(" colspan=\"{0}\" title=\"Number of times leaf is referenced beneath or blank if the maximum has been reached\"", levels.Count), "Sum at this level"));
            if (visibleHeader)
            {
                for (int i = 1; i < levels.Count; ++i)
                {
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, string.Empty));
                }
            }

            this.file.Write(this.TableRowEnd);
            this.TableHeadRowDiv(levels.Count + 1);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", string.Empty));
            if (visibleHeader)
            {
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, string.Empty));
            }

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
                var counts = this.assembled.Usages[leaf];
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

            this.file.Write(this.SubsectionEnd);
        }

        private void InterfaceTable(Dependency branch, Dictionary<Dependency, int> mapping, int section)
        {
            if (!this.assembled.Interfaces.ContainsKey(branch))
            {
                return;
            }

            this.WriteLine(string.Format(this.SubsectionBegin, "Interface" + section, "Interface"));
            var list = new List<string>();
            foreach (var dep in this.assembled.Interfaces[branch])
            {
                list.Add(dep.Path(this.options["sep"]));
            }

            this.Listing(list, "Leaf nodes contained by this node that are depended upon at no higher level", "Dependencies not used at a higher level");

            this.file.Write(this.SubsectionEnd);
        }

        private void ExternalsTable(Dependency branch, int section)
        {
            if (this.assembled.ExternalDependencies[branch].Merged.Count == 0)
            {
                return;
            }

            this.WriteLine(string.Format(this.SubsectionBegin, "Externals" + section, "Externals"));

            var list = new List<string>();
            foreach (var dep in this.assembled.ExternalDependencies[branch].Merged)
            {
                list.Add(dep.Path(this.options["sep"]));
            }

            this.Listing(list, "Leaf nodes not contained by this node that are depended upon", "External Dependencies");

            this.file.Write(this.SubsectionEnd);
        }

        private void InternalsTable(Dependency branch, Dictionary<Dependency, int> mapping, int section)
        {
            var set = new HashSet<string>();
            foreach (var child in branch.Children)
            {
                foreach (var dep in this.assembled.Linkings[child].Interlinks)
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
            this.WriteLine(string.Format(this.SubsectionBegin, "Internals" + section, "Internals"));
            this.Listing(list, "Dependencies that cause the edges of the graph to be formed", "Internal Dependencies");
            this.file.Write(this.SubsectionEnd);
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
            this.TableHeadRowDiv(1);
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

        private bool EdgeExists(Dependency branch)
        {
            foreach (var child in branch.Children)
            {
                foreach (var dep in this.assembled.Linkings[child].Interlinks)
                {
                    return true;
                }
            }

            return false;
        }

        private void LinksTable(Dependency branch, Dictionary<Dependency, int> mapping, bool visibleHeader, int section)
        {
            if (!this.EdgeExists(branch))
            {
                return;
            }

            this.WriteLine(string.Format(this.SubsectionBegin, "Links" + section, "Edge definitions"));

            this.Write(string.Format(this.TableBegin, string.Empty));
            foreach (var child in branch.Children)
            {
                foreach (var dep in this.assembled.Linkings[child].Interlinks)
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

                    if (visibleHeader)
                    {
                        this.Write(string.Format(this.ListItem, string.Empty));
                    }

                    this.Write(this.TableHeadBegin);
                    this.Write(string.Format(this.TableRowBegin, " id=\"main\"", " title=\"Dependencies that cause this edge of the graph to be formed\""));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, first));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, this.RightArrow));
                    this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, second));
                    this.file.Write(this.TableRowEnd);
                    this.Write(this.TableHeadEnd);
                    if (visibleHeader)
                    {
                        this.TableHeadRowDiv(3);
                    }

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

            this.file.Write(this.SubsectionEnd);
        }

        private void Matrix(Dependency branch, int section)
        {
            var list = this.assembled.Structures[branch].Extract();

            if (list.Count < 2)
            {
                return;
            }

            this.WriteLine(string.Format(this.SubsectionBegin, "StructureMatrix" + section, "Structure Matrix"));

            this.file.Write(this.ParagraphBegin);
            this.WriteLine(string.Empty);
            this.file.Write(this.ParagraphEnd);
            this.Write(string.Format(this.TableBegin, string.Empty));

            this.Write(this.TableHeadBegin);
            this.Write(string.Format(this.TableRowBegin, " id=\"main\"", " title=\"Structure matrix\""));
            this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, "Dependency"));
            for (int i = 0; i < list.Count; ++i)
            {
                this.file.Write(string.Format(this.TableHeadItem, string.Empty, string.Empty, string.Empty));
            }

            this.file.Write(this.TableRowEnd);
            this.Write(this.TableHeadEnd);
            this.TableHeadRowDiv(list.Count + 1);

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

            this.file.Write(this.SubsectionEnd);
        }

        private void DotFile(Dependency branch, Dictionary<Dependency, int> mapping, int section)
        {
            this.WriteLine(string.Format(this.SubsectionBegin, "Graph" + section, "Graph"));

            this.WriteLine("\\dot");
            this.WriteLine("digraph solution {");
            this.WriteLine("	subgraph cluster_0 {");
            this.Write("		label=\"");
            this.file.Write(branch.Name);
            this.file.WriteLine("\";");

            var links = this.assembled.Linkings;
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

            this.file.Write(this.SubsectionEnd);
        }

        private List<Complexity> TableRows()
        {
            return Complexities.Factory(this.dependencies.Root, this.assembled.Linkings);
        }

        private void Table(List<Complexity> rows, bool visibleHeader, bool visibleValues)
        {
            this.TableTop(visibleHeader);
            for (int i = 0; i < rows.Count; ++i)
            {
                this.TableRow(rows[i], i, visibleValues);
            }

            this.TableBottom();
        }

        public void TableOnly()
        {
            this.file = new System.IO.StreamWriter(this.filePath);
            this.Table(this.TableRows(), true, true);
            this.file.Close();
        }

        public void Report(bool writeDot, bool visibleHeader)
        {
            if (this.options["split"] == "false")
            {
                this.file = new System.IO.StreamWriter(this.filePath);
                this.file.Write(this.FileHeader);
            }

            var rows = this.TableRows();

            var mapping = new Dictionary<Dependency, int>();
            for (int i = 0; i < rows.Count; ++i)
            {
                mapping[rows[i].Branch] = i;
            }

            this.PageBegin("Doc", "Documentation");
            this.IntroText(visibleHeader);
            this.OtherText(visibleHeader, writeDot);
            this.PageEnd();

            this.PageBegin("Summary", "Summary of graph complexity");
            this.TableTopText(visibleHeader);
            this.Table(rows, visibleHeader, false);
            this.file.Write(this.SubsectionEnd);
            this.PageEnd();

            for (int i = 0; i < rows.Count; ++i)
            {
                var branch = rows[i].Branch;

                this.PageBegin(i.ToString(), this.BranchName(branch));

                this.WriteUp(branch, mapping);

                if (writeDot)
                {
                    this.DotFile(branch, mapping, i);
                }

                this.WriteLine(string.Format(this.SubsectionBegin, "Table" + i, "Table"));
                this.TableTop(visibleHeader);
                this.TableRow(rows[i], i, false);
                foreach (var child in branch.Children.OrderBy(o => o.Name))
                {
                    if (!mapping.ContainsKey(child))
                    {
                        continue;
                    }

                    var index = mapping[child];
                    this.TableRow(rows[index], index, false);
                }

                this.TableBottom();

                this.file.Write(this.SubsectionEnd);

                this.DependencyTable(branch, i);

                this.UsageTable(branch, mapping, visibleHeader, i);

                this.InterfaceTable(branch, mapping, i);

                this.ExternalsTable(branch, i);

                this.InternalsTable(branch, mapping, i);

                this.LinksTable(branch, mapping, visibleHeader, i);

                this.Matrix(branch, i);

                this.PageEnd();
            }

            if (this.options["split"] == "false")
            {
                this.file.Write(this.FileFooter);
                this.file.Close();
            }
        }
    }
}
