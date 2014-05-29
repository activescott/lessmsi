using System.Collections.Generic;
using System.Linq;
using NDesk.Options;

namespace LessMsi.Cli
{
	internal class ExtractCommand : LessMsiCommand
	{
		public override void Run(List<string> allArgs)
		{
			var args = allArgs.Skip(1).ToList();
			// "x msi_name [path_to_extract\] [file_names]+
			if (args.Count < 1)
				throw new OptionException("Invalid argument. Extract command must at least specify the name of an msi file.", "x");

			var i = 0;
			var msiFile = args[i++];
			var filesToExtract = new List<string>();
			var extractDir = "";
			if (i < args.Count)
			{
				if (args[i].EndsWith("\\"))
					extractDir = args[i];
				else
					filesToExtract.Add(args[i]);
			}
			while (++i < args.Count)
				filesToExtract.Add(args[i]);

			Program.DoExtraction(msiFile, extractDir.TrimEnd('\"'), filesToExtract);
		}
	}
}
