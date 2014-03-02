using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
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
			var expectedOutput = @"Property,Value
Manufacturer,nunit.org
ProductCode,{3AD32EC5-806E-43A8-8757-76D05AD4677A}
ProductLanguage,1033
ProductName,NUnit 2.5.2
ProductVersion,2.5.2.9222
UpgradeCode,{009074FF-2CEC-4B0C-9951-B07186F9ED3A}
CMD_EXE,[!SystemFolder]cmd.exe
ARPCONTACT,Charlie Poole
ARPPRODUCTICON,nunit_icon.exe
ARPHELPLINK,http://lists.sourceforge.net/lists/listinfo/nunit-users
ARPREADME,http://nunit.org/?p=releaseNotes&r=2.5
ARPURLINFOABOUT,NUnit is a testing framework for all .NET languages
ARPURLUPDATEINFO,http://nunit.org?p=download
DefaultUIFont,WixUI_Font_Normal
WixUI_Mode,Mondo
WixUI_WelcomeDlg_Next,LicenseAgreementDlg
WixUI_LicenseAgreementDlg_Back,WelcomeDlg
WixUI_LicenseAgreementDlg_Next,SetupTypeDlg
WixUI_SetupTypeDlg_NextTypical,VerifyReadyDlg
WixUI_SetupTypeDlg_NextCustom,CustomizeDlg
WixUI_SetupTypeDlg_NextComplete,VerifyReadyDlg
WixUI_SetupTypeDlg_Back,LicenseAgreementDlg
WixUI_CustomizeDlg_BackChange,MaintenanceTypeDlg
WixUI_CustomizeDlg_BackCustom,SetupTypeDlg
WixUI_CustomizeDlg_BackFeatureTree,**shouldnt_happen**
WixUI_CustomizeDlg_Next,VerifyReadyDlg
WixUI_VerifyReadyDlg_BackCustom,CustomizeDlg
WixUI_VerifyReadyDlg_BackChange,CustomizeDlg
WixUI_VerifyReadyDlg_BackRepair,MaintenanceTypeDlg
WixUI_VerifyReadyDlg_BackRemove,MaintenanceTypeDlg
WixUI_VerifyReadyDlg_BackTypical,SetupTypeDlg
WixUI_VerifyReadyDlg_BackComplete,SetupTypeDlg
WixUI_MaintenanceWelcomeDlg_Next,MaintenanceTypeDlg
WixUI_MaintenanceTypeDlg_Change,CustomizeDlg
WixUI_MaintenanceTypeDlg_Repair,VerifyReadyDlg
WixUI_MaintenanceTypeDlg_Remove,VerifyReadyDlg
WixUI_MaintenanceTypeDlg_Back,MaintenanceWelcomeDlg
ErrorDialog,ErrorDlg
WixUIRMOption,UseRM
";

		    string consoleOutput;
			RunCommandLine("l -t Property TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi", out consoleOutput);
			Assert.AreEqual(expectedOutput, consoleOutput);
		}

		[Test]
		public void Version()
		{
			var expectedOutput = "2.5.2.9222" + Environment.NewLine;
			string consoleOutput;
			RunCommandLine("v TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi", out consoleOutput);
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

			var actualEntries = FileEntryGraph.GetActualEntries(actualOutDir.FullName, "Actual Entries");
	        var actualEntriesFile = GetActualOutputFile(testName);
	        actualEntries.Save(actualEntriesFile);
			//Console.WriteLine("Actual entries saved to " + actualEntriesFile.FullName);
			var expectedEntries = GetExpectedEntriesForMsi(testName);
            AssertAreEqual(expectedEntries, actualEntries);
        }

		#endregion
	}
}
