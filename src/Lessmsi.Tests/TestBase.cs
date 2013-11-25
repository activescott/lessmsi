using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace LessMsi.Tests
{
    using Misc.IO;

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
            if (FileEntryGraph.CompareEntries(expected, actual, out msg)) return;
            Assert.Fail(msg);
        }

        /// <summary>
        /// Extracts some or all of the files from the specified MSI and returns a <see cref="FileEntryGraph"/> representing the files that were extracted.
        /// </summary>
        /// <param name="msiFileName">The msi file to extract or null to extract all files.</param>
        protected FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull)
        {
            //  build command line
            var outputDir = Path.Combine(AppPath, "MsiOutputTemp");
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
            var actualEntries = FileEntryGraph.GetActualEntries(outputDir, msiFileName);
            // dump to actual dir (for debugging and updating tests)
            actualEntries.Save(GetActualOutputFile(msiFileName));
            return actualEntries;
        }

        protected static void DeleteDirectoryRecursive(DirectoryInfo di)
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
                Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), new DirectoryInfo(outputDir), null, null);	
            }
            else
            {
                Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), new DirectoryInfo(outputDir), fileNamesToExtractOrNull);
            }
			
        }

        /// <summary>
        /// This is an "old" way and it is difficul to debug (since it runs test out of proc), but it works.
        /// </summary>
        private void ExtractViaCommandLine(string msiFileName, string outputDir, string[] filenamesToExtractOrNull)
        {
	        var args = string.Format(" /x \"{0}\" \"{1}\"", GetMsiTestFile(msiFileName), outputDir);
			if (filenamesToExtractOrNull != null && filenamesToExtractOrNull.Length > 0)
				throw new NotImplementedException();
	        string consoleOutput;
			RunCommandLine(args, out consoleOutput);
        }



		protected int RunCommandLine(string commandlineArgs)
		{
			string temp;
			return RunCommandLine(commandlineArgs, out temp);

		}

		/// <summary>
		/// Runs lessmsi.exe via commandline.
		/// </summary>
		/// <param name="commandlineArgs">The arguments passed to lessmsi.exe</param>
		/// <param name="consoleOutput">The console output.</param>
		/// <returns>The exe return code.</returns>
	    protected int RunCommandLine(string commandlineArgs, out string consoleOutput)
	    {
		    //  exec & wait
		    var startInfo = new ProcessStartInfo(Path.Combine(AppPath, "lessmsi.exe"), commandlineArgs)
		        {
		            RedirectStandardOutput = true,
		            RedirectStandardError = true,
		            UseShellExecute = false
		        };
		    var p = Process.Start(startInfo);
		    var exited = p.WaitForExit(1000*30);
		    if (!exited)
		    {
			    p.Kill();
			    Assert.Fail("Process did not exit for commandlineArgs:" + commandlineArgs);
		    }
		    consoleOutput = p.StandardOutput.ReadToEnd();

		    if (p.ExitCode == 0)
			    Debug.WriteLine(consoleOutput);
		    else
		    {
			    var errorOutput = p.StandardError.ReadToEnd();
			    throw new ExitCodeException(p.ExitCode, errorOutput, consoleOutput);
		    }
			return p.ExitCode;
	    }

		/// <summary>
		/// Same as <see cref="RunCommandLine"/>, is useful for debugging.
		/// </summary>
		/// <param name="commandLineArgs"></param>
		protected int RunCommandLineInProccess(string commandLineArgs)
		{
			//NOTE: Obviously oversimplified splitting of args. 
			var args = commandLineArgs.Split(' ');
			for (var i = 0; i < args.Length; i++ )
			{
				args[i] = args[i].Trim('\"');
			}
			return Program.Main(args);
		}

		internal sealed class ExitCodeException : Exception
		{
			public ExitCodeException(int exitCode, string errorOutput, string consoleOutput)
				: base("lessmsi.exe returned an error code (" + exitCode + "). Error output was:\r\n" + errorOutput + "\r\nConsole output was:\r\n" + consoleOutput)
			{
				ExitCode = exitCode;
			}

		    private int ExitCode { get; set; }
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

        private FileInfo GetMsiTestFile(string msiFileName)
        {
            return new FileInfo(PathEx.Combine(AppPath, "TestFiles", "MsiInput", msiFileName));
        }

        private FileInfo GetExpectedOutputFile(string msiFileName)
        {
            return new FileInfo(PathEx.Combine(AppPath, "TestFiles", "ExpectedOutput", msiFileName + ".expected.csv"));
        }

        protected FileInfo GetActualOutputFile(string msiFileName)
        {
            // strip any subdirectories here since some input msi files have subdirectories.
            msiFileName = Path.GetFileName(msiFileName); 
            var fi = new FileInfo(Path.Combine(AppPath, msiFileName + ".actual.csv"));
            return fi;
        }

        private string AppPath
        {
            get
            {
                var codeBase = new Uri(GetType().Assembly.CodeBase);
                var local = Path.GetDirectoryName(codeBase.LocalPath);
                return local;
            }
        }
    }
}