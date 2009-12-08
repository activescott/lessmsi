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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LessMsi.Msi;
using LessMsi.UI;

#endregion

namespace LessMsi
{
	public class Program
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int FreeConsole();

		private class Arguments
		{
			public string MsiFileName { get; set; }
			public string OutDirName { get; set; }
			/// <summary>
			/// 0==good
			/// </summary>
			internal int ErrorCode { get; set; }
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] argStrings)
		{
			try
			{
				Arguments args = ParseArguments(argStrings);
				if (args.ErrorCode != 0)
					return args.ErrorCode;

				if (!string.IsNullOrEmpty(args.MsiFileName))
				{
					DoExtraction(args.MsiFileName, args.OutDirName);
					return 0;
				}
				//Else continue down & show the UI
			}
			catch (Exception eCatchAll)
			{
			    ShowHelp(eCatchAll.ToString());
				return -1;
			}
			return LaunchForm("");
		}

		/// <summary>
		/// Extracts all files contained in the specified .msi file into the specified output directory.
		/// </summary>
		/// <param name="msiFileName">The path of the specified MSI file.</param>
		/// <param name="outDirName">The directory to extract to. If empty it will use the current directory.</param>
		public static void DoExtraction(string msiFileName, string outDirName)
		{
			EnsureFileRooted(ref msiFileName);
			EnsureFileRooted(ref outDirName);

			FileInfo msiFile = new FileInfo(msiFileName);
			DirectoryInfo outDir = new DirectoryInfo(outDirName);

			Console.WriteLine("Extracting \'" + msiFile + "\' to \'" + outDir + "\'.");

			Wixtracts.ExtractFiles(msiFile, outDir);
		}

		private static Arguments ParseArguments(string[] args)
		{
			var arguments = new Arguments();
			// Handle args:
			for (int i = 0; i < args.Length; i++)
			{
				if (args.Length < 1)
					continue;

				if (args[i][0] != '/' && args[i][0] != '-')
					continue;
				switch (args[i][1])
				{
					case 'x':
						{
							if (++i >= args.Length)
							{
								ShowHelp("Expected input file argument.");
								arguments.ErrorCode = -2;
								return arguments;
							}

							arguments.MsiFileName = args[i];
							
                            
							if (++i >= args.Length)
								arguments.OutDirName = Path.GetDirectoryName(arguments.MsiFileName);
							else
								arguments.OutDirName = args[i];
							break;
						}
				}
			}
			return arguments;
		}

        private static void ShowHelp(string errorMessage)
        {
            string helpString =
                @"Usage:
lessmsi [/x <msiFileName> [<outouptDir]]

/x <msiFileName>                    Extract contents of the 
                                    specified msi file into a 
                                    new directory in the same 
                                    directory as the msi.

/x <msiFileName> <outouptDir>       Extract files in the specified 
                                    msi file into a directory at the 
                                    same place as the .msi file.
";
            if (!string.IsNullOrEmpty(errorMessage))
            {
                helpString = "\r\nError: " + errorMessage + "\r\n\r\n" + helpString;
            }
            Console.WriteLine(helpString);
        }

	    private static void EnsureFileRooted(ref string sFileName)
		{
			if (!Path.IsPathRooted(sFileName))
				sFileName = Path.Combine(Directory.GetCurrentDirectory(), sFileName);
		}

		static int LaunchForm(string inputFile)
		{
            FreeConsole();
			Application.EnableVisualStyles();
            Application.DoEvents();// make sure EnableVisualStyles works.
            MainForm form = new MainForm(inputFile);
			Application.Run(form);
			return 0;
		}
	}
}