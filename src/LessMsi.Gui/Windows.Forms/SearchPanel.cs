using System;
using System.Drawing;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms
{
	internal partial class SearchPanel : UserControl
	{
		private Action<DataGridView, string> searchTermChanged;
		private Action searchCanceled;
		private DataGridView dataGridToAttachTo;

		public SearchPanel()
		{
			this.Visible = false;
			InitializeComponent();
			this.cancelButton.Click += (sender, args) => this.CancelSearch();
		}

		/// <summary>
		/// Performs the search of the specified data grid.
		/// </summary>
		/// <param name="dataGridToAttachTo">This DataGrid control the search is performed on. 
		/// This control will position itself within the area of the grid.</param>
		/// <param name="searchTermChanged">Handler for search term change.</param>
		/// <param name="searchCanceled">When the user cancels the search the caller can use this handler to clean something up.</param>
		public void SearchDataGrid(DataGridView dataGridToAttachTo,
								   Action<DataGridView, string> searchTermChanged,
								   Action searchCanceled
			)
		{
			if (this.dataGridToAttachTo != null && !object.ReferenceEquals(this.dataGridToAttachTo, dataGridToAttachTo))
			{
				this.Parent.Resize -= Parent_Resize;
				this.dataGridToAttachTo.Parent.Controls.Remove(this);
			}

			this.dataGridToAttachTo = dataGridToAttachTo;
			dataGridToAttachTo.Parent.Controls.Add(this);
			SetPreferredLocationAndSize();
			this.searchTermChanged = searchTermChanged;
			this.searchCanceled = searchCanceled;
			this.Parent.Resize += Parent_Resize;
			this.BringToFront();
			this.Visible = true;
			this.tbSearchText.Focus();
		}

		private void Parent_Resize(object sender, EventArgs e)
		{
			SetPreferredLocationAndSize();
		}

		private void SetPreferredLocationAndSize()
		{
			this.SuspendLayout();
			this.Width = 300;
			this.ClientSize = new Size(this.ClientSize.Width, PreferredClientHeight);
			this.ResumeLayout(true);
			this.Location = new Point(dataGridToAttachTo.Left, dataGridToAttachTo.Height - this.Height);
		}

		private int PreferredClientHeight
		{
			get { return this.tbSearchText.Bottom + this.Padding.Bottom; }
		}

		private void tbSearchText_TextChanged(object sender, EventArgs e)
		{
			searchTermChanged?.Invoke(this.dataGridToAttachTo, tbSearchText.Text);
		}

		public void CancelSearch()
		{
			if (!this.Visible)
				return;
			this.tbSearchText.Text = "";
			this.Hide();
			searchCanceled?.Invoke();
		}

		private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				CancelSearch();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}
	}
}
