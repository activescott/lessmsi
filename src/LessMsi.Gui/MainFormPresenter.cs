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
using System.Diagnostics;
using System.IO;
using System.Linq;
using LessIO;
using LessMsi.Gui.Model;
using LessMsi.Gui.Resources.Languages;
using LessMsi.Gui.Windows.Forms;
using LessMsi.Msi;
using LessMsi.OleStorage;
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
			View.AddPropertyGridColumn("Name", Strings.Name);
			View.AddPropertyGridColumn("Value", Strings.Value);
			View.AddPropertyGridColumn("ID", Strings.ID);
			View.AddPropertyGridColumn("Type", Strings.Type);
		}

		private void InitializeTableGrid()
		{
			// Anything to do?
		}

		private void InitializeFileGrid()
		{
			View.AddFileGridColumn("Name", Strings.Name);
			View.AddFileGridColumn("Directory", Strings.Directory);
			View.AddFileGridColumn("Component", Strings.Component);
			View.AddFileGridColumn("Size", Strings.Size);
			View.AddFileGridColumn("Version", Strings.Version);
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
			using (var msidb = MsiDatabase.Create(new LessIO.Path(this.SelectedMsiFile.FullName)))
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

			using (View.StartWaitCursor())
			{
				try
				{
					Status();

					ViewLeakedAbstraction.fileGrid.DataSource = null;

					if (msidb.TableExists("File"))
					{
						MsiFile[] dataItems = MsiFile.CreateMsiFilesFromMSI(msidb);
						MsiFileItemView[] viewItems = Array.ConvertAll<MsiFile, MsiFileItemView>(dataItems,
							inItem => new MsiFileItemView(inItem)
							);
						fileDataSource = new SortableBindingList<MsiFileItemView>(viewItems);
						ViewLeakedAbstraction.fileGrid.DataSource = fileDataSource;
						View.AutoSizeFileGridColumns();
						Status(fileDataSource.Count + $" {Strings.FilesFoundStatus}");
					}
					else
					{
						Status(Strings.NoFilesPresentStatus);
					}
				}
				catch (Exception eUnexpected)
				{
					Error(string.Concat(Strings.ViewFilesError, eUnexpected.Message), eUnexpected);
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
				using (var msidb = MsiDatabase.Create(new LessIO.Path(this.SelectedMsiFile.FullName)))
				{
					props = MsiPropertyInfo.GetPropertiesFromDatabase(msidb);
				}
				View.SetPropertyGridDataSource(props);
			}
			catch (Exception eUnexpected)
			{
				Error(Strings.LoadingSummaryInfoError, eUnexpected);
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
			string toolTip = exception != null ? exception.ToString() : "";
			View.StatusText($"{Strings.ERROR}:" + msg, toolTip);
		}

		/// <summary>
		/// Sets the default "idle" status.
		/// </summary>
		public void Status()
		{
			this.Status(string.Empty);
		}

		public void Status(string text)
		{
			View.StatusText(text, string.Empty);
		}

		public void LoadTables()
		{
			var allTableNames = new string[]
            {
                #region Hard Coded Table Names
                //FYI: This list is from https://msdn.microsoft.com/en-us/library/windows/desktop/aa368259(v=vs.85).aspx
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

			using (var msidb = MsiDatabase.Create(new LessIO.Path(this.SelectedMsiFile.FullName)))
			{
				using (View.StartWaitCursor())
				{
					try
					{
						Status($"{Strings.LoadingTablesStatus}");
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

		private void LoadStreams()
		{
			using (var stg = new OleStorageFile(new LessIO.Path(SelectedMsiFile.FullName)))
			{
				var streamViews = stg.GetStreams().Select(s => StreamInfoView.FromStream(s));
				View.SetStreamSelectorSource(streamViews);
			}
		}

		/// <summary>
		/// Called by the view to notify of a change in the selection of the SelectedStreamInfo.
		/// </summary>
		public void OnSelectedStreamChanged()
		{
			if (View.SelectedStreamInfo != null && this.SelectedMsiFile != null)
			{
				// 1: find the right stream containing the cab bits:
				using (var oleFile = new OleStorageFile(new LessIO.Path(this.SelectedMsiFile.FullName)))
				{
					var foundStream = oleFile.GetStreams().FirstOrDefault(s => string.Equals(View.SelectedStreamInfo.Name, s.Name, StringComparison.InvariantCulture));
					if (foundStream == null)
					{
						View.ShowUserError(Strings.FindStreamError, View.SelectedStreamInfo.Name);
						return;
					}
					// if the file is a cab, we'll list the files in it (if it isn't clear the view):
					IEnumerable<CabContainedFileView> streamFiles = new CabContainedFileView[]{};
					if (View.SelectedStreamInfo.IsCabStream)
					{
						var tempFileName = System.IO.Path.GetTempFileName();
						using (var cabBits = foundStream.GetStream(FileMode.Open, FileAccess.Read))
						using (var writer = new BinaryWriter(File.Create(tempFileName)))
						{
							var buffer = new byte[1024*1024];
							int bytesRead;
							do
							{
								bytesRead = cabBits.Read(buffer, 0, buffer.Length);
								writer.Write(buffer, 0, bytesRead);
							} while (bytesRead > 0);
						}
						// 2: enumerate files in the cab and set them to the view's
						
						using (var cab = new LibMSPackN.MSCabinet(tempFileName))
						{
							// ToList to force it to enumerate now.
							streamFiles = cab.GetFiles().Select(f => new CabContainedFileView(f.Filename)).ToList();
						}
						Debug.Assert(streamFiles != null && streamFiles.Any());						
					}
					View.SetCabContainedFileListSource(streamFiles);
				}
			}
		}

		/// <summary>
		/// Shows the table based on the current UI selections in the view (selected MSI and selected table).
		/// </summary>
		public void UpdateMSiTableGrid()
		{
			using (var msidb = MsiDatabase.Create(new LessIO.Path(this.SelectedMsiFile.FullName)))
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

			Status(string.Format(Strings.ProcessingTableStatus, tableName));

			using (View.StartWaitCursor())
			{   // clear the columns no matter what happens (in the event the table doesn't exist we don't want to show anything).
				View.ClearTableViewGridColumns();
				try
				{
					// NOTE: Deliberately not calling msidb.TableExists here as some System tables could not be read due to using it.
					string query = string.Concat("SELECT * FROM `", tableName, "`");

					string sequenceName = string.Empty;
					using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
					{
						foreach (ColumnInfo col in view.Columns)
						{
							string displayName = string.Concat(col.Name, " (", col.TypeID, ")");
							View.AddTableViewGridColumn(displayName);
							if (col.Name == "Sequence")
							{
								sequenceName = displayName;
							}
						}
						View.SetTableViewGridDataSource(view.Records);
					}
					if (!string.IsNullOrEmpty(sequenceName))
					{
						View.TableViewSortBy(sequenceName, ListSortDirection.Ascending);
					}
					Status();
				}
				catch (Exception eUnexpected)
				{
					Error(string.Concat(Strings.ViewTableError, eUnexpected.Message), eUnexpected);
				}
			}
		}

		public void OnSelectedPropertyChanged()
		{
			var selectedProperty = View.SelectedMsiProperty;
			View.PropertySummaryDescription = selectedProperty != null ? selectedProperty.Description : "";
		}

		public FileInfo SelectedMsiFile 
		{
			get { return _selectedMsiFile; }
			set
			{
				_selectedMsiFile = value;

				View.SelectedMsiFileFullName = _selectedMsiFile == null ? "" : _selectedMsiFile.FullName;
			}
		} private FileInfo _selectedMsiFile;

		/// <summary>
		/// Loads the specified file.
		/// </summary>
		/// <param name="filePath"></param>
		public void LoadFile(string filePath)
		{
			var path = System.Text.RegularExpressions.Regex.Replace(filePath.Trim(), "^\"(.+)\"$", "$1");
			FileInfo file = null;
			try
			{
				file = new FileInfo(path);
			}
			catch (ArgumentNullException)
			{
				this.Error(Strings.EmptyFilePathError);
			}
			catch (ArgumentException)
			{
				this.Error(Strings.BadlyFormedFilePathError);
			}
			catch (PathTooLongException)
			{
				this.Error(Strings.TooLongFilePathError);
			}
			catch (NotSupportedException)
			{
				this.Error(Strings.InvalidCharsInFilePathError);
			}
			if (file == null) return;
			if (!file.Exists)
			{
				this.Error(string.Format(Strings.FileExistError, file.FullName));
				if (SelectedMsiFile != null)
					ViewLeakedAbstraction.SelectedMsiFileFullName = SelectedMsiFile.FullName;
				return;
			}
			SelectedMsiFile = file;
			this.LoadCurrentFile();
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
				LoadStreams();
				View.NotifyNewFileLoaded();
			}
			catch (Exception eCatchAll)
			{
				isBadFile = true;
				Error(Strings.OpenFileError, eCatchAll);
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
					Status(string.Format($"{0} {Strings.FilesFoundStatus}", dataSource.Count));
			    }
				ViewLeakedAbstraction.fileGrid.DataSource = dataSource;
		    }

	    }
	}
}
