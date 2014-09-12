namespace LessMsi.Gui.Windows.Forms {
    partial class SearchPanel {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.tbSearchText = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbSearchText
			// 
			this.tbSearchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbSearchText.Location = new System.Drawing.Point(2, 2);
			this.tbSearchText.Name = "tbSearchText";
			this.tbSearchText.Size = new System.Drawing.Size(178, 20);
			this.tbSearchText.TabIndex = 0;
			this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
			this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
			// 
			// SearchPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(182, 24);
			this.ControlBox = false;
			this.Controls.Add(this.tbSearchText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximumSize = new System.Drawing.Size(999, 26);
			this.MinimizeBox = false;
			this.Name = "SearchPanel";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResizeEnd += new System.EventHandler(this.SearchPanel_ResizeEnd);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TextBox tbSearchText;
    }
}
