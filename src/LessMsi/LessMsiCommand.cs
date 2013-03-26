using System.Collections.Generic;
using System.Text;

namespace LessMsi
{
	internal abstract class LessMsiCommand
	{
		public abstract void Run(List<string> args);
	}
}
