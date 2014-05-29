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
using System;
using System.Security;
using Microsoft.Win32;

namespace Willeke.Scott.ExplorerShortcutHelper
{
	internal class RegistryTool
	{
		/// <summary>
		/// Registers a verb for the specified windows explorer/shell file class.
		/// </summary>
		/// <param name="commandName">A unique name for your verb/command. It should be just different from any other applications command.</param>
		/// <param name="fileClassName">The file class for the file to register the verb for (see http://msdn.microsoft.com/en-us/library/bb776870(VS.85).aspx ).</param>
		/// <param name="contextMenuCaption">The caption for the verb.</param>
		/// <param name="shellCommandToExecute">The command to execute (as it would be executed in ShellExecute).</param>
		public static void RegisterFileVerb(string commandName, string fileClassName, string contextMenuCaption, string shellCommandToExecute)
		{
            string regRoot = fileClassName + @"\shell\" + commandName;
			var extractKey = OpenRegistryKeyWithAddedErrorInfo(regRoot);
			extractKey.SetValue("", contextMenuCaption);

			var commandKeyValue = OpenRegistryKeyWithAddedErrorInfo(regRoot + "\\command");
			var existingValue = commandKeyValue.GetValue("") as string;
			if (existingValue != null && existingValue.StartsWith(shellCommandToExecute))
				return;
			commandKeyValue.SetValue("", shellCommandToExecute);
		}

		/// <summary>
		/// Deletes/unregisteres a verb previously registered with <see cref="RegisterFileVerb"/>.
		/// </summary>
		/// <param name="commandName">See <see cref="RegisterFileVerb"/>.</param>
		/// <param name="fileClassName">See <see cref="RegisterFileVerb"/>.</param>
		public static void UnRegisterFileVerb(string commandName, string fileClassName)
		{
			var extractKey = OpenRegistryKeyWithAddedErrorInfo(fileClassName + @"\shell\");
			extractKey.DeleteSubKeyTree(commandName);
		}

		/// <summary>
		/// Opens a key with write permission and adds error information if it fails.
		/// </summary>
		private static RegistryKey OpenRegistryKeyWithAddedErrorInfo(string registryKeyName)
		{
			RegistryKey extractKey;
			try
			{
				extractKey = Registry.ClassesRoot.CreateSubKey(registryKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
			}
			catch (SecurityException eSecurity)
			{
				throw new SecurityException("You do not have the permissions required to create or write the registry key '" + registryKeyName + "'.", eSecurity);
			}
			catch (Exception eCatchAll)
			{
				throw new Exception("Unable to open registry key '" + registryKeyName + "'.", eCatchAll);
			}
			return extractKey;
		}

	}
}
