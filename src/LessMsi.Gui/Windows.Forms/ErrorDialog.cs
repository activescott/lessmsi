using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms
{
    public partial class ErrorDialog : Form
    {
        public ErrorDialog()
        {
            InitializeComponent();
        }

        public static void ShowError(IWin32Window owner, string message, string technicalDetails)
        {
            var dlg = new ErrorDialog();
            dlg.txtErrorDetail.Text = string.Format("{0}\r\n\r\nTechnical Detail:\r\n\r\n{1}", message, technicalDetails);
            dlg.ShowDialog(owner);
        }

        private void lblPleaseReportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var target = "https://github.com/activescott/lessmsi/issues/";
            System.Diagnostics.Process.Start(target);
        }
    }
}
