using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LessMsi.Gui.Extensions
{
	internal static class MsiNativeMethods
	{
		[DllImport("msi.dll", CharSet = CharSet.Unicode, EntryPoint = "MsiSummaryInfoGetPropertyW", ExactSpelling = true)]
		internal static extern uint MsiSummaryInfoGetProperty(IntPtr summaryInfo, int property, out uint dataType, out int integerValue, ref System.Runtime.InteropServices.ComTypes.FILETIME fileTimeValue, StringBuilder stringValueBuf, ref int stringValueBufSize);
	}
}
