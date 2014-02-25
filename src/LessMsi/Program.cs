// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LessMsi.Msi;
using LessMsi.UI;
using NDesk.Options;

#endregion

namespace LessMsi
{
	public class Program
	{
        // defines for commandline output
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
		
		enum ConsoleReturnCode
		{
			Success=0,
			UnexpectedError=-1,
			InvalidCommandLineOption=-2,
			UnrecognizedCommand=-3
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static int Main(string[] args)
		{
			var doShowUI = args.Length == 0;
			try
			{
				AttachConsole(ATTACH_PARENT_PROCESS);
				
				/** 
				 * See https://code.google.com/p/lessmsi/wiki/CommandLine for some use cases and docs on commandline parsing.
				 * See https://github.com/mono/mono/blob/master/mcs/tools/mdoc/Mono.Documentation/mdoc.cs#L54  for an example of using "commands" and "subcommands" with the NDesk.Options lib.
				 */

				var subcommands = new Dictionary<string, LessMsiCommand> {
					{"o", new OpenGuiCommand()},
					{"x", new ExtractCommand()},
					{"/x", new ExtractCommand()},
					{"l", new ListTableCommand()},
					{"v", new ShowVersionCommand()},
					{"h", new ShowHelpCommand()}
				};

				if (doShowUI)
				{
					LaunchForm("");
					return (int) ConsoleReturnCode.Success;
				}
				else
				{
					LessMsiCommand cmd;
					if (subcommands.TryGetValue(args[0], out cmd))
					{
						cmd.Run(new List<string>(args));
						return (int) ConsoleReturnCode.Success;
					}
					else
					{
						ShowHelpCommand.ShowHelp("Unrecognized command");
						return (int) ConsoleReturnCode.UnrecognizedCommand;
					}
				}
			}
			catch (NDesk.Options.OptionException oe)
			{
				ShowHelpCommand.ShowHelp(oe.Message);
				return (int) ConsoleReturnCode.InvalidCommandLineOption;
			}
			catch (Exception eCatchAll)
			{
				ShowHelpCommand.ShowHelp(eCatchAll.ToString());
				
				return (int) ConsoleReturnCode.UnexpectedError;
			}
			finally
			{
				if (!doShowUI)
					//ConsoleNewLine();
			}
		}

		private static void ConsoleNewLine()
		{
			try
			{
				// When using a winforms app with AttachConsole the app completes but there is no newline after the process stops. This gives the newline and looks normal from the console:
				SendKeys.SendWait("{ENTER}");
			}
			catch (Exception e)
			{
				Debug.Fail(e.ToString());
			}
		}

		/// <summary>
		/// Extracts all files contained in the specified .msi file into the specified output directory.
		/// </summary>
		/// <param name="msiFileName">The path of the specified MSI file.</param>
		/// <param name="outDirName">The directory to extract to. If empty it will use the current directory.</param>
		/// <param name="filesToExtract">The files to be extracted from the msi. If empty all files will be extracted.</param>
		public static void DoExtraction(string msiFileName, string outDirName, List<string> filesToExtract )
		{
			if (string.IsNullOrEmpty(outDirName))
				outDirName = Path.GetFileNameWithoutExtension(msiFileName);
			EnsureFileRooted(ref msiFileName);
			EnsureFileRooted(ref outDirName);

			var msiFile = new FileInfo(msiFileName);
			var outDir = new DirectoryInfo(outDirName);

			Console.WriteLine("Extracting \'" + msiFile + "\' to \'" + outDir + "\'.");

			Wixtracts.ExtractFiles(msiFile, outDir, filesToExtract.ToArray());
		}

	    private static void EnsureFileRooted(ref string sFileName)
		{
			if (!Path.IsPathRooted(sFileName))
				sFileName = Path.Combine(Directory.GetCurrentDirectory(), sFileName);
		}

		internal static int LaunchForm(string inputFile)
		{
			Application.EnableVisualStyles();
            Application.DoEvents();// make sure EnableVisualStyles works.
            MainForm form = new MainForm(inputFile);
			Application.Run(form);
			return 0;
		}
	}
}
