using Xunit;
using LessIO;

namespace LessMsi.Tests
{
    
    public class MiscTests : TestBase
    {
		[Fact]
		public void InputPathWithSpace()
		{
			ExtractAndCompareToMaster("Path With Spaces\\spaces example.msi");
		}

        [Fact]
        public void LongExtractionPath()
        {
            var msiFileName = "python-2.7.3.msi";
            LessIO.Path outputDir = new LessIO.Path(@"long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping\long-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\toppinglong-directory-name\very\unusually\long\directory\name\with\cream\sugar\and\chocolate\topping");
            /* Since System.IO doesn't support long path names, supporting 
             comparison of output as is done for other tests is a big effort. 
             So we ignore output only for this test.
             As long as we don't get an error we're happy.
            */
            var returnFileEntryGraph = false;
            var actualFileEntries = ExtractFilesFromMsi(msiFileName, null, outputDir, returnFileEntryGraph);
        }

        /// <summary>
        /// Related to issue 28 (http://code.google.com/p/lessmsi/issues/detail?id=28).
        /// Where GUI extracted files differ from console extracted files.
        /// Seems console is probably right. If I don't pass in the file list from the GUI it matches GUI. So probably in GUI am doing something wrong.
        /// Negative: It seems they're both wrong. A dictionary/hashtable used in Wixtracts was using caseinsinitive lookups on MsiFile.File entries and in python msi some entries were differing only by case.
        /// </summary>
        [Fact]
        public void Python()
        {
            ExtractAndCompareToMaster("python-2.7.3.msi");
        }

		/// <summary>
		/// This is from issue 37 (http://code.google.com/p/lessmsi/issues/detail?id=37). Basically if you only checked some files in the UI this occured. Lame that I didn't have a test for it!
		/// </summary>
		[Fact]
		public void ExtractOnlySomeFiles()
		{
			var msiFileName = "ExtractOnlySomeFiles.msi";
			var testFilesToExtract = new string[] { "SampleSuiteExtensionTests.cs", "testOutputOptions.jpg", "NUnitTests.config" };
			var actualFileEntries = ExtractFilesFromMsi(msiFileName, testFilesToExtract);
			var expectedEntries = GetExpectedEntriesForMsi(msiFileName);
			AssertAreEqual(expectedEntries, actualFileEntries);
		}

		/// <summary>
		/// This test is for github issue 4: https://github.com/activescott/lessmsi/issues/4
		/// </summary>
		[Fact]
		public void ExtractFromExternalCab()
		{
			ExtractAndCompareToMaster("msi_with_external_cab.msi");
		}

		/// <summary>
		/// This test is for github issue 169: https://github.com/activescott/lessmsi/issues/169
		/// </summary>
		[Fact]
		public void ExtractFromExternalCabWithSourceDirAndOutputDirSame()
		{
			var msiFileName = "vcredist.msi";
            var cabFileName = "vcredis1.cab";
            // put the msi and cab into the output directory (as this is all about having the source dir and output dir be the same):
            var outputDir = GetTestOutputDir(Path.Empty, msiFileName);
            if (FileSystem.Exists(outputDir))
            {
                FileSystem.RemoveDirectory(outputDir, true);
            }
            FileSystem.CreateDirectory(outputDir);
            var msiFileOutputDir = Path.Combine(outputDir, msiFileName);
            FileSystem.Copy(GetMsiTestFile(msiFileName), msiFileOutputDir);
			FileSystem.Copy(GetMsiTestFile(cabFileName), Path.Combine(outputDir, cabFileName));

            // run test normally:
			var cleanOutputDirectoryBeforeExtracting = false;
            //var actualFileEntries = ExtractFilesFromMsi(msiFileName, null, outputDir, true, cleanOutputDirectoryBeforeExtracting);
            LessMsi.Msi.Wixtracts.ExtractFiles(msiFileOutputDir, outputDir.PathString);
            //  build actual file entries extracted
            var actualFileEntries = FileEntryGraph.GetActualEntries(outputDir.PathString, msiFileName);
            // this test has the original msi and cab as the first two entries and their times change. so we drop them here:
            actualFileEntries.Entries.RemoveRange(0, 2);
            // dump to actual dir (for debugging and updating tests)
            actualFileEntries.Save(GetActualOutputFile(msiFileName));
            var expectedEntries = GetExpectedEntriesForMsi(msiFileName);
			AssertAreEqual(expectedEntries, actualFileEntries);
		}

		/// <summary>
		/// This one demonstrates a problem were paths are screwed up. 
		/// Note that the output path ends up being SourceDir\SlikSvn\bin\Windows\winsxs\... and it should be just \windows\winsxs\...
		/// Actually many of them do, but this one ends up with such long paths that it causes an exception:
		/// 	"Error: System.IO.PathTooLongException: The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters."
		/// </summary>
        [Fact(Skip=@"This one demonstrates a problem were paths are screwed up. Note that the output path ends up being SourceDir\SlikSvn\bin\Windows\winsxs\... and it should be just \windows\winsxs\...")]
        public void SlikSvn()
        {
        	ExtractAndCompareToMaster("Slik-Subversion-1.6.6-x64.msi");
        }

    	/// <summary>
		/// from http://code.google.com/p/lessmsi/issues/detail?id=1
		/// 
		/// </summary>
		[Fact(Skip = "This seems to have to do with the fact that the two CABs in there are merged cabs. MSVBVM60.dll is split across the cabs and I can get it out now with merging supported, but the olepro32.dll file is in disk2.cab and I can't get out with libmspack. Neither can totalcommander. -scott")]
		public void VBRuntime()
		{
			ExtractAndCompareToMaster("VBRuntime.msi");
		}

		/// <summary>
		/// from https://github.com/activescott/lessmsi/issues/49
		/// 
		/// </summary>
		[Fact]
		public void EmptyCabinetFieldInMediaTable()
		{
			ExtractAndCompareToMaster("X86 Debuggers And Tools-x86_en-us.msi");
		}

		/// <summary>
		/// From https://github.com/activescott/lessmsi/issues/78
		/// </summary>
		[Fact]
		public void MissingStreamsTable()
		{
			ExtractAndCompareToMaster("putty-0.68-installer.msi");
		}

        /// <summary>
        /// From https://github.com/activescott/lessmsi/pull/88
        /// </summary>
        [Fact]
        public void MissingParentDirectoryEntry()
        {
            ExtractAndCompareToMaster("IviNetSharedComponents32_Fx20_1.3.0.msi");
        }
    }
}
