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
        private DeepEnds.Console.View view;
        private Dictionary<string, string> options;
        private Dictionary<string, string> types;
        private Dictionary<string, string> filters;
        private Dictionary<string, TextBox> values;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeepEndsControl"/> class.
        /// </summary>
        public DeepEndsControl()
        {
            this.InitializeComponent();

            this.view = new DeepEnds.Console.View();
            this.options = DeepEnds.Console.Options.Defaults();
            this.types = DeepEnds.Console.Options.Types();
            this.filters = DeepEnds.Console.Options.Filters();
            this.values = new Dictionary<string, TextBox>();

            var help = DeepEnds.Console.Options.Help();
            int row = 0;

            var ordered = DeepEnds.Console.Options.Ordered();
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
                label.Width = 50.0;
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

                if (this.filters.ContainsKey(key))
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
            if (type == "fileOut")
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
            else if (type == "fileIn")
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
            else if (type == "directoryIn")
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
            try
            {
                this.view.Read(this.options, this.options["filenames"].Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
                this.view.Write(this.options);
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message, "DeepEnds", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.ShowMessage(this.view.Messages.ToString());
        }

        private void ShowMessage(string message)
        {
            var path = System.IO.Path.GetTempPath();
            var filePath = System.IO.Path.Combine(path, "deepends.txt");
            var file = new System.IO.StreamWriter(filePath);
            file.Write(message);
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
                var message = System.Reflection.Assembly.GetAssembly(typeof(DeepEnds.Console.View)).Location;
                if (message.Contains(" "))
                {
                    message = string.Format("\"{0}\"", message);
                }

                var defaults = DeepEnds.Console.Options.Defaults();
                var ordered = DeepEnds.Console.Options.Ordered();
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

                    if (val.Contains(" "))
                    {
                        val = string.Format("\"{0}\"", val);
                    }

                    message = string.Format("{0} {1}={2}", message, key, val);
                }

                message = string.Format("{0} {1}", message, this.options["filenames"].Replace('\n', ' '));
                this.ShowMessage(message);
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