using System;
using System.Diagnostics;
using Xunit;
using LessIO;

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
                throw new Exception("FileEntryGraph entries are not the equal");
            }
        }

        protected FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull)
        {
            return ExtractFilesFromMsi(msiFileName, fileNamesToExtractOrNull, "");
        }

        protected FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull, string outputDir)
        {
            return ExtractFilesFromMsi(msiFileName, fileNamesToExtractOrNull, new Path(outputDir), true);
        }

        /// <summary>
        /// Extracts some or all of the files from the specified MSI and returns a <see cref="FileEntryGraph"/> representing the files that were extracted.
        /// </summary>
        /// <param name="msiFileName">The msi file to extract or null to extract all files.</param>
        /// <param name="fileNamesToExtractOrNull">The files to extract from the MSI or null to extract all files.</param>
        /// <param name="outputDir">A relative directory to extract output to or an empty string to use the default output directory.</param>
        /// <param name="skipReturningFileEntryGraph">True to return the <see cref="FileEntryGraph"/>. Otherwise null will be returned.</param>
        protected FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull, Path outputDir, bool returnFileEntryGraph)
        {
            outputDir = GetTestOutputDir(outputDir, msiFileName);

            if (FileSystem.Exists(outputDir))
            {
                FileSystem.RemoveDirectory(outputDir, true);
            }
            Debug.Assert(!FileSystem.Exists(outputDir), "Directory still exists!");
            FileSystem.CreateDirectory(outputDir);

            //ExtractViaCommandLine(outputDir, msiFileName);
            ExtractInProcess(msiFileName, outputDir.PathString, fileNamesToExtractOrNull);
            if (returnFileEntryGraph)
            {
                //  build actual file entries extracted
                var actualEntries = FileEntryGraph.GetActualEntries(outputDir.PathString, msiFileName);
                // dump to actual dir (for debugging and updating tests)
                actualEntries.Save(GetActualOutputFile(msiFileName));
                return actualEntries;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a suitable output directory for test data while running the test.
        /// If <paramref name="outputDir"/> is specified it is used and <paramref name="testNameOrMsiFileName"/> is ignored.
        /// Alternatively, <paramref name="testNameOrMsiFileName"/> is used to generate a test dir.
        /// </summary>
        /// <param name="outputDir">The output dir to use or <see cref="Path.Empty"/>.</param>
        /// <param name="testNameOrMsiFileName">
        /// A test name (often the name of a msi file under test) to use to generate a test dir when <paramref name="testNameOrMsiFileName"/> is not specified.
        /// </param>
        /// <returns></returns>
        protected Path GetTestOutputDir(Path outputDir, string testNameOrMsiFileName)
        {
            Path baseOutputDir = Path.Combine(AppPath, "MsiOutputTemp");
            if (outputDir.IsEmpty || !outputDir.IsPathRooted)
                outputDir = Path.Combine(baseOutputDir, "_" + testNameOrMsiFileName);
            else
                outputDir = Path.Combine(baseOutputDir, outputDir);
            return outputDir;
        }

        private void ExtractInProcess(string msiFileName, string outputDir, string[] fileNamesToExtractOrNull)
        {
            //LessMsi.Program.DoExtraction(GetMsiTestFile(msiFileName).FullName, outputDir);
            if (fileNamesToExtractOrNull == null)
            {	//extract everything:
                LessMsi.Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), outputDir, null, null);	
            }
            else
            {
                LessMsi.Msi.Wixtracts.ExtractFiles(GetMsiTestFile(msiFileName), outputDir, fileNamesToExtractOrNull);
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
		    var startInfo = new ProcessStartInfo(LessIO.Path.Combine(AppPath, "lessmsi.exe").PathString, commandlineArgs);
		    startInfo.RedirectStandardOutput = true;
		    startInfo.RedirectStandardError = true;
		    startInfo.UseShellExecute = false;
		    var p = Process.Start(startInfo);
		    bool exited = p.WaitForExit(1000*30);
		    if (!exited)
		    {
			    p.Kill();
                throw new Exception("Process did not exit for commandlineArgs:" + commandlineArgs);
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
			return LessMsi.Cli.Program.Main(args);
		}

		internal sealed class ExitCodeException : Exception
		{
			public ExitCodeException(int exitCode, string errorOutput, string consoleOutput)
				: base("lessmsi.exe returned an error code (" + exitCode + "). Error output was:\r\n" + errorOutput + "\r\nConsole output was:\r\n" + consoleOutput)
			{
				this.ExitCode = exitCode;
			}

			public int ExitCode { get; set; }
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

        private Path GetMsiTestFile(string msiFileName)
        {
            return Path.Combine(AppPath, "TestFiles", "MsiInput", msiFileName);
        }

        private Path GetExpectedOutputFile(string msiFileName)
        {
            return Path.Combine(AppPath, "TestFiles", "ExpectedOutput", msiFileName + ".expected.csv");
        }

        protected Path GetActualOutputFile(string msiFileName)
        {
            // strip any subdirectories here since some input msi files have subdirectories.
            msiFileName = Path.GetFileName(msiFileName); 
            var fi = Path.Combine(AppPath, msiFileName + ".actual.csv");
            return fi;
        }

        protected Path AppPath
        {
            get
            {
                var codeBase = new Uri(this.GetType().Assembly.CodeBase);
                var local = new Path(codeBase.LocalPath);
                return local.Parent;
            }
        }
    }
}