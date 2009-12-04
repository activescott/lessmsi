using System;
using System.Collections.Generic;
using System.Text;
using LessMsi.Msi;

namespace LessMsi.UI.Model
{
    /// <summary>
    /// Represensts an MSI file item (a file contained in an msi) in the UI.
    /// </summary>
    class MsiFileItemView
    {
        private readonly MsiFile _file;

        public MsiFileItemView(MsiFile msiDataFile)
        {
            _file = msiDataFile;
        }

        public string Name
        {
            get { return File.LongFileName; }
        }

        public string Directory
        {
            get { return File.Directory.GetPath(); }
        }

        public int Size
        {
            get { return File.FileSize; }
        }

        public string Version
        {
            get { return File.Version; }
        }

        internal MsiFile File
        {
            get { return _file; }
        }
    }
}
