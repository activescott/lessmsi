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
#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using LessMsi.Msi;

#endregion

namespace LessMsi.Cli
{
	public class Program
	{
		enum ConsoleReturnCode
		{
			Success=0,
			UnexpectedError=-1,
			InvalidCommandLineOption=-2,
			UnrecognizedCommand=-3
		}

        private const string TempFolderSuffix = "_temp";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		public static int Main(string[] args)
		{
			try
			{
                /** 
				 * See https://github.com/activescott/lessmsi/wiki/Command-Line for some use cases and docs on commandline parsing.
				 * See https://github.com/mono/mono/blob/master/mcs/tools/mdoc/Mono.Documentation/mdoc.cs#L54  for an example of using "commands" and "subcommands" with the NDesk.Options lib.
				 */

                ExtractCommand extractCommand = new ExtractCommand();

                var subcommands = new Dictionary<string, LessMsiCommand> {
                    {"o", new OpenGuiCommand()},
                    {"x", extractCommand},
                    {"xfo", extractCommand},
                    {"xfr", extractCommand},
                    {"/x", extractCommand},
                    {"l", new ListTableCommand()},
                    {"v", new ShowVersionCommand()},
                    {"h", new ShowHelpCommand()}
                };

                LessMsiCommand cmd;
                if (args.Length > 0 && subcommands.TryGetValue(args[0], out cmd))
                {
                    cmd.Run(new List<string>(args));
                    return (int)ConsoleReturnCode.Success;
                }
				else if (args.Length == 0)
				{
					OpenGuiCommand.ShowGui(new List<string>());
					return (int)ConsoleReturnCode.Success;
				}
                else
                {
                    ShowHelpCommand.ShowHelp("Unrecognized command");
                    return (int)ConsoleReturnCode.UnrecognizedCommand;
                }
			}
			catch (NDesk.Options.OptionException oe)
			{
				ShowHelpCommand.ShowHelp(oe.Message);
				return (int) ConsoleReturnCode.InvalidCommandLineOption;
			}
			catch (Exception eCatchAll)
			{
				ShowHelpCommand.ShowHelp(eCatchAll.ToString());
				
				return (int) ConsoleReturnCode.UnexpectedError;
			}
		}

        /// <summary>
        /// Extracts all files contained in the specified .msi file into the specified output directory.
        /// </summary>
        /// <param name="msiFileName">The path of the specified MSI file.</param>
        /// <param name="outDirName">The directory to extract to. If empty it will use the current directory.</param>
        /// <param name="filesToExtract">The files to be extracted from the msi. If empty all files will be extracted.</param>
        /// /// <param name="extractionMode">Enum value for files extraction without folder structure</param>
        public static void DoExtraction(string msiFileName, string outDirName, List<string> filesToExtract, ExtractionMode extractionMode)
        {
            msiFileName = EnsureAbsolutePath(msiFileName);

            if (string.IsNullOrEmpty(outDirName))
            {
                outDirName = Path.GetFileNameWithoutExtension(msiFileName);
            }

            msiFileName = EnsureFileRooted(msiFileName);
            outDirName = EnsureFileRooted(outDirName);

            var msiFile = new LessIO.Path(msiFileName);

            Console.WriteLine("Extracting \'" + msiFile + "\' to \'" + outDirName + "\'.");

            if (isExtractionModeFlat(extractionMode))
            {
                string tempOutDirName = $"{outDirName}{TempFolderSuffix}";
                Wixtracts.ExtractFiles(msiFile, tempOutDirName, filesToExtract.ToArray(), PrintProgress);

                var fileNameCountingDict = new Dictionary<string, int>();

                outDirName += "\\";
                Directory.CreateDirectory(outDirName);
                copyFilesInFlatWay(tempOutDirName, outDirName, extractionMode, fileNameCountingDict);
                Directory.Delete(tempOutDirName, true);
            }
            else
            {
                Wixtracts.ExtractFiles(msiFile, outDirName, filesToExtract.ToArray(), PrintProgress);
            }
        }

        private static bool isExtractionModeFlat(ExtractionMode extractionMode)
        {
            return extractionMode == ExtractionMode.RenameFlatExtraction || extractionMode == ExtractionMode.OverwriteFlatExtraction;
        }

        private static void copyFilesInFlatWay(string sourceDir, string targetDir, ExtractionMode extractionMode, Dictionary<string, int> fileNameCountingDict)
        {
            var allFiles = Directory.GetFiles(sourceDir);

            foreach (var filePath in allFiles)
            {
                string fileSuffix = string.Empty;
                string fileName = Path.GetFileName(filePath);

                if (extractionMode == ExtractionMode.RenameFlatExtraction)
                {
                    if (fileNameCountingDict.ContainsKey(fileName))
                    {
                        fileSuffix = $"_{fileNameCountingDict[fileName]}";
                        fileNameCountingDict[fileName]++;
                    }
                    else
                    {
                        fileNameCountingDict.Add(fileName, 1);
                    }
                }

                var outputPath = $"{targetDir}{Path.GetFileNameWithoutExtension(filePath)}{fileSuffix}{Path.GetExtension(filePath)}";

                File.Copy(filePath, outputPath, extractionMode == ExtractionMode.OverwriteFlatExtraction);
            }

            var allFolders = Directory.GetDirectories(sourceDir);
            foreach (var directory in allFolders)
            {
                copyFilesInFlatWay(directory, targetDir, extractionMode, fileNameCountingDict);
            }
        }

        private static void PrintProgress(IAsyncResult result)
        {
            var progress = result as Wixtracts.ExtractionProgress;
            if (progress == null || string.IsNullOrEmpty(progress.CurrentFileName))
                return;

            Console.WriteLine(string.Format("{0}/{1}\t{2}", progress.FilesExtractedSoFar + 1, progress.TotalFileCount, progress.CurrentFileName));
        }

        private static string EnsureFileRooted(string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }

            return Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        private static string EnsureAbsolutePath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }

            return Path.GetFullPath(filePath);
        }
    }
}