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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Willeke.Scott.ExplorerShortcutHelper
{
	class ExplorerShortcutAdder
    {
        static int Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                ExplorerShortcutHelper.UpdateRightMenuByArgs(args);
            }
            else
            {
                /* FIX for http://code.google.com/p/lessmsi/issues/detail?id=11
                 * This code below is funky because apparently Win32 requires us to escape double quotes on the command line when passing them through the command line. 
                 * So we have to actually espcape the escape char here to make sure double quotes are properly escaped
                 * Explained more at http://bytes.com/topic/net/answers/745324-console-application-command-line-parameter-issue and http://msdn.microsoft.com/en-us/library/system.environment.getcommandlineargs.aspx
                 * 
                 * Also see https://github.com/activescott/lessmsi/wiki/Command-Line for commandline reference for the commands used below.
                 */

                const string escapedDoubleQuote = "\\" + "\"";
                const string helperExeName = "AddWindowsExplorerShortcut.exe";

                var shellCommand = escapedDoubleQuote + ExplorerShortcutHelper.GetExeFilePathByName<ExplorerShortcutAdder>(helperExeName) + escapedDoubleQuote + " x " + escapedDoubleQuote + "%1" + escapedDoubleQuote;
                AddRemoveShortcut(true, "extract", "Msi.Package", "Lessmsi &Extract Files", shellCommand);

                /* Fix for https://code.google.com/p/lessmsi/issues/detail?id=6&sort=-id
                 */
                shellCommand = escapedDoubleQuote + ExplorerShortcutHelper.GetExeFilePathByName<ExplorerShortcutAdder>(helperExeName) + escapedDoubleQuote + " o " + escapedDoubleQuote + "%1" + escapedDoubleQuote;
                AddRemoveShortcut(true, "explore", "Msi.Package", "&Lessmsi Explore", shellCommand);
            }

            return 0;
        }

        static void AddRemoveShortcut(
            bool isAdding, 
            string commandName, 
            string fileClass, 
            string caption, 
            string shellCommand)
        {
            List<string> args = new List<string>();

            if (isAdding)
                args.Add("add");
            else
                args.Add("remove");

            args.Add(commandName);
            args.Add(fileClass);

            if (isAdding)
            {
                args.Add(caption);
                args.Add(shellCommand);
            }

            ExplorerShortcutHelper.UpdateRightMenuByArgs(args.ToArray());
        }
	}
}
