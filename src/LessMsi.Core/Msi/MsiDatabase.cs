// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2021 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
	/// <summary>
	/// Helper class for opening an MSI Database or MSI Patch file
	/// </summary>
	public static class MsiDatabase
	{
		/// <summary>
		/// Documented flag, unlisted in Microsoft.Tools.WindowsInstallerXml.Msi.OpenDatabase
		/// </summary>
		const uint MSIDBOPEN_PATCHFILE = 32;

		/// <summary>
		/// Create a Database object from either an .msi or .mso file
		/// </summary>
		/// <param name="msiDatabaseFilePath">The path to the database or patch file</param>
		/// <returns></returns>
		public static Database Create(LessIO.Path msiDatabaseFilePath)
		{
			try
			{
				return new Database(msiDatabaseFilePath.PathString, OpenDatabase.ReadOnly);
			}
			catch (System.IO.IOException)
			{
				// retry as patchfile (.msp)
				return new Database(msiDatabaseFilePath.PathString, OpenDatabase.ReadOnly | (OpenDatabase)MSIDBOPEN_PATCHFILE);
			}
		}
	}
}
