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
using System.Linq;
using LessMsi.Gui.Model;
using LessMsi.Gui.Windows.Forms;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.Gui
{
    /// <summary>
    /// This represents a presenter in the MVP pattern for <see cref="MainForm"/>.
    /// However, this is an old app that didn't use this pattern and will gradually -if ever- completely move to clean MVP.
    /// </summary>
	class MainFormPresenter
	{
		private readonly MainForm _view;
        private SortableBindingList<MsiFileItemView> fileDataSource;

		public MainFormPresenter(MainForm view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			_view = view;
		}

		public void Initialize()
		{
			InitializeFileGrid();
			InitializeTableGrid();
			InitializePropertyGrid();
		}

		private void InitializePropertyGrid()
		{
			View.AddPropertyGridColumn("Name", "Name");
			View.AddPropertyGridColumn("Value", "Value");
			View.AddPropertyGridColumn("ID", "ID");
			View.AddPropertyGridColumn("Type", "Type");
		}

		private void InitializeTableGrid()
		{
			// Anything to do?
		}

		private void InitializeFileGrid()
		{
			View.AddFileGridColumn("Name", "Name");
			View.AddFileGridColumn("Directory", "Directory");
			View.AddFileGridColumn("Component", "Component");
			View.AddFileGridColumn("Size", "Size");
			View.AddFileGridColumn("Version", "Version");
		}

		public IMainFormView View
		{
			get { return _view; }
		}

		[Obsolete("ViewLeakedAbstraction is leaking the raw view implementation. Convert to using View instead.")]
		public MainForm ViewLeakedAbstraction
		{
			get { return _view; }
		}

		/// <summary>
		/// Updates the ui with the currently selected msi file.
		/// </summary>
		public void ViewFiles()
		{
			using (var msidb = new Database(View.SelectedMsiFile.FullName, OpenDatabase.ReadOnly))
			{
				ViewFiles(msidb);
				ToggleSelectAllFiles(true);
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

			using (new DisposableCursor(ViewLeakedAbstraction))
			{
				try
				{
					Status();

					MsiFile[] dataItems = MsiFile.CreateMsiFilesFromMSI(msidb);
					MsiFileItemView[] viewItems = Array.ConvertAll<MsiFile, MsiFileItemView>(dataItems,
						inItem => new MsiFileItemView(inItem)
						);
					fileDataSource = new SortableBindingList<MsiFileItemView>(viewItems);
					ViewLeakedAbstraction.fileGrid.DataSource = fileDataSource;
					View.AutoSizeFileGridColumns();
					Status(fileDataSource.Count + " files found.");
				}
				catch (Exception eUnexpected)
				{
					Error(string.Concat("Cannot view files:", eUnexpected.Message), eUnexpected);
				}
			}
		}

		/// <summary>
		/// Updates the MSI property tab/list
		/// </summary>
		public void UpdatePropertyTabView()
		{
			try
			{
				MsiPropertyInfo[] props;
				using (var msidb = new Database(View.SelectedMsiFile.FullName, OpenDatabase.ReadOnly))
				{
					props = MsiPropertyInfo.GetPropertiesFromDatabase(msidb);
				}
				View.SetPropertyGridDataSource(props);
			}
			catch (Exception eUnexpected)
			{
				Error("Error loading Summary Information", eUnexpected);
			}
		}

		/// <summary>
		/// Selects or unselects all files in the file list.
		/// </summary>
		/// <param name="doSelect">True to select the files, false to unselect them.</param>
		public void ToggleSelectAllFiles(bool doSelect)
		{
			using (WinFormsHelper.BeginUiUpdate(ViewLeakedAbstraction.fileGrid))
			{
				if (doSelect)
					ViewLeakedAbstraction.fileGrid.SelectAll();
				else
				{
					ViewLeakedAbstraction.fileGrid.ClearSelection();
				}
			}
		}

		public void Error(string msg, Exception exception = null)
		{
			Status("ERROR:" + msg);
			ViewLeakedAbstraction.statusPanelDefault.ToolTipText = exception != null ? exception.ToString() : "";
		}

		/// <summary>
		/// Sets the default "idle" status.
		/// </summary>
		public void Status()
		{
			this.Status("");
		}

		public void Status(string text)
		{
			ViewLeakedAbstraction.statusPanelDefault.Text = text;
		}

		public void LoadTables()
		{
			var allTableNames = new string[]
            {
                #region Hard Coded Table Names
                //FYI: This list is from http://msdn.microsoft.com/en-us/library/2k3te2cs%28VS.100%29.aspx
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
                #endregion
            };
			
			var systemTables = new string[]
			{
				"_Validation",
				"_Columns",
				"_Streams",
				"_Storages",
				"_Tables",
				"_TransformView Table"
			};

			IEnumerable<string> msiTableNames = allTableNames;

			using (var msidb = new Database(View.SelectedMsiFile.FullName, OpenDatabase.ReadOnly))
			{
				using (new DisposableCursor(ViewLeakedAbstraction))
				{
					try
					{
						Status("Loading list of tables...");
						var query = "SELECT * FROM `_Tables`";
						using (var msiTable = new ViewWrapper(msidb.OpenExecuteView(query)))
						{
							var tableNames = from record in msiTable.Records
								select record[0] as string;
							//NOTE: system tables are not usually in the _Tables table.
							var tempList = tableNames.ToList();
							tempList.AddRange(systemTables);
							msiTableNames = tempList.ToArray();
						}
						
						Status();
					}
					catch (Exception e)
					{
						Status(e.Message);
					}

					ViewLeakedAbstraction.cboTable.Items.Clear();
					ViewLeakedAbstraction.cboTable.Items.AddRange(msiTableNames.ToArray());
					ViewLeakedAbstraction.cboTable.SelectedIndex = 0;
				}
			}

			
		}

		/// <summary>
		/// Shows the table based on the current UI selections in the view (selected MSI and selected table).
		/// </summary>
		public void UpdateMSiTableGrid()
		{
			using (var msidb = new Database(View.SelectedMsiFile.FullName, OpenDatabase.ReadOnly))
			{
				string tableName = View.SelectedTableName;
				UpdateMSiTableGrid(msidb, tableName);
			}
		}

		/// <summary>
		/// Shows the table in the list on the view table tab.
		/// </summary>
		/// <param name="msidb">The msi database.</param>
		/// <param name="tableName">The name of the table.</param>
		private void UpdateMSiTableGrid(Database msidb, string tableName)
		{
			if (msidb == null || string.IsNullOrEmpty(tableName))
				return;

			Status(string.Concat("Processing Table \'", tableName, "\'."));

			using (new DisposableCursor(ViewLeakedAbstraction))
			{   // clear the columns no matter what happens (in the event the table doesn't exist we don't want to show anything).
				View.ClearTableViewGridColumns();
				try
				{
					// NOTE: Deliberately not calling msidb.TableExists here as some System tables could not be read due to using it.
					string query = string.Concat("SELECT * FROM `", tableName, "`");

					using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
					{
						foreach (ColumnInfo col in view.Columns)
						{
							View.AddTableViewGridColumn(string.Concat(col.Name, " (", col.TypeID, ")"));
						}
						View.SetTableViewGridDataSource(view.Records);
					}
					Status();
				}
				catch (Exception eUnexpected)
				{
					Error(string.Concat("Cannot view table:", eUnexpected.Message), eUnexpected);
				}
			}
		}

		public void OnSelectedPropertyChanged()
		{
			var selectedProperty = View.SelectedMsiProperty;
			View.PropertySummaryDescription = selectedProperty != null ? selectedProperty.Description : "";
		}

		/// <summary>
		/// Loads the file specified in the UI.
		/// </summary>
		public void LoadCurrentFile()
		{
			bool isBadFile = false;
			try
			{
				UpdatePropertyTabView();
				LoadTables();
				ViewFiles();
				UpdateMSiTableGrid();
				View.NotifyNewFileLoaded();
			}
			catch (Exception eCatchAll)
			{
				isBadFile = true;
				Error("Failed to open file.", eCatchAll);
			}
			View.ChangeUiEnabled(!isBadFile);
		}

	    /// <summary>
	    /// Executes searching on gridtable and shows only filtered values
	    /// </summary>
	    /// <param name="searchTerm">Search term or <see cref="String.Empty"/> to cancel the search.</param>
	    internal void BeginSearching(string searchTerm)
	    {
		    if (this.fileDataSource != null)
		    {
			    IList<MsiFileItemView> dataSource;
			    if (string.IsNullOrEmpty(searchTerm))
			    {
				    dataSource = this.fileDataSource;
				    Status();
			    }
			    else
			    {
				    dataSource = this.fileDataSource.Where(x => x.Component.Contains(searchTerm) || x.Directory.Contains(searchTerm) || x.Name.Contains(searchTerm) || x.Version.Contains(searchTerm)).ToList();
					Status(string.Format("{0} files found.", dataSource.Count));
			    }
				ViewLeakedAbstraction.fileGrid.DataSource = dataSource;
		    }

	    }
	}
}
