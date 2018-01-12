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
    /// <summary>
    /// Represents a file in the msi file table/view.
    /// </summary>
    public class MsiFile
    {
        public string File;// a unique id for the file
        public string LongFileName;
        public string ShortFileName;
        public int FileSize;
        public string Version;
        public string Component;
        private MsiDirectory _directory;

        /// <summary>
        /// Returns the directory that this file belongs in.
        /// </summary>
        public MsiDirectory Directory
        {
            get { return _directory; }
        }

        private MsiFile()
        {
        }

	/// <summary>
	/// Creates a list of <see cref="MsiFile"/> objects from the specified database.
	/// </summary>
	public static MsiFile[] CreateMsiFilesFromMSI(LessIO.Path msiDatabaseFilePath)
	{
		using (var db = new Database(msiDatabaseFilePath.PathString, OpenDatabase.ReadOnly))
		{
			return CreateMsiFilesFromMSI(db);
		}
	}

        /// <summary>
        /// Creates a list of <see cref="MsiFile"/> objects from the specified database.
        /// </summary>
        public static MsiFile[] CreateMsiFilesFromMSI(Database msidb)
        {
            TableRow[] rows = TableRow.GetRowsFromTable(msidb, "File");

            // do some prep work to cache values from MSI for finding directories later...
            MsiDirectory[] rootDirectories;
            MsiDirectory[] allDirectories;
            MsiDirectory.GetMsiDirectories(msidb, out rootDirectories, out allDirectories);

            //find the target directory for each by reviewing the Component Table
            TableRow[] components = TableRow.GetRowsFromTable(msidb, "Component"); //Component table: http://msdn.microsoft.com/en-us/library/aa368007(v=vs.85).aspx
            //build a table of components keyed by it's "Component" column value
            Hashtable componentsByComponentTable = new Hashtable();
            foreach (TableRow component in components)
            {
                componentsByComponentTable[component.GetString("Component")] = component;
            }

            ArrayList/*<MsiFile>*/ files = new ArrayList(rows.Length);
            foreach (TableRow row in rows)
            {
                MsiFile file = new MsiFile();
				
                string fileName = row.GetString("FileName");
                string[] split = fileName.Split('|');
                file.ShortFileName = split[0];
                if (split.Length > 1)
                    file.LongFileName = split[1];
                else
                    file.LongFileName = split[0];

                file.File = row.GetString("File");
                file.FileSize = row.GetInt32("FileSize");
                file.Version = row.GetString("Version");
                file.Component = row.GetString("Component_");

                file._directory = GetDirectoryForFile(file, allDirectories, componentsByComponentTable);
                files.Add(file);
            }
            return (MsiFile[])files.ToArray(typeof(MsiFile));
        }

        private static MsiDirectory GetDirectoryForFile(MsiFile file, MsiDirectory[] allDirectories, IDictionary componentsByComponentTable)
        {
            // get the component for the file
            TableRow componentRow = componentsByComponentTable[file.Component] as TableRow;
            if (componentRow == null)
            {
                Debug.Assert(false, "File '{0}' has no component entry.", file.LongFileName);
                return null;
            }
            // found component, get the directory:
            string componentDirectory = componentRow.GetString("Directory_");
            MsiDirectory directory = FindDirectoryByDirectoryKey(allDirectories, componentDirectory);
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

        /// <summary>
        /// Returns the directory with the specified value for <see cref="MsiDirectory.Directory"/> or null if it cannot be found.
        /// </summary>
        /// <param name="directory_Value">The value for the sought directory's <see cref="MsiDirectory.Directory"/> column.</param>
        private static MsiDirectory FindDirectoryByDirectoryKey(MsiDirectory[] allDirectories, string directory_Value)
        {
            foreach (MsiDirectory dir in allDirectories)
            {
                if (0 == string.CompareOrdinal(dir.Directory, directory_Value))
                {
                    return dir;
                }
            }
            return null;
        }
    }
}
