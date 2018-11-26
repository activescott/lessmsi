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
using System.Drawing;
using System.Windows.Forms;
using LessMsi.Msi;
using System.Collections.Generic;

namespace LessMsi.Gui
{
    internal class ExtractionProgressDialog : Form
    {
        private ProgressBar _progressBar;
        private Label _label;
        private readonly List<string> _errorFiles = new List<string>();
        private bool _continuePromptingErrors = true;

        public ExtractionProgressDialog(Form owner)
        {
            this.Owner = owner;

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(320, 125);
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.Manual;

            this.Top = owner.Top + ((owner.Height - this.Height)/2);
            this.Left = owner.Left + ((owner.Width - this.Width)/2);

            this.DockPadding.Left = this.DockPadding.Right = 10;
            this.DockPadding.Top = this.DockPadding.Bottom = 10;

            _progressBar = new ProgressBar();
            _progressBar.Dock = DockStyle.Bottom;
            _progressBar.Value = 0;
            this.Controls.Add(_progressBar);

            _label = new Label();
            _label.Text = "";
            _label.Dock = DockStyle.Fill;
            this.Controls.Add(_label);
        }

        public void UpdateProgress(IAsyncResult result)
        {
            if (result is Wixtracts.ExtractionProgress)
                UpdateProgress((Wixtracts.ExtractionProgress) result);
        }

        private delegate void UpdateProgressHandler(Wixtracts.ExtractionProgress progress);

        public void UpdateProgress(Wixtracts.ExtractionProgress progress)
        {
            if (this.InvokeRequired)
            {
                // This is ahack, but should be okay if needed to get around invoke
                this.Invoke(new UpdateProgressHandler(this.UpdateProgress), new object[] {progress});
                return;
            }

            _progressBar.Minimum = 0;
            _progressBar.Maximum = progress.TotalFileCount;
            _progressBar.Value = progress.FilesExtractedSoFar;
            string details;
            if (progress.Activity == Wixtracts.ExtractionActivity.ExtractingFile)
                details = "Extracting file '" + progress.CurrentFileName + "'";
            else
                details = Enum.GetName(typeof (Wixtracts.ExtractionActivity), progress.Activity);

            _label.Text = String.Format("Extracting ({0})...", details);
            this.Invalidate(true);
            this.Update();
        }

        internal void ShowAnyFinalMessages()
        {
            if (this._errorFiles.Count > 0)
            {
                const int MAX_FILES = 10;
                var displayList = this._errorFiles.GetRange(0, Math.Min(MAX_FILES, this._errorFiles.Count));
                var displayString = string.Join(", ", displayList);
                var displayDelta = _errorFiles.Count - displayList.Count;
                string postfix = displayDelta > 0 ? string.Format("...and {0} more", displayDelta) : "";
                MessageBox.Show(this,
                    string.Format("The following files failed to extract: {0}{1}.", displayString, postfix)
                );
            }
        }

        internal Wixtracts.ExtractionErrorResponse ExtractionErrorHandler(Wixtracts.ExtractionProgress progressState, string error, string fileName, string cabinetName)
        {
            this._errorFiles.Add(fileName);
            if (this._continuePromptingErrors)
            {
                var result = MessageBox.Show(this,
                    string.Format("The following error occurred extracting a file:\r\n{0}\r\n Choose 'Abort' to abort extraction. Choose 'Retry' to attempt to continue extracting files, but continue reporting future errors. Choose 'Ignore' to ignore all future errors and continue extraction.", fileName),
                    "Extraction Error",
                    MessageBoxButtons.AbortRetryIgnore
                );
                this._continuePromptingErrors = (result != DialogResult.Ignore);
                return result == DialogResult.Abort ? Wixtracts.ExtractionErrorResponse.Abort : Wixtracts.ExtractionErrorResponse.Continue;
            }
            else
            {
                return Wixtracts.ExtractionErrorResponse.Continue;
            } 
        }
    }
}