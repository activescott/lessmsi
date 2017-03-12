using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

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
	            const string SEPERATOR = "\t";
				var sb = new StringBuilder();
	            var cols = grid.Columns.Cast<DataGridViewColumn>();
				var colNames = string.Join(SEPERATOR, cols.Select(c => c.HeaderText).ToArray());
				sb.AppendLine(colNames);
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
								sb.Append(SEPERATOR);
							sb.Append(cell.Value);
						}

						FlashDataGridRow(row);
						sb.AppendLine();
					}
				}
				Clipboard.SetText(sb.ToString());
            }
        }

		public static IWin32Window GetParentForm(Control control)
		{
			while (!(control is Form))
			{
				control = control.Parent;
			}
			Debug.Assert(control is Form, "expected control to be form!");
			return (control as Form);
		}

		public static Rectangle GetScreenRect(Control ctl)
		{
			var r = new Rectangle(0, 0, ctl.Width, ctl.Height);
			while (!(ctl is Form))
			{
				r.X += ctl.Left;
				r.Y += ctl.Top;
				ctl = ctl.Parent;
			}
			Debug.Assert(ctl is Form, "expected control to be form!");
			return (ctl as Form).RectangleToScreen(r);
		}
    }
}
