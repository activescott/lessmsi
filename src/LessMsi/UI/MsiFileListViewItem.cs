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
using System;
using System.Windows.Forms;
using LessMsi.Msi;

namespace LessMsi.UI
{
    internal class MsiFileListViewItem : ListViewItem
    {
        public MsiFile _file;

        public static void InitListViewColumns(ListView lv)
        {
            lv.Columns.Add("File Name", 200, HorizontalAlignment.Left);
            lv.Columns.Add("Directory", 225, HorizontalAlignment.Left);
            lv.Columns.Add("Size", 60, HorizontalAlignment.Left);
            lv.Columns.Add("Version", 60, HorizontalAlignment.Left);
            lv.AllowColumnReorder = true;
        }

        public MsiFileListViewItem(MsiFile file)
        {
            this.Checked = true;
            _file = file;
            this.Text = file.LongFileName;
            string path = file.Directory != null ? file.Directory.GetPath() : "";
            this.SubItems.Add(path);
            this.SubItems.Add(Convert.ToString(file.FileSize));
            this.SubItems.Add(Convert.ToString(file.Version));
        }

        public override string ToString()
        {
            return _file.LongFileName;
        }
    }
}