using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms
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

        public static void FlashDataGridRow(DataGridViewRow row)
        {
            var grid = row.DataGridView;
            var r = grid.GetRowDisplayRectangle(row.Index, false);
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

        public static void CopySelectedDataGridRowsToClipboard(DataGridView grid)
        {
            if (grid != null && grid.SelectedRows.Count > 0)
			{
				var sb = new StringBuilder();
				//NOTE: the grid.SelectedRows is in a different order than they are in the grid. So enumerating .Rows is better
				for (int iRow = 0; iRow < grid.Rows.Count; iRow++)
				{
					DataGridViewRow row = grid.Rows[iRow];
					if (row.Selected)
					{
						int i = 0;
						foreach (DataGridViewCell cell in row.Cells)
						{
							if (i++ > 0)
								sb.Append(", ");
							sb.Append(cell.Value);
						}

						FlashDataGridRow(row);
						sb.AppendLine();
					}
				}
				Clipboard.SetText(sb.ToString());
            }
        }
    }
}
