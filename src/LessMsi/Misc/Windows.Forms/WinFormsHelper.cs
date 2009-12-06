using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Misc.Windows.Forms
{
	/// <summary>
	/// Contains misc generic helper methods for dealing with <see cref="System.Windows.Forms"/> controls.
	/// </summary>
	public class WinFormsHelper
    {
        /// <summary>
        /// Allows a c# using statement to be used for the <see cref="ISupportInitialize"/> contract.
        /// </summary>
        public static IDisposable BeginUiUpdate(ISupportInitialize control)
        {
            control.BeginInit();
            return new ControlInitializationToken(control);
        }

        private struct ControlInitializationToken : IDisposable
        {
            private ISupportInitialize _control;

            public ControlInitializationToken(ISupportInitialize control)
            {
                _control = control;
            }

            public void Dispose()
            {
                _control.EndInit();
            }
        }

        public static void FlashDataGridRow(DataGridViewRow item)
        {
            var grid = item.DataGridView;
            var r = grid.GetRowDisplayRectangle(item.Index, true);
            r.Inflate(0, 2);
            using (var g = grid.CreateGraphics())
            {
                g.DrawRectangle(SystemPens.Highlight, r);
            }
            var t = new Timer();
            t.Tick += (obj, ea) =>
                          {
                              r.Inflate(2, 2);
                              grid.Invalidate(r);
                              ((Timer)obj).Stop();
                          };
            t.Interval = 120;
            t.Start();
        }

        public static void CopySelectedDataGridRowToClipboard(DataGridView grid)
        {
            if (grid != null)
            {
                var row = grid.SelectedRows.Count > 0 ? grid.SelectedRows[0] : null;
                if (row != null)
                {
                    var sb = new StringBuilder();
                    int i = 0;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (i++ > 0)
                            sb.Append(", ");
                        sb.Append(cell.Value);
                    }
                    Clipboard.SetText(sb.ToString());
                }
            }
        }
    }
}
