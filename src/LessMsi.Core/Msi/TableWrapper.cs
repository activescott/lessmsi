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
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
    /// <summary>
    /// Represents a generic row in a table.
    /// </summary>
    public class TableRow
    {
        private readonly IDictionary _columns;

        private TableRow(IDictionary columns)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            _columns = columns;
        }

        public static TableRow[] GetRowsFromTable(Database msidb, string tableName)
        {
            if (!msidb.TableExists(tableName))
            {
                Trace.WriteLine(string.Format("Table name does {0} not exist Found.", tableName));
                return new TableRow[0];
            }

            string query = string.Concat("SELECT * FROM `", tableName, "`");
            using (ViewWrapper view = new ViewWrapper(msidb.OpenExecuteView(query)))
            {
                ArrayList /*<TableRow>*/ rows = new ArrayList(view.Records.Count);

                ColumnInfo[] columns = view.Columns;
                foreach (object[] values in view.Records)
                {
                    HybridDictionary valueCollection = new HybridDictionary(values.Length);
                    for (int cIndex = 0; cIndex < columns.Length; cIndex++)
                    {
                        valueCollection[columns[cIndex].Name] = values[cIndex];
                    }
                    rows.Add(new TableRow(valueCollection));
                }
                return (TableRow[]) rows.ToArray(typeof(TableRow));
            }
        }

        public string GetString(string columnName)
        {
            return Convert.ToString(GetValue(columnName));
        }
        public object GetValue(string columnName)
        {
            return _columns[columnName];
        }

        public Int32 GetInt32(string columnName)
        {
            return Convert.ToInt32(GetValue(columnName));
        }
    }
}
