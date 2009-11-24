using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace LessMsi.Tests
{
    [TestFixture]
    public class ConsoleExtractionTests
    {
        [Test]
        public void NUnit()
        {
            string msiFileName = "NUnit-2.5.2.9222.msi";

            var actualFileEntries = ExtractFilesFromMsi(msiFileName);
            var expectedEntries = GetExpectedEntries(msiFileName);
            AssertAreEqual(expectedEntries, actualFileEntries);
        }

        [Test]
        public void SlikSvn()
        {
            string msiFileName = "Slik-Subversion-1.6.6-x64.msi";

            var actualFileEntries = ExtractFilesFromMsi(msiFileName);
            var expectedEntries = GetExpectedEntries(msiFileName);
            AssertAreEqual(expectedEntries, actualFileEntries);
        }

        [DebuggerHidden]
        private static void AssertAreEqual(FileEntryGraph expected, FileEntryGraph actual)
        {
            string msg;
            if (!FileEntryGraph.CompareEntries(expected, actual, out msg))
            {
                Assert.Fail(msg);
            }
        }

        #region Testing Helper Methods
        /// <summary>
        /// Extracts all files from the specified MSI and returns a <see cref="FileEntryGraph"/> representing the files that were extracted.
        /// </summary>
        /// <param name="msiFileName">The msi file to extract.</param>
        private FileEntryGraph ExtractFilesFromMsi(string msiFileName)
        {
            // extract all:
            //  build command line
            string outputDir = Path.Combine(AppPath, "MsiOutputTemp");
            outputDir = Path.Combine(outputDir, "_" + msiFileName);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);
            string args = string.Format(" /x \"{0}\" \"{1}\"", GetMsiTestFile(msiFileName), outputDir);

            //  exec & wait
            var startInfo = new ProcessStartInfo(Path.Combine(AppPath, "lessmsi.exe"), args);
            startInfo.RedirectStandardOutput=true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            var p = Process.Start(startInfo);
            bool exited = p.WaitForExit(100*30);
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

            //  build actual file entries extracted
            var actualEntries = GetActualEntries(outputDir, msiFileName);
            // dump to actual dir (for debugging and updating tests)
            actualEntries.Save(GetActualOutputFile(msiFileName));
            return actualEntries;
        }

        /// <summary>
        /// Loads the expected entries for the specified MSI file from the standard location.
        /// </summary>
        /// <param name="forMsi">The msi filename (no path) to load entries for.</param>
        /// <returns>The <see cref="FileEntryGraph"/> representing the files that are expected to be extracted from the MSI.</returns>
        private FileEntryGraph GetExpectedEntries(string forMsi)
        {
            return FileEntryGraph.Load(GetExpectedOutputFile(forMsi), forMsi);
        }

        /// <summary>
        /// Gets a <see cref="FileEntryGraph"/> representing the files in the specified outputDir (where an MSI was extracted).
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="forFileName"></param>
        /// <returns></returns>
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
            return new FileInfo(Path.Combine(AppPath, msiFileName + ".actual.csv"));
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

        static class PathEx
        {
            public static string Combine(params string[] pathParts)
            {
                if (pathParts.Length < 2)
                    throw new ArgumentException("Expected at least two parts to combine.");

                var output = Path.Combine(pathParts[0], pathParts[1]);
                for (var i = 2; i < pathParts.Length; i++)
                {
                    output = Path.Combine(output, pathParts[i]);
                }
                return output;
            }
        }
        #endregion
    }
}
