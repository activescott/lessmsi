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
using System.Collections;
using System.Diagnostics;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
    class ViewWrapper : IDisposable
    {
        public ViewWrapper(View underlyingView)
        {
            this._underlyingView = underlyingView;
            CreateColumnInfos();
        }

        private View _underlyingView;
        private ColumnInfo[]_columns;

        public ColumnInfo[] Columns
        {
            get { return _columns; }
        }


        private void CreateColumnInfos()
        {
            const int MSICOLINFONAMES = 0;
            const int MSICOLINFOTYPES = 1;
			
            ArrayList colList = new ArrayList();/*<ColumnInfo>*/

            Record namesRecord; Record typesRecord;
            _underlyingView.GetColumnInfo(MSICOLINFONAMES, out namesRecord);
            _underlyingView.GetColumnInfo(MSICOLINFOTYPES, out typesRecord);

            int fieldCount = namesRecord.GetFieldCount();
            Debug.Assert(typesRecord.GetFieldCount() == fieldCount);

            for (int colIndex = 1; colIndex <= fieldCount; colIndex++)
            {
                colList.Add(new ColumnInfo(namesRecord.GetString(colIndex), typesRecord.GetString(colIndex)));
            }
            _columns = (ColumnInfo[])colList.ToArray(typeof(ColumnInfo));
        }


        private ArrayList/*<object[]>*/ _records;
        public IList/*<object[]>*/ Records
        {
            get
            {
                if (_records == null)
                {
                    _records = new ArrayList/*<object[]>*/();
                    Record sourceRecord;

                    while (_underlyingView.Fetch(out sourceRecord))
                    {
                        object[] values = new object[this._columns.Length];

                        for (int i = 0; i < this._columns.Length; i++)
                        {
                            if (this._columns[i].IsString)
                                values[i] = sourceRecord.GetString(i + 1);
                            else if (this._columns[i].IsInteger)
                                values[i] = sourceRecord.GetInteger(i + 1);
                            else
                            {
                                byte[] buffer = new byte[this._columns[i].Size];
                                int actualLen = sourceRecord.GetStream(i + 1, buffer, buffer.Length);
                                if (actualLen < buffer.Length)
                                {
                                    byte[] trim = new byte[actualLen];
                                    Buffer.BlockCopy(buffer, 0, trim, 0, actualLen);
                                    buffer = trim;
                                }
                                values[i] = buffer;
                            }
                        }
                        _records.Add(values);
                    }
                }
                return _records;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_underlyingView == null)
                return;
            _underlyingView.Close();
            _underlyingView = null;
        }

        #endregion
    }
}