using LessMsi.Gui.Resources.Languages;
using LessMsi.Gui.Windows.Forms;

namespace LessMsi.Gui
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
			this.cmdAddShortcut = new ElevationButton();
			this.cmdRemoveShortcut = new ElevationButton();
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
			this.cmdAddShortcut.Text = Strings.AddShortcutText;
			this.cmdAddShortcut.TextNote = Strings.AddShortcutTextNote;
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
			this.cmdRemoveShortcut.Text = Strings.RemoveShortcutText;
			this.cmdRemoveShortcut.TextNote = Strings.RemoveShortcutTextNote;
			this.cmdRemoveShortcut.UseVisualStyleBackColor = true;
			this.cmdRemoveShortcut.Click += new System.EventHandler(this.cmdAddRemoveShortcut_Click);
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
			this.Controls.Add(this.cmdRemoveShortcut);
			this.Controls.Add(this.cmdAddShortcut);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);
		}

		#endregion

		private ElevationButton cmdAddShortcut;
		private ElevationButton cmdRemoveShortcut;
		private System.Windows.Forms.Button btnOk;

	}
}