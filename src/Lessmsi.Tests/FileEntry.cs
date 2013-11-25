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
        public FileEntry(string path, long size, DateTime creationTime, DateTime lastWriteTime, FileAttributes attributes)
        {
            Size = size;
            Path = path;
	        CreationTime = creationTime;
	        LastWriteTime = lastWriteTime;
	        Attributes = attributes;
        }

        /// <summary>
        /// Initializes a new FileEntry
        /// </summary>
        /// <param name="file">The file this object represents.</param>
        /// <param name="relativeTo">The root path that the specified file is relative to. The value of <see cref="FileEntry.Path"/> will be changed to be relative to this value.</param>
        public FileEntry(FileInfo file, string relativeTo)
        {
            Size = file.Length;

            Path = file.FullName.StartsWith(relativeTo) ? file.FullName.Substring(relativeTo.Length) : file.FullName;

	        CreationTime = file.CreationTime;
	        LastWriteTime = file.LastWriteTime;
	        Attributes = file.Attributes;
        }

	    public FileAttributes Attributes { get; private set; }
		public DateTime LastWriteTime { get; private set; }
		public DateTime CreationTime { get; private set; }
	    public string Path { get; private set; }
	    public long Size { get; private set; }

	    #region IEquatable<FileEntry>
        public bool Equals(FileEntry other)
        {
            return Size == other.Size && 
				string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase) && 
				Attributes == other.Attributes &&
				LastWriteTime == other.LastWriteTime &&
				CreationTime == other.CreationTime
				;
        }
        #endregion
    }
}
