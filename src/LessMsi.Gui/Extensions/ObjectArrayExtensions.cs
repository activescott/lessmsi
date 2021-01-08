using System;

namespace LessMsi.Gui.Extensions
{
	public static class ObjectArrayExtensions
	{
		public static bool Contains(this object[] objects, string needle, StringComparison comparisonType)
		{
			foreach(object obj in objects)
			{
				if (obj.ToString().Contains(needle, comparisonType))
				{
					return true;
				}
			}
			return false;
		}
	}
}
