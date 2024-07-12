using System;
using System.Diagnostics;
using System.IO;

namespace LessMsi.Tests
{
    [DebuggerDisplay("{Path}")]
    public sealed class FileEntry : IEquatable<FileEntry>
    {
	    /// <summary>
	    /// Initializes a new FileEntry.
	    /// </summary>
	    /// <param name="path">The initial value for <see cref="FileEntry.Path"/>.</param>
	    /// <param name="size">The initial value for <see cref="FileEntry.Size"/>.</param>
	    /// <param name="creationTime"> </param>
	    /// <param name="lastWriteTime"> </param>
	    /// <param name="attributes"> </param>
	    public FileEntry(string path, long size, DateTime creationTime, DateTime lastWriteTime, FileAttributes attributes)
        {
            Size = size;
            Path = path;
	        this.CreationTime = creationTime;
	        this.LastWriteTime = lastWriteTime;
	        this.Attributes = attributes;
        }

        /// <summary>
        /// Initializes a new FileEntry
        /// </summary>
        /// <param name="file">The file this object represents.</param>
		/// <param name="basePathToRemove">
        /// The root of the path of the specified file that should be removed to ensure that the output is a relative portion of the file. 
        /// Essentially the value of <see cref="FileEntry.Path"/> will be changed by stripping of the begining portion of this file.
        /// </param>
		public FileEntry(FileInfo file, string basePathToRemove)
        {
            Size = file.Length;

			if (file.FullName.StartsWith(basePathToRemove, StringComparison.InvariantCultureIgnoreCase))
				Path = file.FullName.Substring(basePathToRemove.Length);
            else
			{
				Path = file.FullName;
				Debug.Fail("Why would this happen? Normally the file should be rooted in that path.");
			}
                

	        this.CreationTime = file.CreationTime;
	        this.LastWriteTime = file.LastWriteTime;
	        this.Attributes = file.Attributes;
        }

	    public FileAttributes Attributes { get; private set; }
		public DateTime LastWriteTime { get; private set; }
		public DateTime CreationTime { get; private set; }
	    public string Path { get; private set; }
	    public long Size { get; private set; }

		#region IEquatable<FileEntry>
		public bool Equals(FileEntry other)
		{
			return this.Equals(other, false);
		}

        public bool Equals(FileEntry other, bool flatExtractionFlag)
        {
            return isSizeEqual(other) &&
                isPathEqual(other) &&
                areAttributesEqual(other) &&
                isLastWriteTimeEqual(other) &&
                (flatExtractionFlag || isCreationTime(other))
                ;
        }
		#endregion

		#region Checking methods
		private bool isSizeEqual(FileEntry other)
		{
			return this.Size == other.Size;
		}

		private bool isPathEqual(FileEntry other)
		{
			return string.Equals(this.Path, other.Path, StringComparison.InvariantCultureIgnoreCase);
        }

		private bool areAttributesEqual(FileEntry other)
		{
			return this.Attributes == other.Attributes;
		}

		private bool isLastWriteTimeEqual(FileEntry other)
		{
			return this.LastWriteTime == other.LastWriteTime;
		}

		private bool isCreationTime(FileEntry other)
		{ 
			return this.CreationTime == other.CreationTime;
		}
        #endregion
    }
}
