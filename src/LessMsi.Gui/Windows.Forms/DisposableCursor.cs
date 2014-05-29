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

namespace LessMsi.Gui.Windows.Forms
{
    /// <summary>
    /// DisposableCursor allows you to use the C# using statement to return to a normal cursor.
    /// </summary>
    /// <example>Simple Example of using the DisposableCursor with the C# using statement
    /// <code lang="C#">
    /// using (new Willeke.Shared.Windows.Forms.DisposableCursor(this))
    /// {
    ///		// Put the busy operation code here...
    ///		System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
    /// }
    /// </code>
    /// </example>
    internal class DisposableCursor : IDisposable
    {
        // The cursor used before this DisposableCursor was initialized.
        private Cursor previousCursor;
        // The control this cursor instance is used with.
        private Control control;


        /// <summary>
        /// Initializes an instance of the DisposableCursor class with "Wait Cursor" displayed for the specified control.
        /// </summary>
        /// <param name="control">The control to display the cursor over.</param>
        public DisposableCursor(Control control)
            :this(control, Cursors.WaitCursor)
        {
        }

        /// <summary>
        /// Initializes an instance of the DisposableCursor class with the specified cursor displayed for the specified control.
        /// </summary>
        /// <param name="control">The control to display the cursor over.</param>
        /// <param name="newCursor">The cursor to display while the mouse pointer is over the control.</param>
        public DisposableCursor(Control control, Cursor newCursor)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            if (newCursor == null)
                throw new ArgumentNullException("cursor");

            this.previousCursor = control.Cursor;
            this.control = control;
            control.Cursor = newCursor;
            control.Update();
        }

        #region Implementation of IDisposable
        public void Dispose()
        {
            // Dispose the existing cursor (the one created by this class)
            //this.control.Cursor.Dispose();// DON'T dispose this.  Aparently .NET doesn't like you disposing system cursors :D

            // Give the control back it's old cursor
            this.control.Cursor = this.previousCursor;
        }
        #endregion

    }
}
