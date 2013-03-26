using System;
using System.Collections.Generic;

namespace LessMsi
{
	internal class ShowHelpCommand : LessMsiCommand
	{
		public override void Run(List<string> args)
		{
			ShowHelp("");
		}

		public static void ShowHelp(string errorMessage)
		{
			string helpString =
				@"Usage:
lessmsi <command> [options] <msi_name> [<path_to_extract\>] [file_names]

Commands:
  x  Extracts all or specified files from the specified msi_name.	
  l  Lists the contents of the specified msi table as CSV to stdout.
  v  Lists the value of the ProductVersion Property in the msi 
     (typically this is the version of the MSI).
  o  Opens the specified msi_name in the GUI.
  h  Shows this help page.

For more information see https://code.google.com/p/lessmsi/wiki/CommandLine
";
			if (!string.IsNullOrEmpty(errorMessage))
			{
				helpString = "\r\nError: " + errorMessage + "\r\n\r\n" + helpString;
			}
			Console.WriteLine(helpString);
		}
	}
}