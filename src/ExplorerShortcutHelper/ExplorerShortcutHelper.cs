using System;
using System.Globalization;
using System.IO;

namespace Willeke.Scott.ExplorerShortcutHelper
{
    public static class ExplorerShortcutHelper
    {
        public static string GetExeFilePathByName<T>(string exeFileName)
        {
            return Path.Combine(GetThisExeFile<T>().Directory.FullName, exeFileName);
        }

        public static FileInfo GetThisExeFile<T>()
        {
            return new FileInfo(typeof(T).Module.FullyQualifiedName);
        }

        public static int UpdateRightMenuByArgs(string[] args)
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
            {   // if we're adding a key we're expecting more arguments:

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