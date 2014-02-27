using System;
using System.Linq;
using System.Windows.Forms;

namespace LessMsi.Gui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm form = new MainForm(args.FirstOrDefault());
            Application.Run(form);
        }
    }
}
