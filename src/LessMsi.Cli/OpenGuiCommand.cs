using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NDesk.Options;

namespace LessMsi.Cli
{
	internal class OpenGuiCommand : LessMsiCommand
	{
		public override void Run(List<string> args)
		{
			if (args.Count < 2)
				throw new OptionException("You must specify the name of the msi file to open when using the o command.", "o");

			ShowGui(args);
		}

		public static void ShowGui(List<string> args)
		{
			var guiExe = Path.Combine(AppPath, "lessmsi-gui.exe");
			if (File.Exists(guiExe))
			{
				//should we wait for exit?
				if (args.Count > 0)
					Process.Start(guiExe, args[1]);
				else
					Process.Start(guiExe);
			}
		}

		private static string AppPath
		{
			get
			{
				var codeBase = new Uri(typeof(OpenGuiCommand).Assembly.CodeBase);
				var local = Path.GetDirectoryName(codeBase.LocalPath);
				return local;
			}
		}
	}
}
