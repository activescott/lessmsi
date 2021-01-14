using Microsoft.Tools.WindowsInstallerXml.Msi;
using System;
using System.Text;

namespace LessMsi.Gui.Extensions
{
	public static class SummaryInformationExtensions
	{
		/// <summary>
		/// There is a bug in Wix where it does not correctly return data with the FILETIME type.
		/// This extension method manually retrieves the value, and converts it to a DateTime.
		/// </summary>
		public static object GetPropertyFileTime(this SummaryInformation summaryInfo, int index)
		{
			uint iDataType;
			int integerValue, stringValueBufSize = 0;
			System.Runtime.InteropServices.ComTypes.FILETIME fileTimeValue = new System.Runtime.InteropServices.ComTypes.FILETIME();
			StringBuilder stringValueBuf = new StringBuilder();

			uint result = MsiNativeMethods.MsiSummaryInfoGetProperty(summaryInfo.InternalHandle, index,
				out iDataType, out integerValue, ref fileTimeValue, stringValueBuf, ref stringValueBufSize);

			if (result != 0)
			{
				throw new ArgumentNullException();
			}

			switch ((Model.VT)iDataType)
			{
				case Model.VT.EMPTY:
					return string.Empty;
				case Model.VT.FILETIME:
					return DateTime.FromFileTime((((long)fileTimeValue.dwHighDateTime) << 32) | ((uint)fileTimeValue.dwLowDateTime));
				default:
					throw new ArgumentNullException();
			}
		}
	}
}
