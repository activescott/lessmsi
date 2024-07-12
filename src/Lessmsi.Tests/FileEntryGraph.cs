using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace LessMsi.Tests
{
    /// <summary>
    /// Used in testing to represent the graph/directory tree of files extracted from an MSI.
    /// </summary>
    public class FileEntryGraph
    {
	    private const char AltSepartor = '|';

	    /// <summary>
        /// Initializes a new instance of <see cref="FileEntryGraph"/>.
        /// </summary>
        /// <param name="forFileName">The initial value for <see cref="FileEntryGraph.ForFileName"/></param>
        public FileEntryGraph(string forFileName)
        {
            this.ForFileName = forFileName;
        }

        /// <summary>
        /// The file name that this graph is for.
        /// </summary>
        public string ForFileName { get; private set; }
        
        /// <summary>
        /// The entries of the file graph.
        /// </summary>
        public List<FileEntry> Entries
        { 
            get { return _entries; }
            set { _entries = value; } 
        } private  List<FileEntry> _entries = new List<FileEntry>();

        public void Add(FileEntry entry)
        {
            Entries.Add(entry);
        }

        /// <summary>
        /// Saves this <see cref="FileEntryGraph"/> to the specified file.
        /// </summary>
        /// <param name="file">The file that this graph will be saved to.</param>
        public void Save(LessIO.Path file)
        {
            //save it as csv:
            if (file.Exists)
                LessIO.FileSystem.RemoveFile(file);
            using (var f = file.CreateText())
            {
                f.WriteLine("Path,Size,CreationTime,LastWriteTime,Attributes");

                foreach (var e in this.Entries)
                {
                    f.Write(e.Path);
                    f.Write(",");
                    f.Write(e.Size);
					f.Write(",");
					f.Write(SerializeDate(e.CreationTime));
					f.Write(",");
	                f.Write(SerializeDate(e.LastWriteTime));
					f.Write(",");
					f.Write(SerializeAttributes(e.Attributes));
                    f.WriteLine();
                }
            }
        }

        /// <summary>
        /// Loads a <see cref="FileEntryGraph"/> from the specified file.
        /// </summary>
        /// <param name="file">The file to load a new <see cref="FileEntryGraph"/> from.</param>
        /// <param name="forFileName">The initial value for the returned objects <see cref="FileEntryGraph.ForFileName"/></param>
        /// <returns>The newly loaded <see cref="FileEntryGraph"/>.</returns>
        public static FileEntryGraph Load(LessIO.Path file, string forFileName)
        {
            var graph = new FileEntryGraph(forFileName);
            using (var f = System.IO.File.OpenText(file.PathString))
            {
                f.ReadLine();//headings
                while (!f.EndOfStream)
                {
                    var line = f.ReadLine().Split(',');
                    if (line.Length != 5)
                        throw new IOException("Expected 5 fields!");
                    /* FIX for github issue #23: 
					 * The problem was that old ExpectedOutput files were all prefixed with C:\projects\lessmsi\src\Lessmsi.Tests\bin\Debug\<msiFileNameWithoutExtension> (something like C:\projects\lessmsi\src\Lessmsi.Tests\bin\Debug\NUnit-2.5.2.9222\SourceDir\PFiles\NUnit 2.5.2\fit-license.txt)
					 * We need to remove Since we don't reasonably know what the original msi filename was, we do know it was the subdirectory of C:\projects\lessmsi\src\Lessmsi.Tests\bin\Debug\. So we should remove C:\projects\lessmsi\src\Lessmsi.Tests\bin\Debug\ and the next subdirectory from the path. 
					 * HACK: A better fix would undoubtedly be to cleanup those old file swith code like this and remove this hack from this code forever!
					 */
                    var path = line[0];
                    const string oldRootPath = @"C:\projects\lessmsi\src\Lessmsi.Tests\bin\Debug\";
                    if (path.StartsWith(oldRootPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //this is an old file that would trigger github issue #23, so we'll fix it here...
                        // first remove the old root path: 
                        path = path.Substring(oldRootPath.Length);
                        // now romove the msi filename (which we don't know, but we know it is the next subdirectory of the old root):
                        var lengthOfSubDirectoryName = path.IndexOf('\\', 0);
                        path = path.Substring(lengthOfSubDirectoryName);
                    }
                    graph.Add(new FileEntry(path, Int64.Parse(line[1]), DeserializeDate(line[2]), DeserializeDate(line[3]), DeserializeAttributes(line[4])));
                }
            }
            return graph;
        }

		/// <summary>
		/// Gets a <see cref="FileEntryGraph"/> representing the files in the specified outputDir (where an MSI was extracted).
		/// </summary>
		public static FileEntryGraph GetActualEntries(string outputDir, string forFileName)
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

		private static string ReplaceAltSeperatorWithCommas(string line)
		{
			var parts = line.Split(AltSepartor);
			line = string.Join(",", parts);
			return line;
		}

		private static string ReplaceCommasWithAltSeparator(string output)
		{
			output = output.Replace(',', AltSepartor);
			return output;
		}

	    private static FileAttributes DeserializeAttributes(string line)
	    {
		    line = ReplaceAltSeperatorWithCommas(line);
		    return (FileAttributes)Enum.Parse(typeof(FileAttributes), line);
	    }
	    
	    private static string SerializeAttributes(FileAttributes attributes)
		{
			var output = attributes.ToString();
			output = ReplaceCommasWithAltSeparator(output);
			return output;
		}

	    private static DateTime DeserializeDate(string serializedDate)
	    {
		    return DateTime.Parse(serializedDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
	    }

		private static string SerializeDate(DateTime dateTime)
		{
			// Explicitly strip the timezone information when serialising,
			// because .CAB files have no zone information either.
			var output = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified).ToString("o", CultureInfo.InvariantCulture);
			return output;
		}

	    public static CompareEntriesResult CompareEntries(FileEntryGraph a, FileEntryGraph b)
        {
            return CompareEntries(a, b, false);
        }

        public static CompareEntriesResult CompareEntries(FileEntryGraph a, FileEntryGraph b, bool flatExtractionFlag)
        {
            string errorMessage = "";
            bool suceeded = getErrorMessageIfEntriesCountDifferent(a, b, ref errorMessage);

            for (int i = 0; i < Math.Max(a.Entries.Count, b.Entries.Count); i++)
            {
                if (!a.Entries[i].Equals(b.Entries[i], flatExtractionFlag))
                {
                    errorMessage += string.Format("'{0}'!='{1}' at index '{2}'.", a.Entries[i].Path, b.Entries[i].Path, i);
                    suceeded = false;
                }
            }

            return new CompareEntriesResult(suceeded, errorMessage);
        }

        /// <summary>
        /// This method compares two given FileEntryGraph objects, updates string container with error messages if needed and returns bool result
        /// </summary>
        /// <param name="a">Frst FileEntryGraph to compare</param>
        /// <param name="b">Second FileEntryGraph to compare</param>
        /// <param name="errorMessage">String container for storing any error messages</param>
        /// <returns>Method return true if enteries count is same, and false otherwise</returns>
        private static bool getErrorMessageIfEntriesCountDifferent(FileEntryGraph a, FileEntryGraph b, ref string errorMessage)
        {
            bool entryCountEqualFlag = a.Entries.Count == b.Entries.Count;

            if (!entryCountEqualFlag)
            {
                errorMessage = string.Format("Entries for '{0}' and '{1}' have a different number of file entries ({2}, {3} respectively).", a.ForFileName, b.ForFileName, a.Entries.Count, b.Entries.Count);
            }

            return entryCountEqualFlag;
        }
    }
}