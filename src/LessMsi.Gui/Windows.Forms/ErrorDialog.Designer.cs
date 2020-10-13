namespace LessMsi.Gui.Windows.Forms
{
    partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblErrorIntro = new System.Windows.Forms.Label();
            this.flowTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPleaseReportLink = new System.Windows.Forms.LinkLabel();
            this.grpErrorDetail = new System.Windows.Forms.GroupBox();
            this.txtErrorDetail = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.flowTop.SuspendLayout();
            this.grpErrorDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 371);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(540, 52);
            this.panel1.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(439, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(87, 27);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lblErrorIntro
            // 
            this.lblErrorIntro.AutoSize = true;
            this.lblErrorIntro.Location = new System.Drawing.Point(3, 0);
            this.lblErrorIntro.Name = "lblErrorIntro";
            this.lblErrorIntro.Size = new System.Drawing.Size(0, 15);
            this.lblErrorIntro.TabIndex = 2;
            // 
            // flowTop
            // 
            this.flowTop.Controls.Add(this.lblErrorIntro);
            this.flowTop.Controls.Add(this.lblPleaseReportLink);
            this.flowTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowTop.Location = new System.Drawing.Point(0, 0);
            this.flowTop.Name = "flowTop";
            this.flowTop.Size = new System.Drawing.Size(540, 100);
            this.flowTop.TabIndex = 2;
            // 
            // lblPleaseReportLink
            // 
            this.lblPleaseReportLink.AutoSize = true;
            this.lblPleaseReportLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPleaseReportLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPleaseReportLink.LinkArea = new System.Windows.Forms.LinkArea(243, 46);
            this.lblPleaseReportLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lblPleaseReportLink.Location = new System.Drawing.Point(3, 15);
            this.lblPleaseReportLink.Name = "lblPleaseReportLink";
            this.lblPleaseReportLink.Size = new System.Drawing.Size(534, 69);
            this.lblPleaseReportLink.TabIndex = 3;
            this.lblPleaseReportLink.TabStop = true;
            this.lblPleaseReportLink.Text = resources.GetString("lblPleaseReportLink.Text");
            this.lblPleaseReportLink.UseCompatibleTextRendering = true;
            this.lblPleaseReportLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblPleaseReportLink_LinkClicked);
            // 
            // grpErrorDetail
            // 
            this.grpErrorDetail.Controls.Add(this.txtErrorDetail);
            this.grpErrorDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpErrorDetail.Location = new System.Drawing.Point(0, 100);
            this.grpErrorDetail.Name = "grpErrorDetail";
            this.grpErrorDetail.Size = new System.Drawing.Size(540, 271);
            this.grpErrorDetail.TabIndex = 3;
            this.grpErrorDetail.TabStop = false;
            this.grpErrorDetail.Text = "Error Detail:";
            // 
            // txtErrorDetail
            // 
            this.txtErrorDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorDetail.Location = new System.Drawing.Point(3, 19);
            this.txtErrorDetail.Multiline = true;
            this.txtErrorDetail.Name = "txtErrorDetail";
            this.txtErrorDetail.ReadOnly = true;
            this.txtErrorDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorDetail.Size = new System.Drawing.Size(534, 249);
            this.txtErrorDetail.TabIndex = 0;
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 423);
            this.ControlBox = false;
            this.Controls.Add(this.grpErrorDetail);
            this.Controls.Add(this.flowTop);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error!";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.flowTop.ResumeLayout(false);
            this.flowTop.PerformLayout();
            this.grpErrorDetail.ResumeLayout(false);
            this.grpErrorDetail.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblErrorIntro;
        private System.Windows.Forms.FlowLayoutPanel flowTop;
        private System.Windows.Forms.LinkLabel lblPleaseReportLink;
        private System.Windows.Forms.GroupBox grpErrorDetail;
        private System.Windows.Forms.TextBox txtErrorDetail;
    }
}