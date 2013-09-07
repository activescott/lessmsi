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
using System.IO;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
    /// <summary>
    /// Represents an entry in the Directory table of an MSI file.
    /// See  http://msdn.microsoft.com/en-us/library/aa368295(v=vs.85).aspx for Directory Table Reference.
    /// </summary>
    public class MsiDirectory
    {
        private string _sourceName;
        private string _shortName = "";
        //the "DefaultDir value from the MSI table.
        private string _defaultDir="";
        /// The "Directory" entry from the MSI
        private string _directory="";

        private MsiDirectory()
        {
            Children = new ArrayList();
        }

        /// <summary>
        /// The name of this directory on the destination comp.
        /// </summary>
        private string TargetName { get; set; }

        /// The "Directory" entry from the MSI
        public string Directory
        {
            get { return _directory; }
        }

        /// The "Directory_Parent" entry
        private string DirectoryParent { get; set; }

        /// <summary>
        /// The direct child directories of this directory.
        /// </summary>
        private ArrayList Children { get; set; }

        /// <summary>
        /// Returns this directory's parent or null if it is a root directory.
        /// </summary>
        private MsiDirectory Parent { get; set; }

        /// <summary>
        /// Returns the full path considering it's parent directories.
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            var path = this.TargetName;
            var parent = this.Parent;
            while (parent != null)
            {
                //Sometimes parent is a '.' In this case, the files should be directly put into the parent of the parent. See http://msdn.microsoft.com/en-us/library/aa368295%28VS.85%29.aspx
                if (parent.TargetName != ".")
                {
                    path = Path.Combine(parent.TargetName, path);
                }
				
                parent = parent.Parent;
            }
            return path;
        }

        /// <summary>
        /// Creates a list of <see cref="MsiDirectory"/> objects from the specified database.
        /// </summary>
        /// <param name="allDirectories">All directories in the table.</param>
        /// <param name="msidb">The databse to get directories from.</param>
        /// <param name="rootDirectories">
        /// Only the root directories (those with no parent). Use <see cref="MsiDirectory.Children"/> to traverse the rest of the directories.
        /// </param>
        public static void GetMsiDirectories(Database msidb, out MsiDirectory[] rootDirectories, out MsiDirectory[] allDirectories)
        {
            var rows = TableRow.GetRowsFromTable(msidb, "Directory");
            var directoriesByDirId = new Hashtable();

            foreach (var row in rows)
            {
                var directory = new MsiDirectory {_defaultDir = row.GetString("DefaultDir")};
                if (!string.IsNullOrEmpty(directory._defaultDir))
                {
                    var split = directory._defaultDir.Split('|');

                    directory._shortName = split[0];
                    directory.TargetName = split.Length > 1 ? split[1] : split[0];
                    
                    //Semi colons can delmit the "target" and "sorce" names of the directory in DefaultDir, so we're going to use the Target here (in looking at MSI files, I found Target seems most meaningful.
                    #region MSDN Docs on this Table
                    /*  From: http://msdn.microsoft.com/en-us/library/aa368295%28VS.85%29.aspx
                    The DefaultDir column contains the directory's name (localizable)under the parent directory. 
                    By default, this is the name of both the target and source directories. 
                    To specify different source and target directory names, separate the target and source names with a colon as follows: [targetname]:[sourcename].
                    If the value of the Directory_Parent column is null or is equal to the Directory column, the DefaultDir column specifies the name of a root source directory.
                    For a non-root source directory, a period (.) entered in the DefaultDir column for the source directory name or the target directory name indicates the directory should be located in its parent directory without a subdirectory.
                    The directory names in this column may be formatted as short filename | long filename pairs.
                    */
                    #endregion
                    split = directory._shortName.Split(':');
                    if (split.Length > 1)
                    {   //semicolon present
                        directory._shortName = split[0];
                    }
                    split = directory.TargetName.Split(':');
                    if (split.Length > 1)
                    {   //semicolon present
                        directory.TargetName = split[0];
                        directory._sourceName = split[1];
                    }
                    else
                    {
                        directory._sourceName = directory.TargetName;
                    }
                }
                
                directory._directory = row.GetString("Directory");
                directory.DirectoryParent = row.GetString("Directory_Parent");
                directoriesByDirId.Add(directory.Directory, directory);
            }
            //Now we have all directories in the table, create a structure for them based on their parents.
            var rootDirectoriesList = new ArrayList();
            foreach (MsiDirectory dir in directoriesByDirId.Values)
            {
                if (string.IsNullOrEmpty(dir.DirectoryParent))
                {
                    rootDirectoriesList.Add(dir);
                    continue;
                }

                var parent = directoriesByDirId[dir.DirectoryParent] as MsiDirectory;
                dir.Parent = parent;
                if (parent != null) parent.Children.Add(dir);
            }
            // return the values:
            rootDirectories = (MsiDirectory[])rootDirectoriesList.ToArray(typeof(MsiDirectory));
			
            var allDirectoriesLocal = new MsiDirectory[directoriesByDirId.Values.Count];
            directoriesByDirId.Values.CopyTo(allDirectoriesLocal,0);
            allDirectories = allDirectoriesLocal;
        }
    }
}