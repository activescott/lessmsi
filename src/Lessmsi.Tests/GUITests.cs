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
            checkSerbianCyrillicUIStrings();
            checkSerbianLatinUIStrings();
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
            Assert.Equal("Seleiona &tutto", form.btnSelectAll.Text);
            Assert.Equal("&Deseleziona tutto", form.btnUnselectAll.Text);
            Assert.Equal("E&strai", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Modifica", form.editToolStripMenuItem.Text);
            Assert.Equal("&Preferenze", form.preferencesToolStripMenuItem.Text);

            revertToOriginalLocale();
        }

        private void checkSerbianCyrillicUIStrings()
        {
            setCustomLocale("sr-Cyrl");

            var form = new MainForm(string.Empty);

            // test if form was created successfully
            Assert.NotNull(form);

            // check buttons strings
            Assert.Equal("Изабери &све", form.btnSelectAll.Text);
            Assert.Equal("По&ништи избор свега", form.btnUnselectAll.Text);
            Assert.Equal("И&здвоји", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Уреди", form.editToolStripMenuItem.Text);
            Assert.Equal("&Подешавања", form.preferencesToolStripMenuItem.Text);

            revertToOriginalLocale();
        }

        private void checkSerbianLatinUIStrings()
        {
            setCustomLocale("sr-Latn");

            var form = new MainForm(string.Empty);

            // test if form was created successfully
            Assert.NotNull(form);

            // check buttons strings
            Assert.Equal("Izaberi &sve", form.btnSelectAll.Text);
            Assert.Equal("Po&ništi izbor svega", form.btnUnselectAll.Text);
            Assert.Equal("I&zdvoji", form.btnExtract.Text);

            // check strip menu items strings
            Assert.Equal("&Uredi", form.editToolStripMenuItem.Text);
            Assert.Equal("&Podešavanja", form.preferencesToolStripMenuItem.Text);

            revertToOriginalLocale();
        }

        private void setCustomLocale(string locale)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;

            var culture = new CultureInfo(locale);
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