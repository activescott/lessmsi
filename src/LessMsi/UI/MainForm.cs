// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LessMsi.Msi;
using LessMsi.UI.Model;
using Microsoft.Tools.WindowsInstallerXml.Msi;
using System.Text;

namespace LessMsi.UI
{
    internal class MainForm : Form, IMainFormView
    {
        public MainForm(string defaultInputFile)
        {
            this.Presenter = new MainFormPresenter(this);
            InitializeComponent();
            Presenter.Initialize();
            
            if (!string.IsNullOrEmpty(defaultInputFile))
                this.txtMsiFileName.Text = defaultInputFile;
            
        }

        private MainFormPresenter Presenter { get; set; }

        #region UI Event Handlers
        private void OpenFileCommand()
        {
            if (DialogResult.OK != this.openMsiDialog.ShowDialog(this))
                return;
            this.txtMsiFileName.Text = this.openMsiDialog.FileName;
            LoadCurrentFile();
        }

        private void ReloadCurrentUIOnEnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13)
            {
                e.Handled = true;
                LoadCurrentFile();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewTable();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            var selectedFiles = new List<MsiFile>();
            if (fileGrid.SelectedRows.Count == 0)
            {
                Presenter.ShowUserMessage("Please select some or all of the files to extract them.");
                return;
            }

            if (folderBrowser.SelectedPath == null || folderBrowser.SelectedPath.Length <= 0)
                folderBrowser.SelectedPath = GetSelectedMsiFile().DirectoryName;

            if (DialogResult.OK != this.folderBrowser.ShowDialog(this))
                return;

            btnExtract.Enabled = false;
            ExtractionProgressDialog progressDialog = new ExtractionProgressDialog(this);
            progressDialog.Show();
            progressDialog.Update();
            try
            {
                DirectoryInfo outputDir = new DirectoryInfo(this.folderBrowser.SelectedPath);
                foreach (DataGridViewRow row in fileGrid.SelectedRows)
                {
                    MsiFileItemView fileToExtract = (MsiFileItemView)row.DataBoundItem;
                    selectedFiles.Add(fileToExtract.File);
                }

                FileInfo msiFile = GetSelectedMsiFile();
                if (msiFile == null)
                    return;
                var filesToExtract = selectedFiles.ToArray();
                Wixtracts.ExtractFiles(msiFile, outputDir, filesToExtract, new AsyncCallback(progressDialog.UpdateProgress));
            }
            catch (Exception err)
            {
                MessageBox.Show(this, 
                                "The following error occured extracting the MSI: " + err.ToString(), "MSI Error!", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
            }
            finally
            {
                progressDialog.Close();
                progressDialog.Dispose();
                this.btnExtract.Enabled = true;
            }
        }

        private void ChangeUiEnabled(bool doEnable)
        {
            this.btnExtract.Enabled = doEnable;
            this.cboTable.Enabled = doEnable;
        }

        #endregion

        private void LoadCurrentFile()
        {
            bool isBadFile = false;
            try
            {
                Presenter.UpdatePropertyTabView();
                Presenter.LoadTables();
                Presenter.ViewFiles();
                ViewTable();
            }
            catch (Exception eCatchAll)
            {
                isBadFile = true;
                Presenter.Error("Failed to open file.", eCatchAll);
            }
            ChangeUiEnabled(!isBadFile);
        }

        


       

        private void ViewTable()
        {
            using (Database msidb = new Database(GetSelectedMsiFile().FullName, OpenDatabase.ReadOnly))
            {
                string tableName = this.cboTable.Text;
                ViewTable(msidb, tableName);
            }
        }

        /// <summary>
        /// Shows the table in the list on the view table tab.
        /// </summary>
        /// <param name="msidb">The msi database.</param>
        /// <param name="tableName">The name of the table.</param>
        private void ViewTable(Database msidb, string tableName)
        {
            if (msidb == null || string.IsNullOrEmpty(tableName))
                return;

            Presenter.Status(string.Concat("Processing Table \'", tableName, "\'."));

            using (new DisposableCursor(this))
            {
                try
                {
                    tableList.Clear();
                    if (!msidb.TableExists(tableName))
                    {
                        Presenter.Error("Table \'" + tableName + "' does not exist.", null);
                        return;
                    }

                    string query = string.Concat("SELECT * FROM `", tableName, "`");

                    using (ViewWrapper view = new ViewWrapper(msidb.OpenExecuteView(query)))
                    {
                        int colWidth = this.tableList.ClientRectangle.Width/view.Columns.Length;
                        foreach (ColumnInfo col in view.Columns)
                        {
                            ColumnHeader header = new ColumnHeader();
                            header.Text = string.Concat(col.Name, " (", col.TypeID, ")");
                            header.Width = colWidth;
                            tableList.Columns.Add(header);
                        }

                        foreach (object[] values in view.Records)
                        {
                            ListViewItem item = new ListViewItem(Convert.ToString(values[0]));
                            for (int colIndex = 1; colIndex < values.Length; colIndex++)
                                item.SubItems.Add(Convert.ToString(values[colIndex]));
                            tableList.Items.Add(item);
                        }
                    }
                    Presenter.Status("Idle");
                }
                catch (Exception eUnexpected)
                {
                    Presenter.Error(string.Concat("Cannot view table:", eUnexpected.Message), eUnexpected);
                }
            }

        }

        public FileInfo GetSelectedMsiFile()
        {
            FileInfo file = new FileInfo(this.txtMsiFileName.Text);
            if (!file.Exists)
            {
                Presenter.Error(string.Concat("File \'", file.FullName, "\' does not exist."), null);
                return null;
            }
            return file;
        }

        private StatusBar statusBar1;
        internal StatusBarPanel statusPanelDefault;
        private StatusBarPanel statusPanelFileCount;
        private Button btnSelectAll;
        private Button btnUnselectAll;
        private TabPage tabSummary;
        private TextBox txtSummaryDescription;
        private GroupBox grpDescription;
        private Panel panel2;
        public ListView propertiesList;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem preferencesToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        public DataGridView fileGrid;

        #region Designer Stuff

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.txtMsiFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabExtractFiles = new System.Windows.Forms.TabPage();
            this.fileGrid = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnUnselectAll = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.tabTableView = new System.Windows.Forms.TabPage();
            this.cboTable = new System.Windows.Forms.ComboBox();
            this.tableList = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.propertiesList = new System.Windows.Forms.ListView();
            this.grpDescription = new System.Windows.Forms.GroupBox();
            this.txtSummaryDescription = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.openMsiDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusPanelDefault = new System.Windows.Forms.StatusBarPanel();
            this.statusPanelFileCount = new System.Windows.Forms.StatusBarPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs.SuspendLayout();
            this.tabExtractFiles.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabTableView.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.grpDescription.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMsiFileName
            // 
            this.txtMsiFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMsiFileName.Location = new System.Drawing.Point(46, 4);
            this.txtMsiFileName.Name = "txtMsiFileName";
            this.txtMsiFileName.Size = new System.Drawing.Size(247, 20);
            this.txtMsiFileName.TabIndex = 0;
            this.txtMsiFileName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReloadCurrentUIOnEnterKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&File:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBrowse.Location = new System.Drawing.Point(300, 7);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(20, 17);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabExtractFiles);
            this.tabs.Controls.Add(this.tabTableView);
            this.tabs.Controls.Add(this.tabSummary);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 51);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(336, 299);
            this.tabs.TabIndex = 0;
            // 
            // tabExtractFiles
            // 
            this.tabExtractFiles.Controls.Add(this.fileGrid);
            this.tabExtractFiles.Controls.Add(this.panel2);
            this.tabExtractFiles.Location = new System.Drawing.Point(4, 22);
            this.tabExtractFiles.Name = "tabExtractFiles";
            this.tabExtractFiles.Padding = new System.Windows.Forms.Padding(5);
            this.tabExtractFiles.Size = new System.Drawing.Size(328, 273);
            this.tabExtractFiles.TabIndex = 0;
            this.tabExtractFiles.Text = "Extract Files";
            // 
            // fileGrid
            // 
            this.fileGrid.AllowUserToAddRows = false;
            this.fileGrid.AllowUserToDeleteRows = false;
            this.fileGrid.AllowUserToOrderColumns = true;
            this.fileGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileGrid.Location = new System.Drawing.Point(5, 5);
            this.fileGrid.Name = "fileGrid";
            this.fileGrid.ReadOnly = true;
            this.fileGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fileGrid.Size = new System.Drawing.Size(318, 231);
            this.fileGrid.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSelectAll);
            this.panel2.Controls.Add(this.btnUnselectAll);
            this.panel2.Controls.Add(this.btnExtract);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 236);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(318, 32);
            this.panel2.TabIndex = 4;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectAll.Location = new System.Drawing.Point(0, 8);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 1;
            this.btnSelectAll.Text = "Select &All";
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnUnselectAll
            // 
            this.btnUnselectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnUnselectAll.Location = new System.Drawing.Point(88, 8);
            this.btnUnselectAll.Name = "btnUnselectAll";
            this.btnUnselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnUnselectAll.TabIndex = 2;
            this.btnUnselectAll.Text = "&Unselect All";
            this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Enabled = false;
            this.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExtract.Location = new System.Drawing.Point(241, 8);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 3;
            this.btnExtract.Text = "E&xtract";
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // tabTableView
            // 
            this.tabTableView.Controls.Add(this.cboTable);
            this.tabTableView.Controls.Add(this.tableList);
            this.tabTableView.Controls.Add(this.label2);
            this.tabTableView.Location = new System.Drawing.Point(4, 22);
            this.tabTableView.Name = "tabTableView";
            this.tabTableView.Size = new System.Drawing.Size(328, 273);
            this.tabTableView.TabIndex = 1;
            this.tabTableView.Text = "Table View";
            // 
            // cboTable
            // 
            this.cboTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTable.Enabled = false;
            this.cboTable.Location = new System.Drawing.Point(48, 7);
            this.cboTable.Name = "cboTable";
            this.cboTable.Size = new System.Drawing.Size(271, 21);
            this.cboTable.TabIndex = 8;
            this.cboTable.Text = "File";
            this.cboTable.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.cboTable.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReloadCurrentUIOnEnterKeyPress);
            // 
            // tableList
            // 
            this.tableList.AllowColumnReorder = true;
            this.tableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableList.FullRowSelect = true;
            this.tableList.GridLines = true;
            this.tableList.LabelWrap = false;
            this.tableList.Location = new System.Drawing.Point(9, 35);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(310, 232);
            this.tableList.TabIndex = 7;
            this.tableList.UseCompatibleStateImageBehavior = false;
            this.tableList.View = System.Windows.Forms.View.Details;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "&Table";
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.propertiesList);
            this.tabSummary.Controls.Add(this.grpDescription);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(5);
            this.tabSummary.Size = new System.Drawing.Size(328, 273);
            this.tabSummary.TabIndex = 2;
            this.tabSummary.Text = "Summary";
            // 
            // propertiesList
            // 
            this.propertiesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesList.FullRowSelect = true;
            this.propertiesList.GridLines = true;
            this.propertiesList.HideSelection = false;
            this.propertiesList.Location = new System.Drawing.Point(5, 5);
            this.propertiesList.Name = "propertiesList";
            this.propertiesList.Size = new System.Drawing.Size(318, 171);
            this.propertiesList.TabIndex = 2;
            this.propertiesList.UseCompatibleStateImageBehavior = false;
            this.propertiesList.View = System.Windows.Forms.View.Details;
            this.propertiesList.SelectedIndexChanged += new System.EventHandler(this.lstSummaryProperties_SelectedIndexChanged);
            // 
            // grpDescription
            // 
            this.grpDescription.Controls.Add(this.txtSummaryDescription);
            this.grpDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpDescription.Location = new System.Drawing.Point(5, 176);
            this.grpDescription.Name = "grpDescription";
            this.grpDescription.Size = new System.Drawing.Size(318, 92);
            this.grpDescription.TabIndex = 2;
            this.grpDescription.TabStop = false;
            this.grpDescription.Text = "Description:";
            // 
            // txtSummaryDescription
            // 
            this.txtSummaryDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSummaryDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSummaryDescription.Location = new System.Drawing.Point(3, 16);
            this.txtSummaryDescription.Multiline = true;
            this.txtSummaryDescription.Name = "txtSummaryDescription";
            this.txtSummaryDescription.ReadOnly = true;
            this.txtSummaryDescription.Size = new System.Drawing.Size(312, 73);
            this.txtSummaryDescription.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtMsiFileName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(336, 27);
            this.panel1.TabIndex = 0;
            // 
            // openMsiDialog
            // 
            this.openMsiDialog.DefaultExt = "msi";
            this.openMsiDialog.Filter = "msierablefiles|*.msi|All Files|*.*";
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 350);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusPanelDefault,
            this.statusPanelFileCount});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(336, 16);
            this.statusBar1.TabIndex = 2;
            // 
            // statusPanelDefault
            // 
            this.statusPanelDefault.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusPanelDefault.Name = "statusPanelDefault";
            this.statusPanelDefault.Width = 209;
            // 
            // statusPanelFileCount
            // 
            this.statusPanelFileCount.Name = "statusPanelFileCount";
            this.statusPanelFileCount.Width = 110;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(336, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.toolStripSeparator1,
            this.preferencesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(336, 366);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(272, 184);
            this.Name = "MainForm";
            this.Text = "Less MSIérables";
            this.tabs.ResumeLayout(false);
            this.tabExtractFiles.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabTableView.ResumeLayout(false);
            this.tabTableView.PerformLayout();
            this.tabSummary.ResumeLayout(false);
            this.grpDescription.ResumeLayout(false);
            this.grpDescription.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtMsiFileName;
        private Label label1;
        private Button btnBrowse;
        private TabControl tabs;
        private TabPage tabExtractFiles;
        private TabPage tabTableView;
        public ComboBox cboTable;
        private ListView tableList;
        private Label label2;
        private Panel panel1;
        private Button btnExtract;
        private FolderBrowserDialog folderBrowser;
        private OpenFileDialog openMsiDialog;

        #endregion

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            Presenter.ToggleSelectAllFiles(true);
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            Presenter.ToggleSelectAllFiles(false);
        }

        private void lstSummaryProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertiesList.SelectedItems.Count > 0)
            {
                PropertyInfoListViewItem info = propertiesList.SelectedItems[0] as PropertyInfoListViewItem;
                if (info != null)
                    this.txtSummaryDescription.Text = info.Description;
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new UI.PreferencesForm();
            frm.ShowDialog(this);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // copying the currently selected row in the currently selected ListView here:
            ListView lv = this.ActiveControl as ListView;
            if (lv != null)
            {
                var item = lv.FocusedItem;
                if (item != null)
                {
                    var sb = new StringBuilder();
                    int i = 0;
                    foreach (System.Windows.Forms.ListViewItem.ListViewSubItem li in item.SubItems)
                    {
                        if (i++ > 0)
                            sb.Append(", ");
                        sb.Append(li.Text);
                    }
                    Clipboard.SetText(sb.ToString());
                    FlashBackColor(item);
                }
            }
        }

        private void FlashBackColor(ListViewItem item)
        {
            ListView lv = item.ListView;
            Rectangle r = lv.GetItemRect(item.Index);
            r.Inflate(0, 2);
            using (var g = lv.CreateGraphics())
            {
                g.DrawRectangle(SystemPens.Highlight, r);
            }
            Timer t = new Timer();
            t.Tick += (obj, ea) =>
                          {
                              r.Inflate(2, 2);
                              lv.Invalidate(r);
                              ((Timer)obj).Stop();
                          };
            t.Interval = 120;
            t.Start();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileCommand();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileCommand();
        }
    }
}