using System;
using System.Diagnostics;
using LessIO;
using Xunit;

namespace LessMsi.Tests
{
    // We use Test Collections to prevent files get locked by seperate threads: http://xunit.github.io/docs/running-tests-in-parallel.html
    [Collection("NUnit - 2.5.2.9222.msi")] // Since almost all of these use nunit we just put the whole class in the nunit collection.
    public class CommandLineExtractTests : TestBase
    {
        [Fact]
        public void Extract1Arg()
        {
            var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi";
            TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222", false);
        }

        [Fact]
        public void Extract1ArgRelativePath()
        {
            var commandLine = "x .\\TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi";
            // setting "NUnit-2.5.2.9222" as actualEntriesOutputDir value, since no other output dir specified in command line text
            TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222", false);
        }

        [Fact]
        public void FlatOverwriteExtract1Arg()
        {
            var commandLine = "xfo TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi";
            // setting "NUnit-2.5.2.9222" as actualEntriesOutputDir value, since no other output dir specified in command line text
            TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222", false, flatExtractionFlag: true);
        }

        [Fact]
        public void FlatRenameExtract1Arg()
        {
            var commandLine = "xfr TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi";
            // setting "NUnit-2.5.2.9222" as actualEntriesOutputDir value, since no other output dir specified in command line text
            TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222", false, flatExtractionFlag: true);
        }

        [Fact]
        public void Extract2Args()
        {
            var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Ex2Args\\";
            TestExtraction(commandLine, GetTestName(), "Ex2Args", false);
        }

        [Fact]
        public void Extract2ArgsRelativePath()
        {
            var commandLine = "x .\\TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Extract2ArgsRelativePath\\";
            TestExtraction(commandLine, GetTestName(), "Extract2ArgsRelativePath", false);
        }

        [Fact]
        public void FlatOverwriteExtract2Args()
        {
            var commandLine = "xfo TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi FlatOverwriteExtract2Args\\";
            TestExtraction(commandLine, GetTestName(), "FlatOverwriteExtract2Args", false, flatExtractionFlag: true);
        }

        [Fact]
        public void FlatRenameExtract2Args()
        {
            var commandLine = "xfr TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi FlatRenameExtract2Args\\";
            TestExtraction(commandLine, GetTestName(), "FlatRenameExtract2Args", false, flatExtractionFlag: true);
        }

        [Fact]
        public void Extract3Args()
        {
            var commandLine = "x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Ex3\\ \"cs-money.build\" \"requiresMTA.html\"";
            TestExtraction(commandLine, GetTestName(), "Ex3", false);
        }

        [Fact]
        public void Extract3ArgsRelativePath()
        {
            var commandLine = "x .\\TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi Extract3ArgsRelativePath\\ \"cs-money.build\" \"requiresMTA.html\"";
            TestExtraction(commandLine, GetTestName(), "Extract3ArgsRelativePath", false);
        }

        [Fact]
        public void FlatOverwriteExtract3Args()
        {
            var commandLine = "xfo TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi FlatOverwriteExtract3Args\\ \"cs-money.build\" \"requiresMTA.html\"";
            TestExtraction(commandLine, GetTestName(), "FlatOverwriteExtract3Args", false, flatExtractionFlag: true);
        }

        [Fact]
        public void FlatRenameExtract3Args()
        {
            var commandLine = "xfr TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi FlatRenameExtract3Args\\ \"cs-money.build\" \"requiresMTA.html\"";
            TestExtraction(commandLine, GetTestName(), "FlatRenameExtract3Args", false, flatExtractionFlag: true);
        }

        [Fact]
        public void ExtractCompatibility1Arg()
        {
            var commandLine = @"/x TestFiles\MsiInput\NUnit-2.5.2.9222.msi";
            TestExtraction(commandLine, GetTestName(), "NUnit-2.5.2.9222", false);
        }

        [Fact]
        public void ExtractCompatibility2Args()
        {
            var commandLine = @"/x TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi ExtractCompatibility2Args\";
            TestExtraction(commandLine, GetTestName(), "ExtractCompatibility2Args", false);
        }

        [Fact]
        public void BackwardCompatibilityParserNoMsiSpecifiedParser()
        {
            var commandLine = "/x";

            string consoleOutput;
            Assert.Throws(typeof(ExitCodeException), () =>
            {
                var exitCode = RunCommandLine(commandLine, out consoleOutput);
                Assert.Equal(3, exitCode);
            });
        }

        [Fact]
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
            // strangely I've seen newlines treated differently either by xunit or different versions of windows. So lets compare these as a list of lines: https://ci.appveyor.com/project/activescott/lessmsi/build/1.0.7/tests
            string[] expectedOutputLines = expectedOutput.Split(new string[]{ "\r\n", "\n" }, StringSplitOptions.None);
            string[] consoleOutputLines = consoleOutput.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            Assert.Equal(expectedOutputLines, consoleOutputLines);
        }

        [Fact]
        public void Version()
        {
            var expectedOutput = "2.5.2.9222" + Environment.NewLine;
            string consoleOutput;
            RunCommandLine("v TestFiles\\MsiInput\\NUnit-2.5.2.9222.msi", out consoleOutput);
            Assert.Equal(expectedOutput, consoleOutput);
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

        /// <summary>
        /// Executes the specified command. Assume working directory is TestFiles\MsiInput\ dir.
        /// </summary>
        /// <param name="commandLineArguments">The command line arguments (everything after the exe name).</param>
        /// <param name="testName">The name of hte test (used to formulate the expectedEntries output dir).</param>
        /// <param name="actualEntriesOutputDir">The output directory where the actual extraction is expected to occur.</param>
        /// <param name="useInProcessForDebugging">Bool flag for displaying extraction prints while testing</param>
        /// <param name="flatExtractionFlag">Bool flag for testing flat extractions</param>
        private void TestExtraction(string commandLineArguments, string testName, string actualEntriesOutputDir, bool useInProcessForDebugging, bool flatExtractionFlag = false)
        {
			string consoleOutput;
            var actualOutDir = new LessIO.Path(actualEntriesOutputDir).FullPath;
            if (actualOutDir.Exists)
                FileSystem.RemoveDirectory(actualOutDir, true);
	        int exitCode;

			if (useInProcessForDebugging)
				exitCode = base.RunCommandLineInProccess(commandLineArguments);
			else
				exitCode = base.RunCommandLine(commandLineArguments, out consoleOutput);
            Assert.Equal(0, exitCode);
			var actualEntries = FileEntryGraph.GetActualEntries(actualOutDir.FullPathString, "Actual Entries");
	        var actualEntriesFile = GetActualOutputFile(testName);
	        actualEntries.Save(actualEntriesFile);
			//Console.WriteLine("Actual entries saved to " + actualEntriesFile.FullName);
			var expectedEntries = GetExpectedEntriesForMsi(testName);
            AssertAreEqual(expectedEntries, actualEntries, flatExtractionFlag);
        }

		#endregion
	}
}
