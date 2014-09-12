using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms {
    public partial class SearchPanel : Form {
        public event EventHandler<SearchTermChangedEventArgs> SearchTermChanged;
		/// <summary>
		/// Raised when the search term box is canceled. Any filtering done based on the search can be cleared at this point.
		/// </summary>
		public event EventHandler<EventArgs> SearchCanceled;

        public SearchPanel()
		{
            InitializeComponent();
			SetPreferredFormSize();
        }

		private void SearchPanel_ResizeEnd(object sender, EventArgs e)
		{
			SetPreferredFormSize();
		}

	    private void SetPreferredFormSize()
	    {
			this.ClientSize = new Size(this.ClientSize.Width, PreferredHeight);
			this.PerformLayout();
	    }

	    private int PreferredHeight
	    {
		    get { return this.tbSearchText.Bottom + this.Padding.Bottom; }
	    }

	    private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            if (SearchTermChanged != null) {
                SearchTermChanged(this, new SearchTermChangedEventArgs() { SearchString = tbSearchText.Text });
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
			this.Width = dataGridToAttachTo.Width;
			var screenRect = WinFormsHelper.GetScreenRect(dataGridToAttachTo);
			this.Location = new Point(screenRect.X, screenRect.Bottom - this.Height);
			this.SearchTermChanged += searchTermChangedHandler;
			this.SearchCanceled += cancelSearchingEventHandler;
			this.Show(WinFormsHelper.GetParentForm(dataGridToAttachTo));
			this.tbSearchText.Focus();
		}

		private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				CancelSearch();
			}
		}
    }

    public class SearchTermChangedEventArgs : EventArgs{
        public string SearchString { get; set; }
    }
}
