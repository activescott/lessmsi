using System.Collections.Generic;

namespace LessMsi.Cli
{
	internal abstract class LessMsiCommand
	{
		public abstract void Run(List<string> args);
	}
}
