using LessMsi.Gui.Resources.Languages;

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
			this.cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbSearchText
			// 
			this.tbSearchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbSearchText.Location = new System.Drawing.Point(48, 2);
			this.tbSearchText.Name = "tbSearchText";
			this.tbSearchText.Size = new System.Drawing.Size(222, 20);
			this.tbSearchText.TabIndex = 0;
			this.tbSearchText.TextChanged += new System.EventHandler(this.tbSearchText_TextChanged);
			this.tbSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearchText_KeyDown);
			// 
			// cancelButton
			// 
			this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.cancelButton.Location = new System.Drawing.Point(270, 2);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(28, 18);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "X";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(2, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = Strings.Search;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SearchPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.tbSearchText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancelButton);
			this.MaximumSize = new System.Drawing.Size(999, 26);
			this.Name = "SearchPanel";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.Size = new System.Drawing.Size(300, 22);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TextBox tbSearchText;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
    }
}
