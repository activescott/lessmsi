using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LessMsi.Msi;
using Misc.IO;
using NUnit.Framework;

namespace LessMsi.Tests
{
    public class TestBase
    {
        [DebuggerHidden]
        protected void ExtractAndCompareToMaster(string msiFileName)
        {
            var actualFileEntries = ExtractFilesFromMsi(msiFileName, null);
            var expectedEntries = GetExpectedEntriesForMsi(msiFileName);
            AssertAreEqual(expectedEntries, actualFileEntries);
        }

        [DebuggerHidden]
        protected static void AssertAreEqual(FileEntryGraph expected, FileEntryGraph actual)
        {
            string msg;
            if (!FileEntryGraph.CompareEntries(expected, actual, out msg))
            {
                Assert.Fail(msg);
            }
        }

        /// <summary>
        /// Extracts some or all of the files from the specified MSI and returns a <see cref="FileEntryGraph"/> representing the files that were extracted.
        /// </summary>
        /// <param name="msiFileName">The msi file to extract or null to extract all files.</param>
        protected FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull)
        {
            //  build command line
            string outputDir = Path.Combine(AppPath, "MsiOutputTemp");
            outputDir = Path.Combine(outputDir, "_" + msiFileName);
            if (Directory.Exists(outputDir))
            {
                DeleteDirectoryRecursive(new DirectoryInfo(outputDir));
                Directory.Delete(outputDir, true);
            }
            Directory.CreateDirectory(outputDir);

            //ExtractViaCommandLine(outputDir, msiFileName);
            ExtractInProcess(msiFileName, outputDir, fileNamesToExtractOrNull);

            //  build actual file entries extracted
            var actualEntries = GetActualEntries(outputDir, msiFileName);
            // dump to actual dir (for debugging and updating tests)
            actualEntries.Save(GetActualOutputFile(msiFileName));
            return actualEntries;
        }

        private static void DeleteDirectoryRecursive(DirectoryInfo di)
        {
            foreach (var item in di.GetFileSystemInfos())
            {
                var file = item as FileInfo;
                if (file != null)
                {
                    //NOTE: This is why this method is necessary. Directory.Delete and File.Delete won't delete files that have ReadOnly attribute set. So we clear it here before deleting
                    if (file.IsReadOnly)
                        file.IsReadOnly = false;
                    file.Delete();
                }
                else
                {
                    var dir = (DirectoryInfo) item;
                    DeleteDirectoryRecursive(dir);
                }
            }
        }

        private void ExtractInProcess(string msiFileName, string outputDir, string[] fileNamesToExtractOrNull)
        {
            //LessMsi.Program.DoExtraction(GetMsiTestFile(msiFileName).FullName, outputDir);
            if (fileNamesToExtractOrNull == null)
            {	//extract everything:
                LessMsi.Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), new DirectoryInfo(outputDir), null, null);	
            }
            else
            {
                // convert them to MsiFile objects:
                var msiFiles = MsiFile.CreateMsiFilesFromMSI((string) GetMsiTestFile(msiFileName).FullName);
    			
                var msiFilesLookup = new Dictionary<string, LessMsi.Msi.MsiFile>(msiFiles.Length);
                Array.ForEach(msiFiles, f => msiFilesLookup.Add(f.File, f));

                var fileNamesToExtractAsMsiFiles = new List<LessMsi.Msi.MsiFile>();
                foreach (var fileName in fileNamesToExtractOrNull)
                {
                    var found = msiFilesLookup[fileName];
                    fileNamesToExtractAsMsiFiles.Add(found);
                }
                LessMsi.Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), new DirectoryInfo(outputDir), fileNamesToExtractAsMsiFiles.ToArray(), null);
            }
			
        }

        /// <summary>
        /// This is an "old" way and it is difficul to debug (since it runs test out of proc), but it works.
        /// </summary>
        private void ExtractViaCommandLine(string msiFileName, string outputDir, string[] filenamesToExtractOrNull)
        {
            string args = string.Format(" /x \"{0}\" \"{1}\"", GetMsiTestFile(msiFileName), outputDir);
            
            if (filenamesToExtractOrNull != null && filenamesToExtractOrNull.Length > 0)
                throw new NotImplementedException();

            //  exec & wait
            var startInfo = new ProcessStartInfo(Path.Combine(AppPath, "lessmsi.exe"), args);
            startInfo.RedirectStandardOutput=true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            var p = Process.Start(startInfo);
            bool exited = p.WaitForExit(1000*30);
            if (!exited)
            {
                p.Kill();
                Assert.Fail("Process did not exit for msi file " + msiFileName);
            }
            var consoleOutput = p.StandardOutput.ReadToEnd();
            
            if (p.ExitCode == 0)
                Debug.WriteLine(consoleOutput);
            else
            {
                var errorOutput = p.StandardError.ReadToEnd();
                throw new Exception("lessmsi.exe returned an error code (" + p.ExitCode + "). Error output was:\r\n" + errorOutput + "\r\nConsole output was:\r\n" +consoleOutput);
            }
        }

        /// <summary>
        /// Loads the expected entries for the specified MSI file from the standard location.
        /// </summary>
        /// <param name="forMsi">The msi filename (no path) to load entries for.</param>
        /// <returns>The <see cref="FileEntryGraph"/> representing the files that are expected to be extracted from the MSI.</returns>
        protected FileEntryGraph GetExpectedEntriesForMsi(string forMsi)
        {
            return FileEntryGraph.Load(GetExpectedOutputFile(forMsi), forMsi);
        }

        /// <summary>
        /// Gets a <see cref="FileEntryGraph"/> representing the files in the specified outputDir (where an MSI was extracted).
        /// </summary>
        private FileEntryGraph GetActualEntries(string outputDir, string forFileName)
        {
            var actualEntries = new FileEntryGraph(forFileName);
            var dir = new DirectoryInfo(outputDir);
            var dirsToProcess = new Stack<DirectoryInfo>();
            dirsToProcess.Push(dir);
            while (dirsToProcess.Count > 0)
            {
                dir = dirsToProcess.Pop();
                foreach (var file in dir.GetFiles())
                {
                    actualEntries.Add(new FileEntry(file, outputDir));
                }
                foreach (var subDir in dir.GetDirectories())
                {
                    dirsToProcess.Push(subDir);
                }
            }
            return actualEntries;
        }

        private FileInfo GetMsiTestFile(string msiFileName)
        {
            return new FileInfo(PathEx.Combine(AppPath, "TestFiles", "MsiInput", msiFileName));
        }

        private FileInfo GetExpectedOutputFile(string msiFileName)
        {
            return new FileInfo(PathEx.Combine(AppPath, "TestFiles", "ExpectedOutput", msiFileName + ".expected.csv"));
        }

        private FileInfo GetActualOutputFile(string msiFileName)
        {
            // strip any subdirectories here since some input msi files have subdirectories.
            msiFileName = Path.GetFileName(msiFileName); 
            var fi = new FileInfo(Path.Combine(AppPath, msiFileName + ".actual.csv"));
            return fi;
        }

        protected string AppPath
        {
            get
            {
                var codeBase = new Uri(this.GetType().Assembly.CodeBase);
                var local = Path.GetDirectoryName(codeBase.LocalPath);
                return local;
            }
        }
    }
}