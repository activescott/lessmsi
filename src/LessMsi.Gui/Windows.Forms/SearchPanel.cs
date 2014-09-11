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

        public SearchPanel() {
            InitializeComponent();
	        this.Height = tbSearchText.Height + this.Padding.Top + this.Padding.Bottom;
        }

        private void tbSearchText_TextChanged(object sender, EventArgs e) {
            if (SearchTermChanged != null) {
                SearchTermChanged(this, new EnterEventArgs() { SearchString = tbSearchText.Text });
            }
        }

        private void tbSearchText_Leave(object sender, EventArgs e) {
	        this.Close();
        }

		private void tbSearchText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				this.Close();
			}
		}
    }

    public class EnterEventArgs : EventArgs{
        public string SearchString { get; set; }
    }
}
