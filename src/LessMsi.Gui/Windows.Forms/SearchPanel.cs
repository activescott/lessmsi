using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms {
    public partial class SearchPanel : Form {
        public event EventHandler<EnterEventArgs> SearchTermChanged;
		/// <summary>
		/// Raised when the search term box is canceled. Any filtering done based on the search can be cleared at this point.
		/// </summary>
		public event EventHandler<EventArgs> SearchCanceled;

        public SearchPanel() {
            InitializeComponent();
	        this.Height = tbSearchText.Height + this.Padding.Top + this.Padding.Bottom;
        }

        private void tbSearchText_TextChanged(object sender, EventArgs e) {
            if (SearchTermChanged != null) {
                SearchTermChanged(this, new EnterEventArgs() { SearchString = tbSearchText.Text });
            }
        }

		private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				CancelSearch();
		}

	    private void CancelSearch()
	    {
		    this.Hide();
			if (SearchCanceled != null)
			    SearchCanceled(this, EventArgs.Empty);
			this.Close();
	    }

	    private void btnCancel_Click(object sender, EventArgs e)
	    {
			CancelSearch();
	    }
    }

    public class EnterEventArgs : EventArgs{
        public string SearchString { get; set; }
    }
}
