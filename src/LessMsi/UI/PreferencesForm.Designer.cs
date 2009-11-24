namespace LessMsi.UI
{
	partial class PreferencesForm
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
			this.cmdAddShortcut = new CommandLink();
			this.cmdRemoveShortcut = new CommandLink();
			this.creator = new System.Windows.Forms.LinkLabel();
			this.btnOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cmdAddShortcut
			// 
			this.cmdAddShortcut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cmdAddShortcut.Location = new System.Drawing.Point(12, 16);
			this.cmdAddShortcut.Name = "cmdAddShortcut";
			this.cmdAddShortcut.ShowElevationShield = true;
			this.cmdAddShortcut.Size = new System.Drawing.Size(344, 72);
			this.cmdAddShortcut.TabIndex = 1;
			this.cmdAddShortcut.Text = "Add Shortcut Item to Explorer";
			this.cmdAddShortcut.TextNote = "Adds an \'Extract\' menu item to the right-click context menu of .msi files";
			this.cmdAddShortcut.UseVisualStyleBackColor = true;
			this.cmdAddShortcut.Click += new System.EventHandler(this.cmdAddRemoveShortcut_Click);
			// 
			// cmdRemoveShortcut
			// 
			this.cmdRemoveShortcut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cmdRemoveShortcut.Location = new System.Drawing.Point(12, 94);
			this.cmdRemoveShortcut.Name = "cmdRemoveShortcut";
			this.cmdRemoveShortcut.ShowElevationShield = true;
			this.cmdRemoveShortcut.Size = new System.Drawing.Size(344, 60);
			this.cmdRemoveShortcut.TabIndex = 2;
			this.cmdRemoveShortcut.Text = "Remove Shortcut Item";
			this.cmdRemoveShortcut.TextNote = "Removes the context menu if it exists";
			this.cmdRemoveShortcut.UseVisualStyleBackColor = true;
			this.cmdRemoveShortcut.Click += new System.EventHandler(this.cmdAddRemoveShortcut_Click);
			// 
			// creator
			// 
			this.creator.Dock = System.Windows.Forms.DockStyle.Top;
			this.creator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.creator.Location = new System.Drawing.Point(0, 0);
			this.creator.Name = "creator";
			this.creator.Size = new System.Drawing.Size(368, 13);
			this.creator.TabIndex = 3;
			this.creator.TabStop = true;
			this.creator.Text = "Created by Scott Willeke  (http://blog.scott.willeke.com)";
			this.creator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.creator.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.creator_LinkClicked);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOk.Location = new System.Drawing.Point(281, 171);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// PreferencesForm
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOk;
			this.ClientSize = new System.Drawing.Size(368, 206);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.creator);
			this.Controls.Add(this.cmdRemoveShortcut);
			this.Controls.Add(this.cmdAddShortcut);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesForm";
			this.Text = "LessMSIerables Preferences";
			this.ResumeLayout(false);

		}

		#endregion

		private CommandLink cmdAddShortcut;
		private CommandLink cmdRemoveShortcut;
		private System.Windows.Forms.LinkLabel creator;
		private System.Windows.Forms.Button btnOk;

	}
}