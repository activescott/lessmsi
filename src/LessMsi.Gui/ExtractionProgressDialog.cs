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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using LessMsi.Gui.Resources.Languages;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Serialize;

namespace LessMsi.Gui
{
    internal class ExtractionProgressDialog : Form
    {
        private ProgressBar _progressBar;
        private Label _label;

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

            string details = string.Empty;

            switch (progress.Activity)
            {
                case Wixtracts.ExtractionActivity.Initializing:
                    details = Strings.Initializing;
                    break;
                case Wixtracts.ExtractionActivity.Uncompressing:
                    details = Strings.Uncompressing;
                    break;
                case Wixtracts.ExtractionActivity.ExtractingFile:
                    details = $"{Strings.ExtractingFile} '{progress.CurrentFileName}'";
                    break;
                case Wixtracts.ExtractionActivity.Complete:
                    details = Strings.Complete;
                    break;
                default:
                    Debug.Fail("Unrecognised ExtractionActivity value was given");
                    break;
            }

            _label.Text = $"{Strings.Extracting} ({details})";
            this.Invalidate(true);
            this.Update();
        }
    }
}