using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Misc.IO;
using NUnit.Framework;

namespace LessMsi.Tests
{
    [TestFixture]
    public class ConsoleExtractionTests
    {
		[Test]
		public void InputPathWithSpace()
		{
			ExtractAndCompareToMaster("Path With Spaces\\spaces example.msi");
		}

        [Test]
        public void NUnit()
        {
			ExtractAndCompareToMaster("NUnit-2.5.2.9222.msi");
        }

        /// <summary>
        /// Related to issue 28 (http://code.google.com/p/lessmsi/issues/detail?id=28).
        /// Where GUI extracted files differ from console extracted files.
        /// Seems console is probably right. If I don't pass in the file list from the GUI it matches GUI. So probably in GUI am doing something wrong.
        /// Negative: It seems they're both wrong. A dictionary/hashtable used in Wixtracts was using caseinsinitive lookups on MsiFile.File entries and in python msi some entries were differing only by case.
        /// </summary>
        [Test]
        public void Python()
        {
            ExtractAndCompareToMaster("python-2.7.3.msi");
        }

		/// <summary>
		/// This is from issue 37 (http://code.google.com/p/lessmsi/issues/detail?id=37). Basically if you only checked some files in the UI this occured. Lame that I didn't have a test for it!
		/// </summary>
		[Test]
		public void ExtraOnlySomeFiles()
		{
			var msiFileName = "ExtractOnlySomeFiles.msi";
			var testFilesToExtract = new string[] { "SampleSuiteExtensionTests.cs", "testOutputOptions.jpg", "NUnitTests.config_1.1" };
			var actualFileEntries = ExtractFilesFromMsi(msiFileName, testFilesToExtract);
			var expectedEntries = GetExpectedEntries(msiFileName);
			AssertAreEqual(expectedEntries, actualFileEntries);
		}

		/// <summary>
		/// This one demonstrates a problem were paths are screwed up. 
		/// Note that the output path ends up being SourceDir\SlikSvn\bin\Windows\winsxs\... and it should be just \windows\winsxs\...
		/// Actually many of them do, but this one ends up with such long paths that it causes an exception:
		/// 	"Error: System.IO.PathTooLongException: The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters."
		/// </summary>
        [Test]
		[Ignore(@"This one demonstrates a problem were paths are screwed up. Note that the output path ends up being SourceDir\SlikSvn\bin\Windows\winsxs\... and it should be just \windows\winsxs\...")]
        public void SlikSvn()
        {
        	ExtractAndCompareToMaster("Slik-Subversion-1.6.6-x64.msi");
        }

    	/// <summary>
		/// from http://code.google.com/p/lessmsi/issues/detail?id=1
		/// 
		/// </summary>
		[Test]
		[Ignore("This seems to have to do with the fact that the two CABs in there are merged cabs. MSVBVM60.dll is split across the cabs and I can get it out now with merging supported, but the olepro32.dll file is in disk2.cab and I can't get out with libmspack. Neither can totalcommander. -scott")]
		public void VBRuntime()
		{
			ExtractAndCompareToMaster("VBRuntime.msi");
		}

        #region Testing Helper Methods

		[DebuggerHidden]
		private void ExtractAndCompareToMaster(string msiFileName)
		{
			var actualFileEntries = ExtractFilesFromMsi(msiFileName, null);
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

        /// <summary>
        /// Extracts some or all of the files from the specified MSI and returns a <see cref="FileEntryGraph"/> representing the files that were extracted.
        /// </summary>
        /// <param name="msiFileName">The msi file to extract or null to extract all files.</param>
        private FileEntryGraph ExtractFilesFromMsi(string msiFileName, string[] fileNamesToExtractOrNull)
        {
            //  build command line
            string outputDir = Path.Combine(AppPath, "MsiOutputTemp");
            outputDir = Path.Combine(outputDir, "_" + msiFileName);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);

            //ExtractViaCommandLine(outputDir, msiFileName);
			ExtractInProcess(msiFileName, outputDir, fileNamesToExtractOrNull);

        	//  build actual file entries extracted
            var actualEntries = GetActualEntries(outputDir, msiFileName);
            // dump to actual dir (for debugging and updating tests)
            actualEntries.Save(GetActualOutputFile(msiFileName));
            return actualEntries;
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
				var msiFiles = LessMsi.Msi.MsiFile.CreateMsiFilesFromMSI(GetMsiTestFile(msiFileName).FullName);
    			
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
    	private void ExtractViaCommandLine(string outputDir, string msiFileName)
    	{
    		string args = string.Format(" /x \"{0}\" \"{1}\"", GetMsiTestFile(msiFileName), outputDir);

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
        private FileEntryGraph GetExpectedEntries(string forMsi)
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

        
        #endregion
    }
}
