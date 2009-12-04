using System;
using System.Windows.Forms;
using LessMsi.Msi;
using LessMsi.UI.Model;
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
        }

        private void InitializeTableGrid()
        {
            PropertyInfoListViewItem.InitListViewColumns(View.propertiesList);
        }

        private void InitializeFileGrid()
        {
            //MsiFileListViewItem.InitListViewColumns(fileList);
            DataGridViewColumn col;
            View.fileGrid.Columns.Clear();
            col = new DataGridViewTextBoxColumn {DataPropertyName = "Name", HeaderText = "Name"};
            View.fileGrid.Columns.Add(col);
            col = new DataGridViewTextBoxColumn { DataPropertyName = "Directory", HeaderText = "Directory" };
            View.fileGrid.Columns.Add(col);
            col = new DataGridViewTextBoxColumn { DataPropertyName = "Size", HeaderText = "Size" };
            View.fileGrid.Columns.Add(col);
            col = new DataGridViewTextBoxColumn { DataPropertyName = "Version", HeaderText = "Version" };
            View.fileGrid.Columns.Add(col);
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
            using (Database msidb = new Database(View.GetSelectedMsiFile().FullName, OpenDatabase.ReadOnly))
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
                    this.View.fileGrid.DataSource = fileDataSource;
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
            View.propertiesList.Items.Clear();
            try
            {
                using (Database msidb = new Database(View.GetSelectedMsiFile().FullName, OpenDatabase.ReadOnly))
                {
                    foreach (PropertyInfoListViewItem prop in PropertyInfoListViewItem.GetPropertiesFromDatabase(msidb))
                    {
                        View.propertiesList.Items.Add(prop);
                    }
                }
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
            using (WinFormsHelper.BeginUiUpdate(this.View.fileGrid))
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

        public void ShowUserMessage(string message)
        {
            MessageBox.Show(View, message, "LessMSI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
