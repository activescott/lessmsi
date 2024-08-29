using Xunit;
using LessMsi.Gui;
using System.Threading;
using System.Globalization;

namespace LessMsi.Tests
{
    public class GUITests : TestBase
    {
        [Fact]
        public void CheckEnglishUIStrings()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            var form = new MainForm(string.Empty);

            // test if form was created successfully
            Assert.NotNull(form);

            Assert.Equal("Select &All", form.btnSelectAll.Text);
            Assert.Equal("&Unselect All", form.btnUnselectAll.Text);
            Assert.Equal("E&xtract", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Edit", form.editToolStripMenuItem.Text);
            Assert.Equal("&Preferences", form.preferencesToolStripMenuItem.Text);

            Thread.CurrentThread.CurrentUICulture = currentCulture;
        }

        [Fact]
        public void CheckItalianUIStrings()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it");

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

            Thread.CurrentThread.CurrentUICulture = currentCulture;
        }
    }
}