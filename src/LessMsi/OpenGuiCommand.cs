using System;
using System.Collections.Generic;

namespace LessMsi
{
	internal class OpenGuiCommand : LessMsiCommand
	{
		public override void Run(List<string> args)
		{
			if (args.Count < 2)
				throw new OptionException("You must specify the name of the msi file to open when using the o command.", "o");
			Program.LaunchForm(args[1]);
		}
	}
}