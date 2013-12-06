using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Misc.IO;
using NUnit.Framework;

namespace LessMsi.Tests
{
    [TestFixture]
    public class MiscTests : TestBase
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
		[Test]
		public void ExtractFromExternalCab()
		{
			ExtractAndCompareToMaster("msi_with_external_cab.msi");
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
    }
}
