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
        private string _targetName;
        private string _sourceName;
        private string _shortName = "";
        //the "DefaultDir value from the MSI table.
        private string _defaultDir="";
        /// The "Directory" entry from the MSI
        private string _directory="";
        /// The "Directory_Parent" entry
        private string _directoryParent;
        /// Stores the child directories
        private ArrayList _children = new ArrayList();
        private MsiDirectory _parent;

        private MsiDirectory()
        {
        }

        /// <summary>
        /// The name of this directory on the destination comp.
        /// </summary>
        public string TargetName
        {
            get { return _targetName; }
        }
        
        /// <summary>
        /// The name of this directory in the source computer (when the MSI was built).
        /// </summary>
        public string SourceName
        {
            get { return _sourceName; }
        }

        /// <summary>
        /// Returns the alternative short name (8.3 format) of this directory.
        /// </summary>
        public string ShortName
        {
            get { return _shortName; }
        }

        /// The "Directory" entry from the MSI
        public string Directory
        {
            get { return _directory; }
        }

        /// The "Directory_Parent" entry
        public string DirectoryParent
        {
            get { return _directoryParent; }
        }

        /// <summary>
        /// The direct child directories of this directory.
        /// </summary>
        public ICollection Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Returns this directory's parent or null if it is a root directory.
        /// </summary>
        public MsiDirectory Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Returns the full path considering it's parent directories.
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string path = this.TargetName;
            MsiDirectory parent = this.Parent;
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
            TableRow[] rows = TableRow.GetRowsFromTable(msidb, "Directory");
            Hashtable directoriesByDirID = new Hashtable();

            foreach (TableRow row in rows)
            {
                MsiDirectory directory = new MsiDirectory();
                directory._defaultDir = row.GetString("DefaultDir");
                if (directory._defaultDir != null && directory._defaultDir.Length > 0)
                {
                    string[] split = directory._defaultDir.Split('|');

                    directory._shortName = split[0];
                    if (split.Length > 1)
                        directory._targetName = split[1];
                    else
                        directory._targetName = split[0];
                    
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
                    split = directory._targetName.Split(':');
                    if (split.Length > 1)
                    {   //semicolon present
                        directory._targetName = split[0];
                        directory._sourceName = split[1];
                    }
                    else
                    {
                        directory._sourceName = directory._targetName;
                    }
                }
                
                directory._directory = row.GetString("Directory");
                directory._directoryParent = row.GetString("Directory_Parent");
                directoriesByDirID.Add(directory.Directory, directory);
            }
            //Now we have all directories in the table, create a structure for them based on their parents.
            ArrayList rootDirectoriesList = new ArrayList();
            foreach (MsiDirectory dir in directoriesByDirID.Values)
            {
                if (dir.DirectoryParent == null || dir.DirectoryParent.Length == 0)
                {
                    rootDirectoriesList.Add(dir);
                    continue;
                }

                MsiDirectory parent = directoriesByDirID[dir.DirectoryParent] as MsiDirectory;
                dir._parent = parent;
                parent._children.Add(dir);
            }
            // return the values:
            rootDirectories = (MsiDirectory[])rootDirectoriesList.ToArray(typeof(MsiDirectory));
			
            MsiDirectory[] allDirectoriesLocal = new MsiDirectory[directoriesByDirID.Values.Count];
            directoriesByDirID.Values.CopyTo(allDirectoriesLocal,0);
            allDirectories = allDirectoriesLocal;
        }
    }
}