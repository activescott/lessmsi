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
using LessMsi.Msi;
using LessMsi.UI.Model;
using Misc.Windows.Forms;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.UI
{
    /// <summary>
    /// This represents a presenter in the MVP pattern for <see cref="MainForm"/>.
    /// However, this is an old app that didn't use this pattern and will gradually -if ever- completely move to clean MVP.
    /// </summary>
    class MainFormPresenter
    {
        private readonly MainForm _view;

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
            View.AddFileGridColumn("Size", "Size");
            View.AddFileGridColumn("Version", "Version");
        }

        public MainForm View
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

            using (new DisposableCursor(View))
            {
                try
                {
                    Status("");
                    
                    MsiFile[] dataItems = MsiFile.CreateMsiFilesFromMSI(msidb);
                    MsiFileItemView[] viewItems = Array.ConvertAll<MsiFile, MsiFileItemView>(dataItems,
                        inItem => new MsiFileItemView(inItem)
                        );
                    var fileDataSource = new SortableBindingList<MsiFileItemView>(viewItems);
                    View.fileGrid.DataSource = fileDataSource;
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
            using (WinFormsHelper.BeginUiUpdate(View.fileGrid))
            {
                if (doSelect)
                    View.fileGrid.SelectAll();
                else
                {
                    View.fileGrid.ClearSelection();
                }
            }
        }

        public void Error(string msg, Exception exception)
        {
            Status("ERROR:" + msg);
            View.statusPanelDefault.ToolTipText = exception != null ? exception.ToString() : "";
        }

        public void Status(string text)
        {
            View.statusPanelDefault.Text = text;
        }

        public void LoadTables()
        {
            var msiTableNames = new object[]
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

            View.cboTable.Items.Clear();
            View.cboTable.Items.AddRange(msiTableNames);
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

            using (new DisposableCursor(View))
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
                    Status("Idle");
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
            }
            catch (Exception eCatchAll)
            {
                isBadFile = true;
                Error("Failed to open file.", eCatchAll);
            }
            View.ChangeUiEnabled(!isBadFile);
        }
    }
}
