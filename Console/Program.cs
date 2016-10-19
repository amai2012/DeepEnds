//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Zebedee Mason">
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

namespace DeepEnds.Console
{
    using System.Collections.Generic;

    class Program
    {
        private View view;
        private List<string> inputFiles;
        private Dictionary<string, string> options;

        Program()
        {
            this.view = new View();
            inputFiles = new List<string>();
            options = View.Options();
        }

        private void Parse(string[] args)
        {
            foreach (var arg in args)
            {
                var bit = arg.Replace("\"", string.Empty);

                var i = arg.IndexOf("=");
                if (i == -1)
                {
                    inputFiles.Add(arg);
                    continue;
                }

                options[arg.Substring(0, i)] = arg.Substring(i + 1);
            }
        }

        private bool UnsupportedExtension()
        {
            foreach (var fileName in this.inputFiles)
            {
                var ext = System.IO.Path.GetExtension(fileName);
                if (ext == ".vcxproj" || ext == ".csproj" || ext == ".vbproj" || ext == ".exe" || ext == ".dll" || ext == ".sln" || ext == ".xml")
                    continue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        private void Read()
        {
            this.view.Read(this.options, this.inputFiles.ToArray());

            System.Console.Write(this.view.Messages.ToString());
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        private void Write()
        {
            this.view.Write(this.options);
        }


        static int Main(string[] args)
        {
            System.Console.WriteLine(@"    +###############################,   
  #,                                 #` 
 #                                    @ 
 ,                              `;     @
+                              .###    #
#                              ####    #
#                             ####@    #
#                             ####     #
#                            @###:     #
#                            ####      #
#                           ####       #
#                          '###@       #
#                          ####        #
#                         @###'        #
#                       `#####         #
#                      @#####`         #
#                    +######@          #
#           ###    `########           #
#          #####  @#######@            #
#          ##### @#######              #
#          ##### ######+               #
#           ### #####@                 #
#              :####,                  #
#              ###@                    #
#             ###@                     #
#             ###                      #
#            @###                      #
#           `###                       #
#           ###,                       #
#          '###                        #
#          ###                         #
#          @#@                         #
#                                      #
#   `'#####;,+#####:,#####@:,#####@,   #
#   ,###############################   #
#   ,##@+@#####@+@######+@######+###   #
,   `       ,      `.      `.      .   #
 #                                    :.
  @                                  :@ 
   @#################################.  

Dive into architecture with DeepEnds

");

            var prog = new Program();
            prog.Parse(args);
            if (prog.UnsupportedExtension() || args.Length == 0)
            {
                System.Console.WriteLine("DeepEnds command line application for batch execution");
                System.Console.WriteLine("Usage:");
                System.Console.WriteLine("  DeepEnds.Console.exe [report] [graph] [source] filenames");
                System.Console.WriteLine("  where report is of the form 'report=report.html'");
                System.Console.WriteLine("        graph is of the form 'graph=graph.dgml'");
                System.Console.WriteLine("        source is (a directory) of the form 'source=C:\\...'");
                System.Console.WriteLine("        filenames is a list of xml, sln, csproj, vcxproj, vbproj, dll and exe files");
                System.Console.WriteLine("  for parsing Doxygen XML output supply one xml file and set source");
                return 1;
            }

            prog.Read();
            prog.Write();

            return 0;
        }
    }
}
