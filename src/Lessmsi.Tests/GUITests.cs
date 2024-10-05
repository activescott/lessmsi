using Xunit;
using LessMsi.Gui;
using System.Threading;
using System.Globalization;

namespace LessMsi.Tests
{
    public class GUITests : TestBase
    {
        private CultureInfo originalUICulture;
        private CultureInfo originalCulture;

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
            setCustomLocale("it-IT");

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
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;

            var culture = CultureInfo.CreateSpecificCulture(locale);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        private void revertToOriginalLocale()
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}