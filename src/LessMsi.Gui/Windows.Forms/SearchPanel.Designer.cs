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
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbSearchText
			// 
			this.tbSearchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbSearchText.Location = new System.Drawing.Point(2, 2);
			this.tbSearchText.Name = "tbSearchText";
			this.tbSearchText.Size = new System.Drawing.Size(193, 20);
			this.tbSearchText.TabIndex = 0;
			this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
			this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
			// 
			// btnCancel
			// 
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCancel.Location = new System.Drawing.Point(195, 2);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.btnCancel.Size = new System.Drawing.Size(75, 20);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// SearchPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(272, 24);
			this.ControlBox = false;
			this.Controls.Add(this.tbSearchText);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MinimizeBox = false;
			this.Name = "SearchPanel";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSearchText;
		private System.Windows.Forms.Button btnCancel;
    }
}
