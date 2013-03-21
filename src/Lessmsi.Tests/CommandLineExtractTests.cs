using System;
using System.Collections.Generic;
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
        [Test,Ignore]
        public void MinimalExtraction()
        {
            TestExtraction("o NUnit-2.5.2.9222.msi", GetTestName());            
        }

        [Test]
        public void MinimalParser()
        {
            var expected = new Program.Arguments()
            {
                MsiFileName = "someinstall.msi",
                OutDirName = "someinstall"
            };
            TestParser(expected, "o someinstall.msi"); ;
        }

        
        [Test]
        public void BackwardCompatibilityParser1()
        {
            var commandLine = @"/x c:\projects\lessmsi\src\Lessmsi.Tests\TestFiles\MsiInput\ExtractOnlySomeFiles.msi";
            var expected = new Program.Arguments()
            {
                MsiFileName = "c:\\projects\\lessmsi\\src\\Lessmsi.Tests\\TestFiles\\MsiInput\\ExtractOnlySomeFiles.msi",
                OutDirName = "c:\\projects\\lessmsi\\src\\Lessmsi.Tests\\TestFiles\\MsiInput",
                ErrorCode = 0
            };
            TestParser(expected, commandLine);
        }

        [Test]
        public void BackwardCompatibilityParser2()
        {
            var commandLine = @"/x ExtractOnlySomeFiles.msi";
            var expected = new Program.Arguments()
            {
                MsiFileName = "ExtractOnlySomeFiles.msi",
                OutDirName = "",
                ErrorCode = 0
            };
            TestParser(expected, commandLine);
        }

        [Test]
        public void BackwardCompatibilityParser3()
        {
            var commandLine = @"/x ExtractOnlySomeFiles.msi somedir";
            var expected = new Program.Arguments()
            {
                MsiFileName = "ExtractOnlySomeFiles.msi",
                OutDirName = "somedir",
                ErrorCode = 0
            };
            TestParser(expected, commandLine);
        }
        
        [Test]
        public void BackwardCompatibilityParser4()
        {
            var commandLine = @"/x .\Lessmsi.Tests\TestFiles\MsiInput\NUnit-2.5.2.9222.msi";
            var expected = new Program.Arguments()
            {
                MsiFileName = @".\Lessmsi.Tests\TestFiles\MsiInput\",
                OutDirName = "",
                ErrorCode = 0
            };
            TestParser(expected, commandLine);
        }
        
        [Test, ExpectedException(typeof(OptionException))]
        public void BackwardCompatibilityParserNoMsiSpecified()
        {
            var commandLine = "/x";
            var expected = new Program.Arguments()
            {
                MsiFileName = "ExtractOnlySomeFiles.msi",
                OutDirName = "somedir",
                ErrorCode = -2
            };
            TestParser(expected, commandLine); ;
        }

        

        private void TestParser(Program.Arguments expected, string commandLine)
        {
            var split = commandLine.Split(' ');
            var actual = Program.ParseArguments(split);
            AssertAreEqual(expected, actual);
        }
        
        private void AssertAreEqual(Program.Arguments expected, Program.Arguments actual)
        {
            var serializer = new DataContractJsonSerializer(typeof (Program.Arguments));
            var expectedStream = new MemoryStream();
            var actualStream = new MemoryStream(); 
            serializer.WriteObject(expectedStream, expected);
            serializer.WriteObject(actualStream, actual);
            var expectedString = new StreamReader(expectedStream).ReadToEnd();
            var actualString = new StreamReader(actualStream).ReadToEnd();
            Assert.AreEqual(expectedString, actualString);
        }

        /// <summary>
        /// Returns the name of the calling method.
        /// </summary>
        private string GetTestName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the specified command. Assume working directory is TestFiles\MsiInput\ dir.
        /// </summary>
        /// <param name="theCommand"></param>
        private void TestExtraction(string theCommand, string testName)
        {
            throw new NotImplementedException();
            var actualEntries = RunCommand(theCommand);
            var expectedEntries = GetExpectedEntriesForTest(testName);
            AssertAreEqual(expectedEntries, actualEntries);
        }

        private FileEntryGraph RunCommand(string commandLine)
        {
            //TODO: Process the specified command and get a list of arguments.
            //TODO:
            throw new NotImplementedException();
        }

        private FileEntryGraph GetExpectedEntriesForTest(string getTestName)
        {
            throw new NotImplementedException();
        }
    }
}
