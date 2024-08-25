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
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using LessMsi.Gui.Resources.Languages;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LessMsi.Gui
{
	internal partial class PreferencesForm : Form
	{
		public PreferencesForm()
		{
			InitializeComponent();

            this.Text = $"LessMSIerables {Strings.Preferences}";
        }

		private void btnOk_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cmdAddRemoveShortcut_Click(object sender, EventArgs e)
		{
			bool isAdding;

			if (sender == cmdAddShortcut)
				isAdding = true;
			else
				isAdding = false;

			/* FIX for http://code.google.com/p/lessmsi/issues/detail?id=11
			 * This code below is funky because apparently Win32 requires us to escape double quotes on the command line when passing them through the command line. 
			 * So we have to actually espcape the escape char here to make sure double quotes are properly escaped
			 * Explained more at http://bytes.com/topic/net/answers/745324-console-application-command-line-parameter-issue and http://msdn.microsoft.com/en-us/library/system.environment.getcommandlineargs.aspx
			 * 
			 * Also see https://github.com/activescott/lessmsi/wiki/Command-Line for commandline reference for the commands used below.
			 */
			const string escapedDoubleQuote = "\\" + "\"";
			
			var shellCommand = escapedDoubleQuote + GetLessMsiExeFile() + escapedDoubleQuote + " x " + escapedDoubleQuote + "%1" + escapedDoubleQuote;
			Debug.WriteLine("ShellCommand:[" + shellCommand + "]");
			AddRemoveShortcut(isAdding, "extract", "Msi.Package", "Lessmsi &Extract Files", shellCommand);

			/* Fix for https://code.google.com/p/lessmsi/issues/detail?id=6&sort=-id
			 */
			shellCommand = escapedDoubleQuote + GetLessMsiExeFile() + escapedDoubleQuote + " o " + escapedDoubleQuote + "%1" + escapedDoubleQuote;
			Debug.WriteLine("ShellCommand:[" + shellCommand + "]");
			AddRemoveShortcut(isAdding, "explore", "Msi.Package", "&Lessmsi Explore", shellCommand);
		}
		void AddRemoveShortcut(bool isAdding, string commandName, string fileClass, string caption, string shellCommand)
		{
			var thisExe = GetThisExeFile();
			string shortcutHelperExe = Path.Combine(thisExe.Directory.FullName, "AddWindowsExplorerShortcut.exe");
			if (!File.Exists(shortcutHelperExe))
			{
				MessageBox.Show(this,
								$"{Strings.File} '" + Path.GetFileNameWithoutExtension(shortcutHelperExe) +
								$"' {Strings.SameDirMassage} lessmsi-gui.exe.", 
								Strings.MissingFile, 
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
				return;
			}
			var newProcess = new Process();
			var info = new ProcessStartInfo("");
			info.FileName = shortcutHelperExe;
			info.UseShellExecute = true;
			info.ErrorDialog = true;
			info.ErrorDialogParentHandle = this.Handle;
			//AddWindowsExplorerShortcut add|remove commandName fileClass [caption shellCommand]
			var args = new StringBuilder();
			if (isAdding)
				args.Append("add");
			else
				args.Append("remove");
			args.Append(' ').Append(commandName);
			args.Append(' ').Append(fileClass);
			if (isAdding)
			{
				args.Append(" \"").Append(caption).Append("\"");
				args.Append(' ').Append('\"').Append(shellCommand).Append('\"');
			}
			info.Arguments = args.ToString();
			newProcess.StartInfo = info;
			newProcess.Start();
		}

		static string GetLessMsiExeFile()
		{
			return Path.Combine(GetThisExeFile().Directory.FullName, "lessmsi.exe");
		}

		static FileInfo GetThisExeFile()
		{
			return new FileInfo(typeof(PreferencesForm).Module.FullyQualifiedName);
		}
	}
}
