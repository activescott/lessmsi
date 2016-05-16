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
// Copyright (c) 2009 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LessMsi.IO
{
    /// <summary>
    /// Supports various File I/O operations including support for path names beyond 255 characters and up to 32K characters on Win32.
    /// </summary>
	public static partial class PathEx
	{
        /// <summary>
        /// Specified in Windows Headers for default maximum path. To go beyond this length you must prepend <see cref="LongPathPrefix"/> to the path.
        /// </summary>
        internal const int MAX_PATH = 260;
        /// <summary>
        /// This is the special prefix to prepend to paths to support up to 32,767 character paths.
        /// </summary>
        public static readonly string LongPathPrefix = @"\\?\";

        public static string Combine(params string[] pathParts)
		{
			if (pathParts.Length < 2)
				throw new ArgumentException("Expected at least two parts to combine.");
			var output = Path.Combine(pathParts[0], pathParts[1]);
			for (var i = 2; i < pathParts.Length; i++)
			{
				output = Path.Combine(output, pathParts[i]);
			}
			return output;
		}

        /// <summary>
        /// Indicates if teh two paths are equivelent and point to the same file or directory.
        /// </summary>
        public static bool Equals(string pathA, string pathB)
        {
            // the big thing I know to look for here is to make sure both have the LongPathPrefix or both don't:
            Func<string,string> RemovePathPrefix = (string path) => path.StartsWith(LongPathPrefix) ? path.Substring(LongPathPrefix.Length) : path;
            pathA = RemovePathPrefix(pathA);
            pathB = RemovePathPrefix(pathB);
            var partsA = pathA.Split(DirectorySeperatorChars);
            var partsB = pathB.Split(DirectorySeperatorChars);
            if (partsA.Length != partsB.Length)
                return false;
            
            for (var i=0; i < partsA.Length; i++)
            {
                var areEqual = string.Equals(partsA[i], partsB[i], StringComparison.InvariantCultureIgnoreCase);
                if (!areEqual)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the specified path with the <see cref="LongPathPrefix"/> prepended if necessary.
        /// </summary>
        internal static string EnsureLongPathPrefix(string path)
        {
            if (!path.StartsWith(LongPathPrefix)) // More consistent to deal with if we just add it to all of them: if (!path.StartsWith(LongPathPrefix) && path.Length >= MAX_PATH)
                return LongPathPrefix + path;
            else
                return path;
        }

        /// <summary>
        /// Returns the directory seperator characers.
        /// </summary>
        public static char[] DirectorySeperatorChars
        {
            get
            {   
                return new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            }
        }

        /// <summary>
        /// Returns the path that is the parent of the specified path.
        /// Safe for long file names.
        /// </summary>
        /// <param name="path">The path to get the parent of.</param>
        /// <returns></returns>
        public static string GetParentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            bool didTrimLongPathPrefix = false;

            // if this path starts with the long directory prefix, just trim it off to keep things simple:
            if (didTrimLongPathPrefix = path.StartsWith(PathEx.LongPathPrefix))
                path = path.Substring(PathEx.LongPathPrefix.Length);

            path = path.TrimEnd(PathEx.DirectorySeperatorChars);
            var parentEnd = path.LastIndexOfAny(PathEx.DirectorySeperatorChars);
            if (parentEnd >= 0 && parentEnd > GetRootLength(path)) {
                var result = path.Substring(0, parentEnd);
                if (didTrimLongPathPrefix)
                    result = LongPathPrefix + result;
                return result;
            }
            else
                return "";
        }

        public static bool IsDirectorySeparator(char ch)
        {
            return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
        }

        /// <summary>
        /// Uses Win32 to create the path at the specified location.
        /// The point of this method is to avoid <see cref="System.IO.PathTooLongException"/> of .NET System.IO APIs.
        /// </summary>
        public static void CreateDirectory(string path)
        {
            // Directory.Create() creates all neecessary directories, so we have to emulate here:
            var dirsToCreate = new List<String>();
            int lengthRoot = GetRootLength(path);
            char[] seperators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

            var firstNonRootPathIndex = path.IndexOfAny(seperators, lengthRoot);
            var i = firstNonRootPathIndex;
            while (i < path.Length)
            {
                if (IsDirectorySeparator(path[i]) || i == path.Length - 1)
                {
                    var currentPath = path.Substring(0, i + 1);
                    currentPath = currentPath.TrimEnd(seperators);// Win32 won't deal with trailing seperators
                    currentPath = EnsureLongPathPrefix(currentPath);
                    var pathExists = Exists(currentPath);
                    if (!pathExists)
                        pathExists = NativeMethods.CreateDirectory(currentPath, null);
                    Debug.Assert(pathExists, "path should always exists at this point!");
                }
                i++;
            }
        }

        /// <summary>
        /// Returns true if the specified directory exists.
        /// Supports long path names.
        /// </summary>
        internal static bool IsDirectory(string path)
        {
            FileAttributes attributes = GetFileAttributes(path);
            return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static bool IsReadOnly(string path)
        {
            FileAttributes attributes = GetFileAttributes(path);
            return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        /// <summary>
        /// Returns true if a file or directory exists at the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            bool fileExists;
            var ret = GetFileAttributes(path, out fileExists);
            return fileExists;
        }

        /// <summary>
        /// Returns file attriubtes for the specified file/directory.
        /// Supports long path names.
        /// </summary>
        /// <param name="path"></param>
        public static FileAttributes GetFileAttributes(string path)
        {
            bool fileExists;
            var ret = GetFileAttributes(path, out fileExists);
            if (!fileExists)
                throw new IOException(string.Format("File not found: '{0}'", path));
            return ret;
        }

        /// <summary>
        /// Returns file attriubtes for the specified file/directory.
        /// Supports long path names.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exists">Indicates whether the file exists or not.</param>
        /// <returns></returns>
        public static FileAttributes GetFileAttributes(string path, out bool exists)
        {
            path = EnsureLongPathPrefix(path);
            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle = NativeMethods.FindFirstFile(path, out findData);
            try
            {
                exists = findHandle != NativeMethods.INVALID_HANDLE_VALUE;
                if (exists)
                    return findData.dwFileAttributes;
                else
                    return 0;
            }
            finally
            {
                if (findHandle != NativeMethods.INVALID_HANDLE_VALUE)
                    NativeMethods.FindClose(findHandle);
            }
        }

        /// <summary>
        /// Sets the attributes for a file or directory.
        /// </summary>
        /// <param name="lpFileName">
        /// The name of the file whose attributes are to be set.
        /// In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
        /// </param>
        /// <param name="dwFileAttributes">
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365535%28v=vs.85%29.aspx
        /// </remarks>
        internal static void SetFileAttributes(string lpFileName, FileAttributes fileAttributes)
        {
            var ret = NativeMethods.SetFileAttributes(lpFileName, (uint)fileAttributes);
            if (!ret)
                throw new IOException(string.Format("Error setting file attributes on file '{0}'. Error Code=0x{1:x8}", lpFileName, Marshal.GetLastWin32Error()));
        }

        /// <summary>
        /// Returns the length of the root path specificiation in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static int GetRootLength(string path)
        {
            return GetPathRoot(path).Length;
        }

        /// <summary>
        /// Modeled after <see cref="System.IO.Path.GetPathRoot(string)"/> but supports long path names.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// See https://msdn.microsoft.com/en-us/library/system.io.path.getpathroot%28v=vs.110%29.aspx
        /// Possible patterns for the string returned by this method are as follows:
        /// An empty string (path specified a relative path on the current drive or volume).
        /// "/"(path specified an absolute path on the current drive).
        /// "X:"(path specified a relative path on a drive, where X represents a drive or volume letter).
        /// "X:/"(path specified an absolute path on a given drive).
        /// "\\ComputerName\SharedFolder"(a UNC path).
        /// </remarks>
        public static string GetPathRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            // "X:/"(path specified an absolute path on a given drive).
            if (path.Length >= 3 && path[1] == ':' && IsDirectorySeparator(path[2]))
                return path.Substring(0, 3);
            // "X:"(path specified a relative path on a drive, where X represents a drive or volume letter).
            if (path.Length >= 2 && path[1] == ':')
            {
                return path.Substring(0, 2);
            }
            // "\\ComputerName\SharedFolder"(a UNC path).
            // NOTE: UNC Path "root" includes the server/host AND have the root share folder too.
            if (path.Length > 2 
                && IsDirectorySeparator(path[0]) 
                && IsDirectorySeparator(path[1]) 
                && path.IndexOfAny(PathEx.DirectorySeperatorChars, 2)>2)
            {
                var beginShareName = path.IndexOfAny(PathEx.DirectorySeperatorChars, 2);
                var endShareName = path.IndexOfAny(PathEx.DirectorySeperatorChars, beginShareName+1);
                if (endShareName < 0)
                    endShareName = path.Length;
                if (beginShareName > 2 && endShareName > beginShareName)
                    return path.Substring(0, endShareName);
            }
            // "/"(path specified an absolute path on the current drive).
            if (path.Length >=1 && IsDirectorySeparator(path[0]))
            {
                return path.Substring(0, 1);
            }

            // path specified a relative path on the current drive or volume?
            return "";
        }


        /// <summary>
        /// Deletes an existing empty directory.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa365488%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path"></param>
        internal static void RemoveDirectory(string path)
        {
            path = PathEx.EnsureLongPathPrefix(path);
            var ret = NativeMethods.RemoveDirectory(path);
            if (!ret)
                throw new IOException(
                    string.Format("Error removing directory{0}. Error code=0x{1:x8}", path, Marshal.GetLastWin32Error())
                );
        }

        /// <summary>
        /// Deletes an existing file.
        /// To remove an empty directory see <see cref="RemoveDirectory"/>.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363915%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="path"></param>
        internal static void DeleteFile(string path)
        {
            path = PathEx.EnsureLongPathPrefix(path);
            var ret = NativeMethods.DeleteFile(path);
            if (!ret)
                throw new IOException(
                    string.Format("Error deleting file {0}. Error code=0x{1:x8}", path, Marshal.GetLastWin32Error())
                );
        }

        /// <summary>
        /// Deletes the file or directory at the specified path.
        /// </summary>
        public static bool DeleteFileOrDirectory(string value)
        {
            Action<string> UnSetReadOnlyAttribute = (string path) => PathEx.SetFileAttributes(path, (PathEx.GetFileAttributes(path) & ~FileAttributes.ReadOnly));
            if (PathEx.IsDirectory(value))
            {
                RemoveDirectory(value);
            }
            else
            {
                if (PathEx.IsReadOnly(value))
                    UnSetReadOnlyAttribute(value);
                NativeMethods.DeleteFile(value);
            }
            return true;
        }

        /// <summary>
        /// Returns a list of all files and subdirectories within the specified directory.
        /// Safe for long file path names.
        /// </summary>
        public static List<string> GetAllFilesAndDirectories(string dirName)
        {
            dirName = EnsureLongPathPrefix(dirName);

            List<string> results = new List<string>();
            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle = NativeMethods.FindFirstFile(dirName + @"\*", out findData);
            if (findHandle != NativeMethods.INVALID_HANDLE_VALUE)
            {
                bool found;
                do
                {
                    string currentFileName = findData.cFileName;
                    // if this is a directory, find its contents
                    if (((int)findData.dwFileAttributes & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
                    {
                        if (currentFileName != "." && currentFileName != "..")
                        {
                            List<string> childResults = GetAllFilesAndDirectories(Path.Combine(dirName, currentFileName));
                            // add children and self to results
                            results.AddRange(childResults);
                            results.Add(Path.Combine(dirName, currentFileName));
                        }
                    }
                    // it’s a file; add it to the results
                    else {
                        results.Add(Path.Combine(dirName, currentFileName));
                    }
                    // find next
                    found = NativeMethods.FindNextFile(findHandle, out findData);
                } while (found);
            }
            // close the find handle
            NativeMethods.FindClose(findHandle);
            return results;
        }

        /// <summary>
        /// Recursively delets the specified directory and all of its contents.
        /// Supports long path names on windows.
        /// </summary>
        /// <param name="dirName">The name of the directory to delete.</param>
        public static void DeleteAllFilesAndDirectories(string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
                throw new ArgumentNullException("dirName");
            // normalize dirName so we can assume it doesn't have a slash on the end:
            dirName = dirName.TrimEnd(PathEx.DirectorySeperatorChars);
            dirName = PathEx.EnsureLongPathPrefix(dirName);
            
            // add the longfilename previx too (because GetAllFilesAndDirectories does to the children)
            var list = GetAllFilesAndDirectories(dirName);

            /****** Summary ******
             * First build out a tree of the files in the same hierarchy as you'd expect on the filesystem.
             * Then use the tree to delete all the descendents first (as Win32 requires us to delete all children before deleting a directory).
            ******/
            var rootNode = new FileNode(dirName);
            foreach (var pathName in list)
            {
                // build out a stack of paths such that all ancestors of the current node will be added before the current node:
                var ancestorPaths = new Stack<string>();
                // first add the current node since he'll be last off the stack this way.
                ancestorPaths.Push(pathName);
                Predicate<string> IsRoot = (string path) => rootNode.Equals(path);
                for (var parentPath = PathEx.GetParentPath(pathName); !IsRoot(parentPath); parentPath = PathEx.GetParentPath(parentPath))
                {
                    ancestorPaths.Push(parentPath);
                }
                // make sure that each ancestors has a node in the tree:
                while (ancestorPaths.Count > 0)
                {
                    var ancestorPath = ancestorPaths.Pop();
                    var ancestorNode = rootNode.FindDescendent(ancestorPath);
                    if (ancestorNode == null)
                    {
                        ancestorNode = new FileNode(ancestorPath);
                        Debug.Assert(rootNode.FindDescendent(ancestorPath)==null, "node already exists!");
                        rootNode.Insert(ancestorNode);
                    }
                }
            }

            // now do a depth first deletion of each FileNode:
            foreach (var node in rootNode.Children)
            {
                node.DeleteFile();
            }
        }

        /// <summary>
        /// Creates or overwrites the file at the specified path.
        /// </summary>
        /// <param name="filePath">The path and name of the file to create. Supports long file paths.</param>
        /// <returns>A <see cref="System.IO.FileStream"/> that provides read/write access to the file specified in path.</returns>
        public static FileStream CreateFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);

            filePath = EnsureLongPathPrefix(filePath);

            NativeMethods.EFileAccess fileAccess = NativeMethods.EFileAccess.GenericWrite | NativeMethods.EFileAccess.GenericRead;
            NativeMethods.EFileShare fileShareMode = NativeMethods.EFileShare.None;//exclusive
            NativeMethods.ECreationDisposition creationDisposition = NativeMethods.ECreationDisposition.CreateAlways;
            SafeFileHandle hFile = NativeMethods.CreateFile(filePath, fileAccess, fileShareMode, IntPtr.Zero, creationDisposition, NativeMethods.EFileAttributes.Normal, IntPtr.Zero);
            var win32Errror = Marshal.GetLastWin32Error();
            if (hFile.IsInvalid)
                throw new IOException(string.Format("Erorr creating file at path '{0}'. Win32 Error='{1}'.", filePath, win32Errror));

            return new FileStream(hFile, FileAccess.ReadWrite);
        }
    }
}
