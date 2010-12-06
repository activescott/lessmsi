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
using System.Collections.Generic;
using System.IO;
using LessMsi.UI.Model;

namespace LessMsi.UI
{
    internal interface IMainFormView
    {
    	/// <summary>
    	/// Call to notify the view that a new file has been loaded into the UI.
    	/// </summary>
    	void NotifyNewFileLoaded();
        /// <summary>
        /// Returns the currently chosen MSI file in the UI.
        /// </summary>
        FileInfo SelectedMsiFile { get; }
        /// <summary>
        /// The currently selected table headerText in the UI.
        /// </summary>
        string SelectedTableName { get; }
        /// <summary>
        /// Adds a column to the file grid.
        /// </summary>
        /// <param headerText="boundPropertyName">The headerText of the property the grid column is bound to.</param>
        /// <param headerText="headerText">The header caption for the column.</param>
        void AddFileGridColumn(string boundPropertyName, string headerText);
		/// <summary>
		/// Autosizes file grid columns based on content.
		/// </summary>
    	void AutoSizeFileGridColumns();
        /// <summary>
        /// Enables or disables the UI controls.
        /// </summary>
        void ChangeUiEnabled(bool doEnable);
        /// <summary>
        /// Returns the property currently selected in the UI or null if none selected.
        /// </summary>
        MsiPropertyInfo SelectedMsiProperty { get; }
        /// <summary>
        /// Sets or returns the description for the selected property in the UI.
        /// </summary>
        string PropertySummaryDescription {get; set;}
        /// <summary>
        /// Shows an informational message to the user.
        /// </summary>
        void ShowUserMessageBox(string message);
        /// <summary>
        /// Adds a column to the MSI table grid.
        /// </summary>
        void AddTableViewGridColumn(string headerText);
        /// <summary>
        /// Removes all columsn from the MSI Table grid.
        /// </summary>
        void ClearTableViewGridColumns();
        /// <summary>
        /// Adds a row to the MSI table grid.
        /// </summary>
        void SetTableViewGridDataSource(IEnumerable<object[]> values);

        void SetPropertyGridDataSource(MsiPropertyInfo[] props);
        void AddPropertyGridColumn(string boundPropertyName, string headerText);
    }
}