// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System.Collections;
using System.Diagnostics;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a file in the msi file table/view.
    /// </summary>
    public class MsiFile
    {
        public string File;// a unique id for the file
        public string LongFileName;
        public int FileSize;
        public string Version;
        private string _component;

        /// <summary>
        /// Returns the directory that this file belongs in.
        /// </summary>
        public MsiDirectory Directory { get; private set; }

        private string ShortFileName { get; set; }

        private MsiFile()
        {
        }

		/// <summary>
		/// Creates a list of <see cref="MsiFile"/> objects from the specified database.
		/// </summary>
		public static MsiFile[] CreateMsiFilesFromMsi(string msiDatabaseFilePath)
		{
		    if (msiDatabaseFilePath != null)
		        using (var db = new Database(msiDatabaseFilePath, OpenDatabase.ReadOnly))
		        {
		            return CreateMsiFilesFromMsi(db);
		        }
		}

        /// <summary>
        /// Creates a list of <see cref="MsiFile"/> objects from the specified database.
        /// </summary>
        public static MsiFile[] CreateMsiFilesFromMsi(Database msidb)
        {
            var rows = TableRow.GetRowsFromTable(msidb, "File");

            // do some prep work to cache values from MSI for finding directories later...
            MsiDirectory[] rootDirectories;
            MsiDirectory[] allDirectories;
            MsiDirectory.GetMsiDirectories(msidb, out rootDirectories, out allDirectories);

            //find the target directory for each by reviewing the Component Table
            var components = TableRow.GetRowsFromTable(msidb, "Component"); //Component table: http://msdn.microsoft.com/en-us/library/aa368007(v=vs.85).aspx
            //build a table of components keyed by it's "Component" column value
            var componentsByComponentTable = new Hashtable();
            foreach (var component in components)
            {
                componentsByComponentTable[component.GetString("Component")] = component;
            }

            var/*<MsiFile>*/ files = new ArrayList(rows.Length);
            foreach (var row in rows)
            {
                var file = new MsiFile();
				
                var fileName = row.GetString("FileName");
                var split = fileName.Split('|');
                file.ShortFileName = split[0];
                file.LongFileName = split.Length > 1 ? split[1] : split[0];

                file.File = row.GetString("File");
                file.FileSize = row.GetInt32("FileSize");
                file.Version = row.GetString("Version");
                file._component = row.GetString("Component_");

                file.Directory = GetDirectoryForFile(file, allDirectories, componentsByComponentTable);
                files.Add(file);
            }
            return (MsiFile[])files.ToArray(typeof(MsiFile));
        }

        private static MsiDirectory GetDirectoryForFile(MsiFile file, MsiDirectory[] allDirectories, IDictionary componentsByComponentTable)
        {
            // get the component for the file
            var componentRow = componentsByComponentTable[file._component] as TableRow;
            if (componentRow != null)
            {
                var componentDirectory = componentRow.GetString("Directory_");
                var directory = FindDirectoryByDirectoryKey(allDirectories, componentDirectory);
                if (directory != null)
                {
                    //Trace.WriteLine(string.Format("Directory for '{0}' is '{1}'.", file.LongFileName, directory.GetPath()));
                }
                else
                {
                    Debug.Fail(string.Format("directory not found for file '{0}'.", file.LongFileName));
                }
                return directory;
            }
            // found component, get the directory:
            Debug.Assert(false, "File '{0}' has no component entry.", file.LongFileName);
            return null;
        }

        /// <summary>
        /// Returns the directory with the specified value for <see cref="MsiDirectory.Directory"/> or null if it cannot be found.
        /// </summary>
        /// <param name="allDirectories"></param>
        /// <param name="directory_Value">The value for the sought directory's <see cref="MsiDirectory.Directory"/> column.</param>
        private static MsiDirectory FindDirectoryByDirectoryKey(IEnumerable<MsiDirectory> allDirectories, string directory_Value)
        {
            return allDirectories.FirstOrDefault(dir => 0 == string.CompareOrdinal(dir.Directory, directory_Value));
        }
    }
}