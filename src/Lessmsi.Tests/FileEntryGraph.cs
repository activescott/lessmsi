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
	    private FileEntryGraph(string forFileName)
        {
            ForFileName = forFileName;
        }

        /// <summary>
        /// The file name that this graph is for.
        /// </summary>
        private string ForFileName { get; set; }
        
        /// <summary>
        /// The entries of the file graph.
        /// </summary>
        private List<FileEntry> Entries
        { 
            get { return _entries; }
        } private readonly List<FileEntry> _entries = new List<FileEntry>();

        private void Add(FileEntry entry)
        {
            Entries.Add(entry);
        }

        /// <summary>
        /// Saves this <see cref="FileEntryGraph"/> to the specified file.
        /// </summary>
        /// <param name="file">The file that this graph will be saved to.</param>
        public void Save(FileInfo file)
        {
            //save it as csv:
            if (file.Exists)
                file.Delete();
            using (var f = file.CreateText())
            {
                f.WriteLine("Path,Size,CreationTime,LastWriteTime,Attributes");

                foreach (var e in Entries)
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
        public static FileEntryGraph Load(FileInfo file, string forFileName)
        {
            var graph = new FileEntryGraph(forFileName);
            using (var f = file.OpenText())
            {
                f.ReadLine();//headings
                while (!f.EndOfStream)
                {
                    var readLine = f.ReadLine();
                    if (readLine == null) continue;
                    var line = readLine.Split(',');
                    if (line.Length != 5)
                        throw new IOException("Expected 5 fields!");
                    graph.Add(new FileEntry(line[0], Int64.Parse(line[1]), DeserializeDate(line[2]), DeserializeDate(line[3]), DeserializeAttributes(line[4])) );
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
			var output = dateTime.ToString("o", CultureInfo.InvariantCulture);
			return output;
		}

	    public static bool CompareEntries(FileEntryGraph a, FileEntryGraph b, out string errorMessage)
        {
            if (a.Entries.Count != b.Entries.Count)
            {
                errorMessage = string.Format("Entries for '{0}' and '{1}' have a different number of file entries ({2}, {3} respectively).", a.ForFileName, b.ForFileName, a.Entries.Count, b.Entries.Count);
                return false;
            }

            for (var i = 0; i < Math.Max(a.Entries.Count, b.Entries.Count); i++)
            {
                if (a.Entries[i].Equals(b.Entries[i])) continue;
                errorMessage = string.Format("'{0}'!='{1}' at index '{2}'.", a.Entries[i].Path, b.Entries[i].Path, i);
                return false;
            }
            errorMessage = "";
            return true;
        }
    }
}