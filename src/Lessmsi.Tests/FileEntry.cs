using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace LessMsi.Tests
{
    [DebuggerDisplay("{Path}")]
    public class FileEntry : IEquatable<FileEntry>
    {
        /// <summary>
        /// Initializes a new FileEntry.
        /// </summary>
        /// <param name="path">The initial value for <see cref="FileEntry.Path"/>.</param>
        /// <param name="size">The initial value for <see cref="FileEntry.Size"/>.</param>
        public FileEntry(string path, long size)
        {
            _size = size;
            _path = path;
        }

        /// <summary>
        /// Initializes a new FileEntry
        /// </summary>
        /// <param name="file">The file this object represents.</param>
        /// <param name="relativeTo">The root path that the specified file is relative to. The value of <see cref="FileEntry.Path"/> will be changed to be relative to this value.</param>
        public FileEntry(FileInfo file, string relativeTo)
        {
            _size = file.Length;

            if (file.FullName.StartsWith(relativeTo))
                _path = file.FullName.Substring(relativeTo.Length);
            else
                _path = file.FullName;
        }

        public string Path
        {
            get { return _path; }
        } private readonly string _path;

        public long Size
        {
            get { return _size; }
        } private readonly long _size;

        #region IEquatable<FileEntry>
        public bool Equals(FileEntry other)
        {
            return this.Size == other.Size && string.Equals(this.Path, other.Path, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
    }
}
