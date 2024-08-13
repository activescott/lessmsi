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
using System.Reflection.PortableExecutable;
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
        /// <param name="extractionMode">Enum value for files extraction without folder structure</param>
        /// <param name="architectureType">Enum value for architecture type to delete after files extraction</param>
        public static void DoExtraction(string msiFileName, string outDirName, List<string> filesToExtract, ExtractionMode extractionMode, ArchitectureType architectureType)
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

            if (architectureType != ArchitectureType.None)
            {
                deleteFilesInGivenArchitecture(outDirName, architectureType);
                removeFileNameSuffixInDirectory(outDirName, ".duplicate1");
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

        private static bool isPEFile(string filePath)
        {
            bool peFlag = true;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Check for MZ header
                    if (br.ReadUInt16() != 0x5A4D) // 'MZ' in little-endian
                    {
                        peFlag = false;
                    }
                    else
                    {
                        // Skip to the PE header offset location (0x3C)
                        fs.Seek(0x3C, SeekOrigin.Begin);
                        uint peHeaderOffset = br.ReadUInt32();

                        // Check for PE header signature
                        fs.Seek(peHeaderOffset, SeekOrigin.Begin);
                        if (br.ReadUInt32() != 0x00004550) // 'PE\0\0' in little-endian
                        {
                            peFlag = false;
                        }
                    }
                }
            }

            return peFlag;
        }

        private static ArchitectureType getArchitectureType(string filePath)
        {
            ArchitectureType architectureType = ArchitectureType.None;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new PEReader(stream))
                {
                    var headers = reader.PEHeaders;
                    switch (headers.PEHeader.Magic)
                    {
                        case PEMagic.PE32:
                            architectureType = ArchitectureType.X32;
                            break;
                        case PEMagic.PE32Plus:
                            architectureType = ArchitectureType.X64;
                            break;
                    }
                }
            }

            return architectureType;
        }

        private static void deleteFilesInGivenArchitecture(string directoryPath, ArchitectureType architectureType)
        {
            // Get all files in the current directory
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                // Check if current file is a PE file 
                if (isPEFile(file))
                {
                    // delete all files in given architecture
                    if (getArchitectureType(file) == architectureType)
                    {
                        File.Delete(file);
                    }
                }
            }

            // Get all subdirectories in the current directory
            string[] directories = Directory.GetDirectories(directoryPath);

            foreach (string directory in directories)
            {
                // Recursively call this method for each subdirectory
                deleteFilesInGivenArchitecture(directory, architectureType);
            }
        }

        private static void removeFileNameSuffixInDirectory(string directoryPath, string suffix)
        {
            // Get all files in the current directory
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                if (isPEFile(file))
                {
                    // Remove the suffix if it exists in the filename
                    removeFileNameSuffix(file, suffix);
                }
            }

            // Get all subdirectories in the current directory
            string[] directories = Directory.GetDirectories(directoryPath);

            foreach (string directory in directories)
            {
                // Recursively call this method for each subdirectory
                removeFileNameSuffixInDirectory(directory, suffix);
            }
        }

        private static void removeFileNameSuffix(string filePath, string suffix)
        {
            // Get the directory and filename separately
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            // Check if the file has the suffix
            if (fileName.EndsWith(suffix))
            {
                // Remove the suffix from the filename
                string newFileName = fileName.Substring(0, fileName.Length - suffix.Length);

                // Combine the new filename with the directory to get the new path
                string newFilePath = Path.Combine(directory, newFileName);

                // Rename the file by moving it to the new path
                File.Move(filePath, newFilePath);
            }
        }
    }
}