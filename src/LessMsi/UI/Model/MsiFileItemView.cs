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
