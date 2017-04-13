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
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Msi
{
	public class ViewWrapper : IDisposable
	{
		public ViewWrapper(View underlyingView)
		{
			_underlyingView = underlyingView;
			CreateColumnInfos();
		}

		private View _underlyingView;
		private ColumnInfo[] _columns;

		public ColumnInfo[] Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Returns the index of the specified column.
		/// </summary>
		/// <param name="columnName">The name of the column to return an index for.</param>
		public int ColumnIndex(string columnName)
		{
			for(var i=0; i < Columns.Length; i++)
			{
				if (string.Equals(columnName, Columns[i].Name, StringComparison.InvariantCultureIgnoreCase))
					return i;
			}
			Debug.Fail("Column {0} not found.", columnName);
			return -1;
		}

		private void CreateColumnInfos()
		{
			const int MSICOLINFONAMES = 0;
			const int MSICOLINFOTYPES = 1;

			var colList = new List<ColumnInfo>();

			Record namesRecord;
			Record typesRecord;

			_underlyingView.GetColumnInfo(MSICOLINFONAMES, out namesRecord);
			_underlyingView.GetColumnInfo(MSICOLINFOTYPES, out typesRecord);
			using (namesRecord)
			using (typesRecord)
			{
				var fieldCount = namesRecord.GetFieldCount();
				Debug.Assert(typesRecord.GetFieldCount() == fieldCount);

				for (var colIndex = 1; colIndex <= fieldCount; colIndex++)
				{
					colList.Add(new ColumnInfo(namesRecord.GetString(colIndex), typesRecord.GetString(colIndex)));
				}
			}
			_columns = colList.ToArray();
		}


		private List<object[]> _records;
		public IList<object[]> Records
		{
			get
			{
				if (_records == null)
				{
					_records = new List<object[]>();
					Record sourceRecord = null;

					while (_underlyingView.Fetch(out sourceRecord))
					{
						using (sourceRecord)
						{
							var values = new object[_columns.Length];

							for (int i = 0; i < _columns.Length; i++)
							{
								if (_columns[i].IsString)
									values[i] = sourceRecord.GetString(i + 1);
								else if (_columns[i].IsInteger)
									values[i] = sourceRecord.GetInteger(i + 1);
								else if (_columns[i].IsStream)
								{
									var tempBuffer = new byte[_columns[i].Size + 1];
									var allData = new byte[_columns[i].Size + 1];
									int totalBytesRead = 0;
									int bytesReadThisCall;
									do
									{
										// It seems to read the Binary table with _columns[i].Size ==0 tempBuffer must be at least 1 in length or an ExecutionEngineException occurs.
										bytesReadThisCall = sourceRecord.GetStream(i + 1, tempBuffer, tempBuffer.Length);
										Buffer.BlockCopy(tempBuffer, 0, allData, totalBytesRead, bytesReadThisCall);
										totalBytesRead += bytesReadThisCall;
										Debug.Assert(bytesReadThisCall > 0);
									} while (bytesReadThisCall > 0 && (totalBytesRead < _columns[i].Size));
									values[i] = allData;
								}
								else if (_columns[i].IsObject)
								{
									//we deliberately skip this case. Found this case in reading the _Tables table of some recent .msi files.
								}
								else
								{
									Debug.Fail("Unknown column type");
								}
							}
							_records.Add(values);
						}
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