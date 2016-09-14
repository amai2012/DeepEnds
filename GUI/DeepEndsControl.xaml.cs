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
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DeepEndsControl.
    /// </summary>
    public partial class DeepEndsControl : UserControl
    {
        private DeepEnds.Console.View view;
        private bool read;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeepEndsControl"/> class.
        /// </summary>
        public DeepEndsControl()
        {
            this.InitializeComponent();

            this.view = new DeepEnds.Console.View();
            this.writeButton.IsEnabled = false;
            this.read = false;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void input_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sln"; // Default file extension
            dlg.Filter = "MS Visual Studio solution (.sln)|*.sln|C++ Project (.vcxproj)|*.vcxproj|C# Project (.csproj)|*.csproj|VB.NET Project (.vbproj)|*.vbproj|.NET assemblies (.dll)|*.dll|.NET executables (.exe)|*.exe"; // Filter files by extension
            dlg.Multiselect = true;

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result != true)
            {
                return;
            }

            var selection = inputFiles.Text;
            foreach (var item in dlg.FileNames)
            {
                selection += string.Format("{0}\n", item);
            }

            inputFiles.Text = selection;
            this.read = true;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void output_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".dgml"; // Default file extension
            dlg.Filter = "Directed Graph Markup Language (.dgml)|*.dgml|Report (.html)|*.html"; // Filter files by extension

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result != true)
            {
                return;
            }

            outputFile.Text = dlg.FileName;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void read_Click(object sender, RoutedEventArgs e)
        {
            var read = this.view.Read(this.inputFiles.Text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
            if (read)
            {
                this.writeButton.IsEnabled = this.outputFile.Text != string.Empty;
                this.read = true;
            }

            var path = System.IO.Path.GetTempPath();
            var filePath = System.IO.Path.Combine(path, "deepends.txt");
            var file = new System.IO.StreamWriter(filePath);
            file.Write(this.view.Messages.ToString());
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
        private void write_Click(object sender, RoutedEventArgs e)
        {
            this.view.Write(this.outputFile.Text);
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void inputFiles_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.read = false;
            this.writeButton.IsEnabled = false;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void outputFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.writeButton.IsEnabled = this.read && (this.outputFile.Text != string.Empty);
        }
    }
}