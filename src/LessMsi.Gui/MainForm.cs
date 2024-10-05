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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LessMsi.Gui.Extensions;
using LessMsi.Gui.Model;
using LessMsi.Gui.Properties;
using LessMsi.Gui.Resources.Languages;
using LessMsi.Gui.Windows.Forms;
using LessMsi.Msi;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("LessMsi.Tests")]

namespace LessMsi.Gui
{
    internal class MainForm : Form, IMainFormView
	{
		private readonly MruMenuStripManager _mruManager;
		private TabPage tabStreams;
		private Panel panel4;
		private Label lblStream;
		public ComboBox cboStream;
		private ListBox lstStreamFiles;
		private Panel pnlStreamsBottom;
		private Button btnExtractStreamFiles;
		private ToolStripMenuItem searchFileToolStripMenuItem;
		readonly static string[] AllowedDragDropExtensions = new[] { ".msi", ".msp" };

        public MainForm(string defaultInputFile)
		{
            InitializeComponent();
			msiTableGrid.AutoGenerateColumns = false;
			msiPropertyGrid.AutoGenerateColumns = false;
			Presenter = new MainFormPresenter(this);
			Presenter.Initialize();

			_mruManager = new MruMenuStripManager(mruPlaceHolderToolStripMenuItem);
			_mruManager.MruItemClicked += (mruFilePathName) => Presenter.LoadFile(mruFilePathName);
			if (!string.IsNullOrEmpty(defaultInputFile))
			{
				Presenter.LoadFile(defaultInputFile);
			}

            Icon = Properties.Resources.LessmsiIcon;
        }

		private MainFormPresenter Presenter { get; set; }

		#region IMainFormView Implementation

		public void NotifyNewFileLoaded()
		{
			_mruManager.UsedFile(this.SelectedMsiFileFullName);
		}

		public void AddFileGridColumn(string boundPropertyName, string headerText)
		{
			DataGridViewColumn col = new DataGridViewTextBoxColumn
				                         {
					                         DataPropertyName = boundPropertyName,
					                         HeaderText = headerText,
					                         Name = headerText,
					                         SortMode = DataGridViewColumnSortMode.Automatic
				                         };
			fileGrid.Columns.Add(col);
		}

		public void AutoSizeFileGridColumns()
		{
			foreach (DataGridViewColumn col in this.fileGrid.Columns)
			{
				col.Width = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
			}
		}

		/// <summary>
		/// Sets or returns the fully-qualified name of the selected MSI file in the UI.
		/// No validation or anytihng is done ont his. It just sets/returns what's displayed in the UI.
		/// </summary>
		public string SelectedMsiFileFullName 
		{
			get { return txtMsiFileName.Text; }
			set { txtMsiFileName.Text = value; }
		}

		public string SelectedTableName
		{
			get { return cboTable.Text; }
		}

		public void ChangeUiEnabled(bool doEnable)
		{
			btnExtract.Enabled = doEnable;
			cboTable.Enabled = doEnable;
			cboStream.Enabled = doEnable;
		}

		public MsiPropertyInfo SelectedMsiProperty
		{
			get
			{
				if (msiPropertyGrid.SelectedRows.Count > 0)
					return msiPropertyGrid.SelectedRows[0].DataBoundItem as MsiPropertyInfo;
				else
					return null;
			}
		}

		public string PropertySummaryDescription
		{
			get { return txtSummaryDescription.Text; }
			set { txtSummaryDescription.Text = value; }
		}

		public StreamInfoView SelectedStreamInfo 
		{
			get { return this.cboStream.SelectedValue as StreamInfoView; }
		}

		public void SetStreamSelectorSource(IEnumerable<StreamInfoView> streamNames)
		{
			cboStream.DisplayMember = "Label";
			cboStream.DataSource = streamNames.ToList().AsReadOnly();
		}

		public void SetCabContainedFileListSource(IEnumerable<CabContainedFileView> streamFiles)
		{
			lstStreamFiles.DisplayMember = "Name";
			lstStreamFiles.DataSource = streamFiles;
		}

		public void ShowUserMessageBox(string message)
		{
			MessageBox.Show(this, message, "LessMSI", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void ShowUserError(string formatStr, params object[] args)
		{
			ShowUserMessageBox(string.Format(CultureInfo.CurrentUICulture, formatStr, args));
		}

		#region MSI Table Grid Stuff

		public void AddTableViewGridColumn(string headerText)
		{
			DataGridViewColumn col = new DataGridViewTextBoxColumn
				                         {
											 Name = headerText,
					                         HeaderText = headerText,
					                         Resizable = DataGridViewTriState.True,
					                         SortMode = DataGridViewColumnSortMode.Automatic
				                         };
			msiTableGrid.Columns.Add(col);
		}

		public void ClearTableViewGridColumns()
		{
			msiTableGrid.Columns.Clear();
		}

		public void SetTableViewGridDataSource(IEnumerable<object[]> values)
		{
			msiTableGrid.Rows.Clear();
			foreach (var row in values)
			{
				msiTableGrid.Rows.Add(row);
			}
			msiTableGrid.AutoResizeColumnsSafe();
		}

		public void TableViewSortBy(string columnName, ListSortDirection direction)
		{
			msiTableGrid.Sort(msiTableGrid.Columns[columnName], direction);
			if (msiTableGrid.Rows.Count > 0)
			{
				// Prevent the view from scrolling down
				msiTableGrid.CurrentCell = msiTableGrid.Rows[0].Cells[0];
			}
		}

		#region Property Grid Stuff

		public void SetPropertyGridDataSource(MsiPropertyInfo[] props)
		{
			msiPropertyGrid.DataSource = props;
			msiPropertyGrid.AutoResizeColumnsSafe();
		}

		public void AddPropertyGridColumn(string boundPropertyName, string headerText)
		{
			DataGridViewColumn col = new DataGridViewTextBoxColumn
				                         {DataPropertyName = boundPropertyName, HeaderText = headerText};
			msiPropertyGrid.Columns.Add(col);
		}

		#endregion

		#endregion

		#endregion

		#region Designer Stuff

		// ReSharper disable InconsistentNaming
		private TextBox txtMsiFileName;
		private Label label1;
		private Button btnBrowse;
		private TabControl tabs;
		private TabPage tabExtractFiles;
		private TabPage tabTableView;
		public ComboBox cboTable;
		private Label label2;
		private Panel panel1;
		public Button btnExtract;
		private FolderBrowserDialog folderBrowser;
		private OpenFileDialog openMsiDialog;
		private StatusBar statusBar1;
		internal StatusBarPanel statusPanelDefault;
		private StatusBarPanel statusPanelFileCount;
		public Button btnSelectAll;
        public Button btnUnselectAll;
		private TabPage tabSummary;
		private TextBox txtSummaryDescription;
		private GroupBox grpDescription;
		private Panel panel2;
		private MenuStrip menuStrip1;
		public ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem copyToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		public ToolStripMenuItem preferencesToolStripMenuItem;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		public DataGridView fileGrid;
		private DataGridView msiTableGrid;
		private DataGridView msiPropertyGrid;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mruPlaceHolderToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem exitToolStripMenuItem;
		private Panel panel3;
		private ToolStripMenuItem aboutToolStripMenuItem;
		// ReSharper restore InconsistentNaming
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;
        private SearchPanel searchPanel;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cboTable = new System.Windows.Forms.ComboBox();
            this.msiTableGrid = new System.Windows.Forms.DataGridView();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.msiPropertyGrid = new System.Windows.Forms.DataGridView();
            this.grpDescription = new System.Windows.Forms.GroupBox();
            this.txtSummaryDescription = new System.Windows.Forms.TextBox();
            this.tabStreams = new System.Windows.Forms.TabPage();
            this.lstStreamFiles = new System.Windows.Forms.ListBox();
            this.pnlStreamsBottom = new System.Windows.Forms.Panel();
            this.btnExtractStreamFiles = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblStream = new System.Windows.Forms.Label();
            this.cboStream = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.openMsiDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusPanelDefault = new System.Windows.Forms.StatusBarPanel();
            this.statusPanelFileCount = new System.Windows.Forms.StatusBarPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mruPlaceHolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs.SuspendLayout();
            this.tabExtractFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileGrid)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabTableView.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msiTableGrid)).BeginInit();
            this.tabSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msiPropertyGrid)).BeginInit();
            this.grpDescription.SuspendLayout();
            this.tabStreams.SuspendLayout();
            this.pnlStreamsBottom.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPanelDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPanelFileCount)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMsiFileName
            // 
            this.txtMsiFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMsiFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtMsiFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtMsiFileName.Location = new System.Drawing.Point(55, 5);
            this.txtMsiFileName.Name = "txtMsiFileName";
            this.txtMsiFileName.Size = new System.Drawing.Size(367, 23);
            this.txtMsiFileName.TabIndex = 0;
            this.txtMsiFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReloadCurrentUIOnEnterKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = $"{Strings.File}:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBrowse.Location = new System.Drawing.Point(428, 7);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(24, 19);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabExtractFiles);
            this.tabs.Controls.Add(this.tabTableView);
            this.tabs.Controls.Add(this.tabSummary);
            this.tabs.Controls.Add(this.tabStreams);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 55);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(464, 441);
            this.tabs.TabIndex = 0;
            this.tabs.TabStop = false;
            // 
            // tabExtractFiles
            // 
            this.tabExtractFiles.Controls.Add(this.fileGrid);
            this.tabExtractFiles.Controls.Add(this.panel2);
            this.tabExtractFiles.Location = new System.Drawing.Point(4, 24);
            this.tabExtractFiles.Name = "tabExtractFiles";
            this.tabExtractFiles.Padding = new System.Windows.Forms.Padding(5);
            this.tabExtractFiles.Size = new System.Drawing.Size(456, 413);
            this.tabExtractFiles.TabIndex = 0;
            this.tabExtractFiles.Text = Strings.ExtractFiles;
            // 
            // fileGrid
            // 
            this.fileGrid.AllowUserToAddRows = false;
            this.fileGrid.AllowUserToDeleteRows = false;
            this.fileGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.fileGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.fileGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.fileGrid.Location = new System.Drawing.Point(5, 5);
            this.fileGrid.Name = "fileGrid";
            this.fileGrid.ReadOnly = true;
            this.fileGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fileGrid.Size = new System.Drawing.Size(446, 366);
            this.fileGrid.TabIndex = 5;
            this.fileGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileGrid_KeyDown);
            this.fileGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fileGrid_KeyPress);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSelectAll);
            this.panel2.Controls.Add(this.btnUnselectAll);
            this.panel2.Controls.Add(this.btnExtract);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 371);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(446, 37);
            this.panel2.TabIndex = 4;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectAll.Location = new System.Drawing.Point(0, 9);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(90, 27);
            this.btnSelectAll.TabIndex = 1;
            this.btnSelectAll.Text = Strings.SelectAll;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            this.btnSelectAll.AutoSize = true;
            this.btnSelectAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnSelectAll.AutoEllipsis = false;
            // 
            // btnUnselectAll
            // 
            this.btnUnselectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnUnselectAll.Location = new System.Drawing.Point(106, 9);
            this.btnUnselectAll.Name = "btnUnselectAll";
            this.btnUnselectAll.Size = new System.Drawing.Size(90, 27);
            this.btnUnselectAll.TabIndex = 2;
            this.btnUnselectAll.Text = Strings.UnselectAll;
            this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
            this.btnUnselectAll.AutoSize = true;
            this.btnUnselectAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnUnselectAll.AutoEllipsis = false;
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Enabled = false;
            this.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExtract.Location = new System.Drawing.Point(354, 9);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(90, 27);
            this.btnExtract.TabIndex = 3;
            this.btnExtract.Text = Strings.Extract;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            this.btnExtract.AutoSize = true;
            this.btnExtract.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnExtract.AutoEllipsis = false;
            // 
            // tabTableView
            // 
            this.tabTableView.Controls.Add(this.panel3);
            this.tabTableView.Controls.Add(this.msiTableGrid);
            this.tabTableView.Location = new System.Drawing.Point(4, 22);
            this.tabTableView.Name = "tabTableView";
            this.tabTableView.Size = new System.Drawing.Size(456, 415);
            this.tabTableView.TabIndex = 1;
            this.tabTableView.Text = Strings.TableView;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.cboTable);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(456, 28);
            this.panel3.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = Strings.Table;
            // 
            // cboTable
            // 
            this.cboTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable.Enabled = false;
            this.cboTable.Location = new System.Drawing.Point(50, 5);
            this.cboTable.Name = "cboTable";
            this.cboTable.Size = new System.Drawing.Size(323, 23);
            this.cboTable.TabIndex = 8;
            this.cboTable.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // msiTableGrid
            // 
            this.msiTableGrid.AllowUserToAddRows = false;
            this.msiTableGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.msiTableGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.msiTableGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.msiTableGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.msiTableGrid.Location = new System.Drawing.Point(0, 35);
            this.msiTableGrid.Name = "msiTableGrid";
            this.msiTableGrid.ReadOnly = true;
            this.msiTableGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.msiTableGrid.Size = new System.Drawing.Size(453, 370);
            this.msiTableGrid.TabIndex = 10;
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.msiPropertyGrid);
            this.tabSummary.Controls.Add(this.grpDescription);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(5);
            this.tabSummary.Size = new System.Drawing.Size(456, 415);
            this.tabSummary.TabIndex = 2;
            this.tabSummary.Text = Strings.Summary;
            // 
            // msiPropertyGrid
            // 
            this.msiPropertyGrid.AllowUserToAddRows = false;
            this.msiPropertyGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.msiPropertyGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.msiPropertyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.msiPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msiPropertyGrid.Location = new System.Drawing.Point(5, 5);
            this.msiPropertyGrid.Name = "msiPropertyGrid";
            this.msiPropertyGrid.ReadOnly = true;
            this.msiPropertyGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.msiPropertyGrid.Size = new System.Drawing.Size(446, 299);
            this.msiPropertyGrid.TabIndex = 3;
            this.msiPropertyGrid.SelectionChanged += new System.EventHandler(this.msiPropertyGrid_SelectionChanged);
            // 
            // grpDescription
            // 
            this.grpDescription.Controls.Add(this.txtSummaryDescription);
            this.grpDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpDescription.Location = new System.Drawing.Point(5, 304);
            this.grpDescription.Name = "grpDescription";
            this.grpDescription.Size = new System.Drawing.Size(446, 106);
            this.grpDescription.TabIndex = 2;
            this.grpDescription.TabStop = false;
            this.grpDescription.Text = Strings.Description;
            // 
            // txtSummaryDescription
            // 
            this.txtSummaryDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSummaryDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSummaryDescription.Location = new System.Drawing.Point(3, 19);
            this.txtSummaryDescription.Multiline = true;
            this.txtSummaryDescription.Name = "txtSummaryDescription";
            this.txtSummaryDescription.ReadOnly = true;
            this.txtSummaryDescription.Size = new System.Drawing.Size(440, 84);
            this.txtSummaryDescription.TabIndex = 1;
            // 
            // tabStreams
            // 
            this.tabStreams.Controls.Add(this.lstStreamFiles);
            this.tabStreams.Controls.Add(this.pnlStreamsBottom);
            this.tabStreams.Controls.Add(this.panel4);
            this.tabStreams.Location = new System.Drawing.Point(4, 22);
            this.tabStreams.Name = "tabStreams";
            this.tabStreams.Size = new System.Drawing.Size(456, 415);
            this.tabStreams.TabIndex = 3;
            this.tabStreams.Text = Strings.Streams;
            this.tabStreams.UseVisualStyleBackColor = true;
            // 
            // lstStreamFiles
            // 
            this.lstStreamFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStreamFiles.FormattingEnabled = true;
            this.lstStreamFiles.ItemHeight = 15;
            this.lstStreamFiles.Location = new System.Drawing.Point(0, 28);
            this.lstStreamFiles.Name = "lstStreamFiles";
            this.lstStreamFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstStreamFiles.Size = new System.Drawing.Size(456, 351);
            this.lstStreamFiles.TabIndex = 2;
            // 
            // pnlStreamsBottom
            // 
            this.pnlStreamsBottom.BackColor = System.Drawing.SystemColors.Control;
            this.pnlStreamsBottom.Controls.Add(this.btnExtractStreamFiles);
            this.pnlStreamsBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStreamsBottom.Location = new System.Drawing.Point(0, 379);
            this.pnlStreamsBottom.Name = "pnlStreamsBottom";
            this.pnlStreamsBottom.Size = new System.Drawing.Size(456, 36);
            this.pnlStreamsBottom.TabIndex = 1;
            // 
            // btnExtractStreamFiles
            // 
            this.btnExtractStreamFiles.Enabled = false;
            this.btnExtractStreamFiles.Location = new System.Drawing.Point(319, 6);
            this.btnExtractStreamFiles.Name = "btnExtractStreamFiles";
            this.btnExtractStreamFiles.Size = new System.Drawing.Size(129, 23);
            this.btnExtractStreamFiles.TabIndex = 0;
            this.btnExtractStreamFiles.Text = Strings.ExtractStreamFiles;
            this.btnExtractStreamFiles.UseVisualStyleBackColor = true;
            this.btnExtractStreamFiles.AutoSize = true;
            this.btnExtractStreamFiles.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnExtractStreamFiles.AutoEllipsis = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.lblStream);
            this.panel4.Controls.Add(this.cboStream);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(456, 28);
            this.panel4.TabIndex = 0;
            // 
            // lblStream
            // 
            this.lblStream.AutoSize = true;
            this.lblStream.Location = new System.Drawing.Point(4, 7);
            this.lblStream.Name = "lblStream";
            this.lblStream.Size = new System.Drawing.Size(44, 15);
            this.lblStream.TabIndex = 11;
            this.lblStream.Text = Strings.Stream;
            // 
            // cboStream
            // 
            this.cboStream.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStream.Enabled = false;
            this.cboStream.Location = new System.Drawing.Point(50, 5);
            this.cboStream.Name = "cboStream";
            this.cboStream.Size = new System.Drawing.Size(323, 23);
            this.cboStream.TabIndex = 10;
            this.cboStream.Text = "streamInfo";
            this.cboStream.SelectedValueChanged += new System.EventHandler(this.cboStream_SelectedValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtMsiFileName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 31);
            this.panel1.TabIndex = 0;
            // 
            // openMsiDialog
            // 
            this.openMsiDialog.DefaultExt = "msi";
            this.openMsiDialog.Filter = "msierablefiles|*.msi;*.msp|All Files|*.*";
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 496);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusPanelDefault,
            this.statusPanelFileCount});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(464, 16);
            this.statusBar1.TabIndex = 2;
            // 
            // statusPanelDefault
            // 
            this.statusPanelDefault.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusPanelDefault.Name = "statusPanelDefault";
            this.statusPanelDefault.Width = 337;
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
            this.editToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(464, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator2,
            this.mruPlaceHolderToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = Strings.FileMenuLabel;
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = Strings.Open;
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // mruPlaceHolderToolStripMenuItem
            // 
            this.mruPlaceHolderToolStripMenuItem.Enabled = false;
            this.mruPlaceHolderToolStripMenuItem.Name = "mruPlaceHolderToolStripMenuItem";
            this.mruPlaceHolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mruPlaceHolderToolStripMenuItem.Text = $"<{Strings.RecentFiles}>";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = Strings.Exit;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.toolStripSeparator1,
            this.preferencesToolStripMenuItem,
            this.searchFileToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = Strings.Edit;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.copyToolStripMenuItem.Text = Strings.Copy;
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.preferencesToolStripMenuItem.Text = Strings.PreferencesMenuLabel;
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // searchFileToolStripMenuItem
            // 
            this.searchFileToolStripMenuItem.Name = "searchFileToolStripMenuItem";
            this.searchFileToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
            this.searchFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchFileToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.searchFileToolStripMenuItem.Text = Strings.SearchFile;
            this.searchFileToolStripMenuItem.Click += new System.EventHandler(this.searchFileToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = Strings.About;
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            this.ClientSize = new System.Drawing.Size(464, 512);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(352, 404);
            this.Name = "MainForm";
            this.Text = "Less MSIérables";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.tabs.ResumeLayout(false);
            this.tabExtractFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tabTableView.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msiTableGrid)).EndInit();
            this.tabSummary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.msiPropertyGrid)).EndInit();
            this.grpDescription.ResumeLayout(false);
            this.grpDescription.PerformLayout();
            this.tabStreams.ResumeLayout(false);
            this.pnlStreamsBottom.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPanelDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPanelFileCount)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		#endregion


		private void OpenFileCommand()
		{
			if (DialogResult.OK != openMsiDialog.ShowDialog(this))
			{
				Presenter.Error(string.Format(Strings.FileExistError, openMsiDialog.FileName));
				txtMsiFileName.Text = SelectedMsiFileFullName;
				return;
			}
			LoadFile(openMsiDialog.FileName);
		}

		private void LoadFile(string fileToLoad)
		{
			Presenter.LoadFile(fileToLoad);
			//to make sure shortcut keys for menuitems work properly select a grid:
			if (tabs.SelectedTab == tabExtractFiles)
				fileGrid.Select();
			else if (tabs.SelectedTab == tabTableView)
				msiTableGrid.Select();
			else if (tabs.SelectedTab == tabSummary)
				msiPropertyGrid.Select();
		}

		private void ReloadCurrentUIOnEnterKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 13)
			{
				e.Handled = true;
				var fileString = txtMsiFileName.Text;

				// If directory, open a file open dialog to that directory instead of attempting to open the file --https://github.com/activescott/lessmsi/issues/82
				var p = new LessIO.Path(fileString);
				//TODO: Would be nice to have p.IsDirectory!
				if ((LessIO.FileSystem.GetAttributes(p) & LessIO.FileAttributes.Directory) == LessIO.FileAttributes.Directory)
				{
					if (p.Exists)
					{
						openMsiDialog.InitialDirectory = p.FullPathString;
						OpenFileCommand();
					}
					return;
				}

				Presenter.LoadFile(fileString);
			}
		}

		#region UI Event Handlers

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Presenter.UpdateMSiTableGrid();
		}

		private void btnExtract_Click(object sender, EventArgs e)
		{
			//TODO: Refactor to Presenter
			var selectedFiles = new List<MsiFile>();
			if (fileGrid.SelectedRows.Count == 0)
			{
				ShowUserMessageBox($"{Strings.SelectFilesMessage}");
				return;
			}

            //TODO: Refactor to Presenter
            if (Presenter.SelectedMsiFile == null)
                return;
			LessIO.Path msiFile = new LessIO.Path(Presenter.SelectedMsiFile.FullName);

			if (folderBrowser.SelectedPath == null || folderBrowser.SelectedPath.Length <= 0)
				folderBrowser.SelectedPath = msiFile.Parent.PathString;

			if (DialogResult.OK != folderBrowser.ShowDialog(this))
				return;

			btnExtract.Enabled = false;
			using (var progressDialog = BeginShowingProgressDialog())
			{
				try
				{
					string outputDir = folderBrowser.SelectedPath;
					foreach (DataGridViewRow row in fileGrid.SelectedRows)
					{
						MsiFileItemView fileToExtract = (MsiFileItemView) row.DataBoundItem;
						selectedFiles.Add(fileToExtract.File);
					}


					var filesToExtract = selectedFiles.ToArray();
					Wixtracts.ExtractFiles(msiFile, outputDir, filesToExtract,
					                       new AsyncCallback(progressDialog.UpdateProgress));
				}
				catch (Exception err)
				{
                    // close progress dialog because it can appear on top of the error dialog
                    progressDialog.Close();
                    ErrorDialog.ShowError(this, err.Message, err.StackTrace);
				}
			}
			btnExtract.Enabled = true;
		}

		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			Presenter.ToggleSelectAllFiles(true);
		}

		private void btnUnselectAll_Click(object sender, EventArgs e)
		{
			Presenter.ToggleSelectAllFiles(false);
		}

		private void msiPropertyGrid_SelectionChanged(object sender, EventArgs e)
		{
			Presenter.OnSelectedPropertyChanged();
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var frm = new PreferencesForm();
			frm.ShowDialog(this);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileCommand();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			OpenFileCommand();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// copying the currently selected row in the currently selected ListView here:
			var grid = ActiveControl as DataGridView;
			WinFormsHelper.CopySelectedDataGridRowsToClipboard(grid);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutBox().ShowDialog(this);
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			if (GetDraggedFiles(e).Any())
				e.Effect = DragDropEffects.Copy;
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			var fileName = GetDraggedFiles(e).FirstOrDefault();
			if (fileName != null && File.Exists(fileName))
				LoadFile(fileName);
		}

		private static IEnumerable<string> GetDraggedFiles(DragEventArgs e)
		{
			var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
			var query = from file in files
			            where file != null && AllowedDragDropExtensions.Contains(Path.GetExtension(file).ToLowerInvariant())
			            select file;
			return query;
		}

        /// <summary>
        /// Triggers the file grid search action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
	        if (searchPanel == null)
	        {
				searchPanel = new SearchPanel();
	        }
	        if (IsFileTabSelected)
	        {
		        searchPanel.SearchDataGrid(this.fileGrid,
		                                   (o, args) => Presenter.BeginSearching(args.SearchString),
		                                   (o, args) => { Presenter.BeginSearching(""); }
			        );
	        }
        }

		private void fileGrid_KeyDown(object sender, KeyEventArgs e)
		{
			// If they press escape while navigating the grid and the search panel is open in the search panel, cancel the search:
			if (e.KeyCode == Keys.Escape)
			{
				searchPanel.CancelSearch();
			}
		}

		private void fileGrid_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Char.IsLetterOrDigit(e.KeyChar))
			{
				// looks like a search term while the grid has focus...
				searchFileToolStripMenuItem_Click(this, EventArgs.Empty);
				e.Handled = true;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_mruManager.SavePreferences();
			Properties.Settings.Default.Save();
		}

		private void cboStream_SelectedValueChanged(object sender, EventArgs e)
		{
			this.Presenter.OnSelectedStreamChanged();
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            Size lastRecordedAppSize = Settings.Default.LastRecordedAppSize;
            Point lastRecordedAppLocation = Settings.Default.LastRecordedAppLocation;

            Size = new Size
            (
                Math.Max(MinimumSize.Width, lastRecordedAppSize.Width),
                Math.Max(MinimumSize.Height, lastRecordedAppSize.Height)
            );

            Location = lastRecordedAppLocation;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.LastRecordedAppSize = Size;
            Settings.Default.LastRecordedAppLocation = Location;
            Settings.Default.Save();
        }
        #endregion

        private ExtractionProgressDialog BeginShowingProgressDialog()
		{
			var progressDialog = new ExtractionProgressDialog(this);
			progressDialog.Show();
			progressDialog.Update();
			return progressDialog;
		}

		public IDisposable StartWaitCursor()
		{
			return new DisposableCursor(this);
		}

		public void StatusText(string text, string toolTip)
		{
			statusPanelDefault.Text = text;
			statusPanelDefault.ToolTipText = toolTip;
		}

		protected bool IsFileTabSelected
		{
			get
			{
				return tabs.SelectedTab == tabExtractFiles;
			}
		}
    }
}