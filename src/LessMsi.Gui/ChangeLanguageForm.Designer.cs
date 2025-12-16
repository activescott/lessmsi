namespace LessMsi.Gui
{
    partial class ChangeLanguageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveBtn = new System.Windows.Forms.Button();
            this.checkBoxesPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // panel1 (Bottom button area)
            // 
            this.panel1.Controls.Add(this.saveBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom; // Dock to bottom
            this.panel1.Location = new System.Drawing.Point(0, 397);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 80); // Reduced height slightly for a more compact look
            this.panel1.TabIndex = 0;
            
            // 
            // saveBtn
            // 
            // Center the button
            this.saveBtn.Anchor = System.Windows.Forms.AnchorStyles.None; 
            this.saveBtn.Location = new System.Drawing.Point(72, 20); 
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(257, 39);
            this.saveBtn.TabIndex = 0;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            
            // 
            // checkBoxesPanel (Language list area)
            // 
            this.checkBoxesPanel.Dock = System.Windows.Forms.DockStyle.Fill; // Fill remaining space
            this.checkBoxesPanel.AutoScroll = true; // [Key Change] Enable auto-scrolling
            this.checkBoxesPanel.Location = new System.Drawing.Point(0, 0);
            this.checkBoxesPanel.Name = "checkBoxesPanel";
            this.checkBoxesPanel.Size = new System.Drawing.Size(400, 397);
            this.checkBoxesPanel.TabIndex = 1;
            
            // 
            // ChangeLanguageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // [Key Change] Increase width to 400 (was 291) to ensure content isn't cut off
            this.ClientSize = new System.Drawing.Size(400, 480); 
            this.Controls.Add(this.checkBoxesPanel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; // Changed to FixedDialog to look more like a dialog
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Suggestion: Center on parent
            this.Name = "ChangeLanguageForm";
            this.Text = "Change Language";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChangeLanguageForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel checkBoxesPanel;
        private System.Windows.Forms.Button saveBtn;
    }
}
