using System;
using System.Windows.Forms;

namespace LessMsi.Gui.Extensions
{
	public static class DataGridViewExtensions
	{
		/// <summary>
		/// Adjusts the width of all columns to fit the contents of all cells.
		/// This function takes care of the edgecase where the grid is not created yet.
		/// (For example, when it is filled with data before the control is shown.)
		/// </summary>
		/// <param name="gridView">The target control.</param>
		public static void AutoResizeColumnsSafe(this DataGridView gridView)
		{
			if (gridView.IsHandleCreated)
			{
				gridView.AutoResizeColumns();
			}
			else
			{
				// If the handle is not created yet we cannot resize columns,
				// so delay the auto-resizing to when the control is created.
				gridView.HandleCreated += Grid_HandleCreated;
			}
		}

		private static void Grid_HandleCreated(object sender, EventArgs e)
		{
			DataGridView dgv = sender as DataGridView;
			if (dgv != null)
			{
				dgv.HandleCreated -= Grid_HandleCreated;
				dgv.AutoResizeColumns();
			}
		}

	}
}
