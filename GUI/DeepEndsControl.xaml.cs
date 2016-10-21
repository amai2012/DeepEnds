//------------------------------------------------------------------------------
// <copyright file="DeepEndsControl.xaml.cs" company="Zebedee Mason">
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

namespace DeepEnds.GUI
{
    using DeepEnds.Console;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DeepEndsControl.
    /// </summary>
    public partial class DeepEndsControl : UserControl
    {
        private View view;
        private Dictionary<string, string> options;
        private Dictionary<string, Options.Browse> types;
        private Dictionary<string, string> filters;
        private Dictionary<string, TextBox> values;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeepEndsControl"/> class.
        /// </summary>
        public DeepEndsControl()
        {
            this.InitializeComponent();

            this.view = new View();
            this.options = Options.Defaults();
            this.types = Options.Types();
            this.filters = Options.Filters();
            this.values = new Dictionary<string, TextBox>();

            var help = Options.Help();
            int row = 0;

            var ordered = Options.Ordered();
            foreach (var key in ordered)
            {
                ++row;
                var def = new RowDefinition();
                this.grid.RowDefinitions.Add(def);
                if (key == "filenames")
                {
                    def.MinHeight = 20.0;
                }
                else
                {
                    def.Height = new GridLength(10.0, GridUnitType.Auto);
                }

                var label = new TextBlock();
                label.Name = key;
                label.ToolTip = help[key];
                label.Text = key;
                label.Margin = new Thickness(10.0);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                this.grid.Children.Add(label);

                var value = new TextBox();
                value.Name = key;
                value.ToolTip = help[key];
                value.Text = this.options[key];
                value.MinWidth = 120.0;
                value.TextChanged += this.Value_TextChanged;
                Grid.SetColumn(value, 1);
                Grid.SetRow(value, row);
                this.grid.Children.Add(value);
                this.values[key] = value;
                if (key == "filenames")
                {
                    value.AcceptsReturn = true;
                    value.TextWrapping = TextWrapping.Wrap;
                }

                if (this.types.ContainsKey(key))
                {
                    var browse = new Button();
                    browse.Name = key;
                    browse.ToolTip = help[key];
                    browse.Content = "Browse...";
                    browse.Click += this.Browse_Click;
                    Grid.SetColumn(browse, 2);
                    Grid.SetRow(browse, row);
                    this.grid.Children.Add(browse);
                }
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var name = ((System.Windows.Controls.Button)e.Source).Name;

            var type = this.types[name];
            if (type == Options.Browse.fileOut)
            {
                // Configure save file dialog box
                var dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Filter = this.filters[name];
                var result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.values[name].Text = dlg.FileName;
                }
            }
            else if (type == Options.Browse.fileIn)
            {
                // Configure save file dialog box
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Filter = this.filters[name];
                dlg.Multiselect = true;
                var result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var selection = this.values[name].Text;
                    foreach (var item in dlg.FileNames)
                    {
                        selection += string.Format("{0}\n", item);
                    }

                    this.values[name].Text = selection;
                }
            }
            else if (type == Options.Browse.directoryIn)
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                var result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.values[name].Text = dlg.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            string filePath;
            System.IO.StreamWriter logFile;
            this.OpenLog(out filePath, out logFile);

            try
            {
                this.view.Read(logFile, this.options, this.options["filenames"].Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
                this.view.Write(logFile, this.options);
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message, "DeepEnds", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.CloseLog(filePath, logFile);
        }

        private void OpenLog(out string filePath, out System.IO.StreamWriter file)
        {
            var path = System.IO.Path.GetTempPath();
            filePath = System.IO.Path.Combine(path, "deepends.txt");
            file = new System.IO.StreamWriter(filePath);
        }

        private void CloseLog(string filePath, System.IO.StreamWriter file)
        {
            file.Close();

            var p = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo("devenv.exe", "/edit " + filePath);
            p.StartInfo = startInfo;
            p.Start();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Command_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath;
                System.IO.StreamWriter file;
                this.OpenLog(out filePath, out file);

                var message = System.Reflection.Assembly.GetAssembly(typeof(View)).Location;
                if (message.Contains(" "))
                {
                    file.Write("\"");
                    file.Write(message);
                    file.Write("\"");
                }
                else
                {
                    file.Write(message);
                }

                var defaults = Options.Defaults();
                var ordered = Options.Ordered();
                foreach (var key in this.options.Keys)
                {
                    if (!ordered.Contains(key) || key == "filenames")
                    {
                        continue;
                    }

                    var val = this.options[key];
                    if (val == defaults[key])
                    {
                        continue;
                    }

                    file.Write(" ");
                    file.Write(key);
                    file.Write("=");

                    if (val.Contains(" "))
                    {
                        file.Write("\"");
                        file.Write(val);
                        file.Write("\"");
                    }
                    else
                    {
                        file.Write(val);
                    }
                }

                file.Write(" ");
                file.WriteLine(this.options["filenames"].Replace('\n', ' '));

                this.CloseLog(filePath, file);
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message, "DeepEnds", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = ((System.Windows.Controls.TextBox)e.Source).Name;
            this.options[name] = this.values[name].Text;
        }
    }
}