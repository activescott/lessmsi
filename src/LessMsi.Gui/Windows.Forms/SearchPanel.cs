using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms {
    public partial class SearchPanel : UserControl {
        public event EventHandler<EnterEventArgs> EnterPressed;

        public SearchPanel() {
            InitializeComponent();
        }

        private void tbSearchText_Enter(object sender, EventArgs e) {
            if (EnterPressed != null) {
                EnterPressed(this, new EnterEventArgs() {SearchString = tbSearchText.Text });
            }
        }
    }

    public class EnterEventArgs : EventArgs{
        public string SearchString { get; set; }
    }
}
