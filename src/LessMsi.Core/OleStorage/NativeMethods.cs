using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LessMsi.OleStorage
{
	internal static class NativeMethods
	{
		[DllImport("ole32.dll")]
		internal static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)]string pwcsName);

	}
}
