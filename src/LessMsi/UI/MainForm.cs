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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;
using System.Text;

namespace LessMsi.UI
{
    internal class MainForm : Form
    {
        public MainForm(string defaultInputFile)
        {
            InitializeComponent();
            MsiFileListViewItem.InitListViewColumns(fileList);
            PropertyInfoListViewItem.InitListViewColumns(this.propertiesList);
            if (defaultInputFile != null && defaultInputFile.Length > 0)
                this.txtMsiFileName.Text = defaultInputFile;
        }

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
            ArrayList /*<MsiFile>*/ selectedFiles = new ArrayList();
            if (this.fileList.CheckedItems.Count == 0)
                return;

            if (this.folderBrowser.SelectedPath == null || folderBrowser.SelectedPath.Length <= 0)
                this.folderBrowser.SelectedPath = this.GetSelectedMsiFile().DirectoryName;

            if (DialogResult.OK != this.folderBrowser.ShowDialog(this))
                return;

            this.btnExtract.Enabled = false;
            ExtractionProgressDialog progressDialog = new ExtractionProgressDialog(this);
            progressDialog.Show();
            progressDialog.Update();
            try
            {
                DirectoryInfo outputDir = new DirectoryInfo(this.folderBrowser.SelectedPath);
                foreach (MsiFileListViewItem item in this.fileList.Items)
                {
                    if (item.Checked)
                        selectedFiles.Add(item._file);
                }

                FileInfo msiFile = GetSelectedMsiFile();
                if (msiFile == null)
                    return;
                MsiFile[] filesToExtract = (MsiFile[])selectedFiles.ToArray(typeof(MsiFile));
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
                ViewSummary();
                LoadTables();
                ViewFiles();
                ViewTable();
            }
            catch (Exception eCatchAll)
            {
                isBadFile = true;
                Error("Failed to open file.", eCatchAll);
            }
            ChangeUiEnabled(!isBadFile);
        }

        private void LoadTables()
        {
            this.cboTable.Items.Clear();
            this.cboTable.Items.AddRange(new object[]
            {//FYI: This list is from http://msdn.microsoft.com/en-us/library/2k3te2cs%28VS.100%29.aspx
                "ActionText",
                "AdminExecuteSequence ",
                "AdminUISequence",
                "AdvtExecuteSequence",
                "AdvtUISequence",
                "AppId",
                "AppSearch",
                "BBControl",
                "Billboard",
                "Binary",
                "BindImage",
                "CCPSearch",
                "CheckBox",
                "Class",
                "ComboBox",
                "CompLocator",
                "Complus",
                "Component",
                "Condition",
                "Control",
                "ControlCondition",
                "ControlEvent",
                "CreateFolder",
                "CustomAction",
                "Dialog",
                "Directory",
                "DrLocator",
                "DuplicateFile",
                "Environment",
                "Error",
                "EventMapping",
                "Extension",
                "Feature",
                "FeatureComponents",
                "File",
                "FileSFPCatalog",
                "Font",
                "Icon",
                "IniFile",
                "IniLocator",
                "InstallExecuteSequence",
                "InstallUISequence",
                "IsolatedComponent",
                "LaunchCondition",
                "ListBox",
                "ListView",
                "LockPermissions",
                "Media",
                "MIME",
                "MoveFile",
                "MsiAssembly",
                "MsiAssemblyName",
                "MsiDigitalCertificate",
                "MsiDigitalSignature",
                "MsiEmbeddedChainer",
                "MsiEmbeddedUI",
                "MsiFileHash",
                "MsiLockPermissionsEx Table",
                "MsiPackageCertificate",
                "MsiPatchCertificate",
                "MsiPatchHeaders",
                "MsiPatchMetadata",
                "MsiPatchOldAssemblyName",
                "MsiPatchOldAssemblyFile",
                "MsiPatchSequence",
                "MsiServiceConfig",
                "MsiServiceConfigFailureActions",
                "MsiSFCBypass",
                "ODBCAttribute",
                "ODBCDataSource",
                "ODBCDriver",
                "ODBCSourceAttribute",
                "ODBCTranslator",
                "Patch",
                "PatchPackage",
                "ProgId",
                "Property",
                "PublishComponent",
                "RadioButton",
                "Registry",
                "RegLocator",
                "RemoveFile",
                "RemoveIniFile",
                "RemoveRegistry",
                "ReserveCost",
                "SelfReg",
                "ServiceControl",
                "ServiceInstall",
                "SFPCatalog",
                "Shortcut",
                "Signature",
                "TextStyle",
                "TypeLib",
                "UIText",
                "Verb",
                "_Validation",
                "_Columns",
                "_Streams",
                "_Storages",
                "_Tables",
                "_TransformView Table",
                "Upgrade"
                             
                                             });
        }


        /// <summary>
        /// Updates the ui with the currently selected msi file.
        /// </summary>
        private void ViewFiles()
        {
            using (Database msidb = new Database(GetSelectedMsiFile().FullName, OpenDatabase.ReadOnly))
            {
                ViewFiles(msidb);
            }
        }

        /// <summary>
        /// Displays the list of files in the extract tab.
        /// </summary>
        /// <param name="msidb">The msi database.</param>
        private void ViewFiles(Database msidb)
        {
            if (msidb == null)
                return;

            using (new DisposableCursor(this))
            {
                //used to filter and sort columns
                Hashtable /*<string, int>*/ columnMap = new Hashtable(StringComparer.InvariantCulture);
                columnMap.Add("FileName", 0);
                columnMap.Add("FileSize", 1);
                columnMap.Add("Version", 2);
                columnMap.Add("Language", 3);
                columnMap.Add("Attributes", 4);

                try
                {
                    this.statusPanelDefault.Text = "";
                    fileList.Items.Clear();

                    foreach (MsiFile msiFile in MsiFile.CreateMsiFilesFromMSI(msidb))
                        fileList.Items.Add(new MsiFileListViewItem(msiFile));
                    statusPanelFileCount.Text = string.Concat(fileList.Items.Count, " files found.");
                }
                catch (Exception eUnexpected)
                {
                    Error(string.Concat("Cannot view files:", eUnexpected.Message), eUnexpected);
                }
            }
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
            if (msidb == null || tableName == null && tableName.Length == 0)
                return;

            Status(string.Concat("Processing Table \'", tableName, "\'."));

            using (new DisposableCursor(this))
            {
                try
                {
                    tableList.Clear();
                    if (!msidb.TableExists(tableName))
                    {
                        Error("Table \'" + tableName + "' does not exist.", null);
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
                    Status("Idle");
                }
                catch (Exception eUnexpected)
                {
                    Error(string.Concat("Cannot view table:", eUnexpected.Message), eUnexpected);
                }
            }

        }


        private FileInfo GetSelectedMsiFile()
        {
            FileInfo file = new FileInfo(this.txtMsiFileName.Text);
            if (!file.Exists)
            {
                Error(string.Concat("File \'", file.FullName, "\' does not exist."), null);
                return null;
            }
            return file;
        }

        #region Status Stuff

        private void Status(string msg)
        {
            this.statusPanelDefault.Text = "Status:" + msg;
        }

        private void Error(string msg, Exception exception)
        {
            this.statusPanelDefault.Text = "ERROR:" + msg;
            if (exception != null)
                this.statusPanelDefault.ToolTipText = exception.ToString();
            else
                this.statusPanelDefault.ToolTipText = "";

        }

        #endregion	

        private StatusBar statusBar1;
        private StatusBarPanel statusPanelDefault;
        private StatusBarPanel statusPanelFileCount;
        private Button btnSelectAll;
        private Button btnUnselectAll;
        private TabPage tabSummary;
        private TextBox txtSummaryDescription;
        private GroupBox grpDescription;
        private Panel panel2;
        private ListView propertiesList;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem preferencesToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;

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
            this.fileList = new System.Windows.Forms.ListView();
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
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tabExtractFiles.Controls.Add(this.fileList);
            this.tabExtractFiles.Controls.Add(this.panel2);
            this.tabExtractFiles.Location = new System.Drawing.Point(4, 22);
            this.tabExtractFiles.Name = "tabExtractFiles";
            this.tabExtractFiles.Padding = new System.Windows.Forms.Padding(5);
            this.tabExtractFiles.Size = new System.Drawing.Size(328, 273);
            this.tabExtractFiles.TabIndex = 0;
            this.tabExtractFiles.Text = "Extract Files";
            // 
            // fileList
            // 
            this.fileList.AllowColumnReorder = true;
            this.fileList.CheckBoxes = true;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FullRowSelect = true;
            this.fileList.GridLines = true;
            this.fileList.LabelWrap = false;
            this.fileList.Location = new System.Drawing.Point(5, 5);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(318, 231);
            this.fileList.TabIndex = 0;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
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
        private ComboBox cboTable;
        private ListView tableList;
        private Label label2;
        private Panel panel1;
        private ListView fileList;
        private Button btnExtract;
        private FolderBrowserDialog folderBrowser;
        private OpenFileDialog openMsiDialog;

        #endregion

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            ToggleSelectAllFiles(true);
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            ToggleSelectAllFiles(false);
        }

        /// <summary>
        /// Selects or unselects all files in the file list.
        /// </summary>
        /// <param name="doSelect">True to select the files, false to unselect them.</param>
        private void ToggleSelectAllFiles(bool doSelect)
        {
            this.fileList.BeginUpdate();
            try
            {
                foreach (ListViewItem item in this.fileList.Items)
                {
                    item.Checked = doSelect;
                }
            }
            finally
            {
                fileList.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the ui with the currently selected msi file.
        /// </summary>
        private void ViewSummary()
        {
            this.propertiesList.Items.Clear();
            try
            {
                using (Database msidb = new Database(GetSelectedMsiFile().FullName, OpenDatabase.ReadOnly))
                {
                    foreach (PropertyInfoListViewItem prop in PropertyInfoListViewItem.GetPropertiesFromDatabase(msidb))
                    {
                        this.propertiesList.Items.Add(prop);
                    }
                }
            }
            catch (Exception eUnexpected)
            {
                Error("Error loading Summary Information", eUnexpected);
            }
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