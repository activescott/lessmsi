using Xunit;
using LessMsi.Gui;
using System.Threading;
using System.Globalization;
using Microsoft.Tools.WindowsInstallerXml;

namespace LessMsi.Tests
{
    public class GUITests : TestBase
    {
        private CultureInfo m_OriginalCultureInfo;

        [Fact]
        public void CheckUIStrings()
        {
            checkEnglishUIStrings();
            checkItalianUIStrings();
        }

        private void checkEnglishUIStrings()
        {
            setCustomLocale("en");

            var form = new MainForm(string.Empty);

            // test if form was created successfully
            Assert.NotNull(form);

            // check buttons strings
            Assert.Equal("Select &All", form.btnSelectAll.Text);
            Assert.Equal("&Unselect All", form.btnUnselectAll.Text);
            Assert.Equal("E&xtract", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Edit", form.editToolStripMenuItem.Text);
            Assert.Equal("&Preferences", form.preferencesToolStripMenuItem.Text);

            revertToOriginalLocale();
        }

        private void checkItalianUIStrings()
        {
            setCustomLocale("it");

            var form = new MainForm(string.Empty);

            // test if form was created successfully
            Assert.NotNull(form);

            // check buttons strings
            Assert.Equal("Selziona &Tutto", form.btnSelectAll.Text);
            Assert.Equal("&Deseleziona Tutto", form.btnUnselectAll.Text);
            Assert.Equal("E&strai", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Modifica", form.editToolStripMenuItem.Text);
            Assert.Equal("&Preferenze", form.preferencesToolStripMenuItem.Text);

            revertToOriginalLocale();
        }

        private void setCustomLocale(string locale)
        {
            m_OriginalCultureInfo = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
        }

        private void revertToOriginalLocale()
        {
            Thread.CurrentThread.CurrentCulture = m_OriginalCultureInfo;
            Thread.CurrentThread.CurrentUICulture = m_OriginalCultureInfo;
        }
    }
}