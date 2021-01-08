using System;

namespace LessMsi.Gui.Extensions
{
	public static class StringExtensions
	{
		public static bool Contains(this string src, string needle, StringComparison comparisonType)
		{
			return src.IndexOf(needle, comparisonType) >= 0;
		}
	}
}
