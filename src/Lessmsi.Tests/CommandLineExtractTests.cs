using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using NDesk.Options;
using NUnit.Framework;

namespace LessMsi.Tests
{
    [TestFixture]
    public class CommandLineExtractTests : TestBase
    {
        [Test]
        public void Extract1Arg()
        {
			var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi";
			TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222");
        }

		[Test]
		public void Extract2Args()
		{
			var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Ex2Args\\";
			TestExtraction(commandLine, GetTestName(), "Ex2Args");
		}

	    [Test]
		public void Extract3Args()
		{
			var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Ex3\\ \"cs-money.build\" \"requiresMTA.html\"";
			TestExtraction(commandLine, GetTestName(), "Ex3");
		}

		[Test]
	    public void ExtractCompatibility1Arg()
		{
			var commandLine = @"/x TestFiles\MsiInput\NUnit-2.5.2.9222.msi";
			TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222");
		}

		[Test]
		public void ExtractCompatibility2Args()
		{
			var commandLine = @"/x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi ExtractCompatibility2Args\";
			TestExtraction(commandLine, GetTestName(), "ExtractCompatibility2Args");
		}

		[Test, ExpectedException(typeof(ExitCodeException))]
		public void BackwardCompatibilityParserNoMsiSpecifiedParser()
		{
			var commandLine = "/x";
			
			string consoleOutput;
			var exitCode = RunCommandLine(commandLine, out consoleOutput);
			Assert.AreEqual(3, exitCode);
		}

	    [Test]
		public void List()
		{
			var expectedOutput = "TODO";
			var consoleOutput = RunCommandLine("l -t Property TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi");
			Assert.AreEqual(expectedOutput, consoleOutput);
		}

		[Test]
		public void Version()
		{
			var expectedOutput = "2.5.2.9222";
			var consoleOutput = RunCommandLine("v TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi");
			Assert.AreEqual(expectedOutput, consoleOutput);
		}

		#region Helpers
		
        /// <summary>
        /// Returns the name of the calling method.
        /// </summary>
        private string GetTestName()
        {
	        var method = new StackFrame(1).GetMethod();
	        return method.Name;
		}


		private void TestExtraction(string commandLineArguments, string testName, string actualEntriesOutputDir)
		{
			TestExtraction(commandLineArguments, testName, actualEntriesOutputDir, false);
		}

        /// <summary>
        /// Executes the specified command. Assume working directory is TestFiles\MsiInput\ dir.
        /// </summary>
        /// <param name="commandLineArguments">The command line arguments (everything after the exe name).</param>
        /// <param name="testName">The name of hte test (used to formulate the expectedEntries output dir).</param>
        /// <param name="actualEntriesOutputDir">The output directory where the actual extraction is expected to occur.</param>
        private void TestExtraction(string commandLineArguments, string testName, string actualEntriesOutputDir, bool useInProcessForDebugging)
        {
			string consoleOutput;
	        var actualOutDir = new DirectoryInfo(actualEntriesOutputDir);
			if (actualOutDir.Exists)
				DeleteDirectoryRecursive(actualOutDir);
	        int exitCode;

			if (useInProcessForDebugging)
				exitCode = base.RunCommandLineInProccess(commandLineArguments);
			else
				exitCode = base.RunCommandLine(commandLineArguments, out consoleOutput);
			
			var actualEntries = FileEntryGraph.GetActualEntries(actualEntriesOutputDir, "Actual Entries");
	        var actualEntriesFile = GetActualOutputFile(testName);
	        actualEntries.Save(actualEntriesFile);
			Console.WriteLine("Actual entries saved to " + actualEntriesFile.FullName);
	        var expectedEntries = GetExpectedEntriesForMsi(testName);
            AssertAreEqual(expectedEntries, actualEntries);
        }

		#endregion
	}
}
