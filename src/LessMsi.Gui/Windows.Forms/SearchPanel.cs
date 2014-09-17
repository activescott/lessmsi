using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms
{
	internal partial class SearchPanel : UserControl
	{
		public event EventHandler<SearchTermChangedEventArgs> SearchTermChanged;

		/// <summary>
		/// Raised when the search term box is canceled. Any filtering done based on the search can be cleared at this point.
		/// </summary>
		public event EventHandler<EventArgs> SearchCanceled;
		private Control dataGridToAttachTo;

		public SearchPanel()
		{
			this.Visible = false;
			InitializeComponent();
		}

		/// <summary>
		/// Performs the search of the specified data grid.
		/// </summary>
		/// <param name="dataGridToAttachTo">This DataGrid control the search is performed on. 
		/// This control will position itself within the area of the grid.</param>
		/// <param name="searchTermChangedHandler">Handler for search term change.</param>
		/// <param name="cancelSearchingEventHandler">When the user cancels the search the caller can use this handler to clean something up.</param>
		public void SearchDataGrid(Control dataGridToAttachTo,
		                           EventHandler<SearchTermChangedEventArgs> searchTermChangedHandler,
		                           EventHandler<EventArgs> cancelSearchingEventHandler
			)
		{
			Debug.Assert(this.dataGridToAttachTo == null || object.ReferenceEquals(this.dataGridToAttachTo, dataGridToAttachTo), "expected always to be the same data grid? Something odd here.");
			this.dataGridToAttachTo = dataGridToAttachTo;
			dataGridToAttachTo.Parent.Controls.Add(this);
			SetPreferredLocationAndSize();
			this.SearchTermChanged += searchTermChangedHandler;
			this.SearchCanceled += cancelSearchingEventHandler;
			this.Parent.Resize += (sender, args) => SetPreferredLocationAndSize();
			this.cancelButton.Click += (sender, args) => this.CancelSearch();
			this.BringToFront();
			this.Visible = true;
			this.tbSearchText.Focus();
		}

		private void SetPreferredLocationAndSize()
		{
			this.SuspendLayout();
			this.Width = 300;//this.Width = dataGridToAttachTo.Width;
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
			if (SearchTermChanged != null)
			{
				SearchTermChanged(this, new SearchTermChangedEventArgs() {SearchString = tbSearchText.Text});
			}
		}

		public void CancelSearch()
		{
			if (!this.Visible)
				return;
			this.tbSearchText.Text = "";
			this.Hide();
			if (SearchCanceled != null)
				SearchCanceled(this, EventArgs.Empty);
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

	internal class SearchTermChangedEventArgs : EventArgs
	{
		public string SearchString { get; set; }
	}
}