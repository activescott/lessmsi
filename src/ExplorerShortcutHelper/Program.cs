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
// Copyright (c) 2004-2013 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Globalization;

namespace Willeke.Scott.ExplorerShortcutHelper
{
	class Program
	{
		static int Main(string[] args)
		{
			bool? isAddingKey = null;
			if (args != null && args.Length > 0)
			{
				var addOrRemoveString = args[0]; // add or remove
				if (!string.IsNullOrEmpty(addOrRemoveString))
				{
					if ("add".Equals(addOrRemoveString, StringComparison.InvariantCulture))
						isAddingKey = true;
					else if ("remove".Equals(addOrRemoveString, StringComparison.InvariantCulture))
						isAddingKey = false;
				}
			}

			if (!isAddingKey.HasValue)
				return CommandLineError("Invalid argument. Expected 'add' or 'remove'.");


			if (args.Length < 2)
				return CommandLineError("Invalid argument. Expected a unique command name.");

			string commandName = args[1];

			if (args.Length < 3)
				return CommandLineError("Invalid argument. Expected a file class.");

			var fileClass = args[2]; //e.g. "Msi.Package";

			if (isAddingKey.Value)
			{	// if we're adding a key we're expecting more arguments:

				if (args.Length < 3)
					return CommandLineError("Invalid argument. Expected caption.");

				var contextMenuCaption = args[3]; // e.g. "&Extract Files";
				var shellCommandToExecute = args[4]; //e.g. '\"' + GetExePath() + "\" /x \"%1\" \"%1_extracted\"";

				var message = string.Format(CultureInfo.InvariantCulture,
										"Adding a shortcut for '{0}' files that will execute the following command: '{1}'...",
										fileClass, shellCommandToExecute);

				Console.WriteLine(message);
				try
				{
					RegistryTool.RegisterFileVerb(commandName, fileClass, contextMenuCaption, shellCommandToExecute);
				}
				catch (Exception oops)
				{
					Console.WriteLine("Error adding shortcut menu: " + oops.ToString());
					return -1;
				}
				Console.WriteLine("Shortcut added.");
			}
			else
			{
				Console.WriteLine("Removing shortcut...");
				try
				{
					RegistryTool.UnRegisterFileVerb(commandName, fileClass);
				}
				catch (Exception oops)
				{
					Console.WriteLine("Error removing shortcut menu: " + oops.ToString());
					return -1;
				}
				Console.WriteLine("Shortcut removed.");
			}
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
			return 0;
		}

		private static int CommandLineError(string errorMessage)
		{
			Console.WriteLine("Adds or removes a shortcut menu item for a specific type of file in Windows Explorer.");
			Console.WriteLine();
			Console.WriteLine("Usage:  AddWindowsExplorerShortcut add|remove commandName fileClass [caption shellCommand]");
			Console.WriteLine();
			Console.WriteLine(errorMessage);
			return 100;
		}
	}

}
