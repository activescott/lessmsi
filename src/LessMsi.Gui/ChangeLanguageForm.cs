using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;

namespace LessMsi.Gui
{
    public partial class ChangeLanguageForm : Form
    {
        private Dictionary<string, CheckBox> m_CheckBoxDict;
        private Dictionary<string, CultureInfo> m_CultureInfoDict;

        private string m_PreviousCheckedLang;
        private string m_CurrentCheckedLang;

        public ChangeLanguageForm()
        {
            InitializeComponent();

            m_PreviousCheckedLang = string.Empty;
            m_CurrentCheckedLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            setGUIData();

            fillCultureInfoDict();

            generateCheckboxes();
        }

        public string NewSelectedLanguage => m_CurrentCheckedLang;

        private void setGUIData()
        {
            Icon = Properties.Resources.LessmsiIcon;
            Text = Resources.Languages.Strings.ChangeLang;
        }

        private void fillCultureInfoDict()
        {
            m_CultureInfoDict = new Dictionary<string, CultureInfo>();

            string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var cultureDirectories = Directory.GetDirectories(executingAssemblyPath);

            var cultures = cultureDirectories
                .Select(Path.GetFileName)
                .Where(dir => !string.IsNullOrEmpty(dir))
                .OrderBy(c => c)
                .ToList();

            cultures.Add("en");

            if (cultures.Any())
            {
                foreach (var culture in cultures)
                {
                    var cultureInfo = new CultureInfo(culture);
                    m_CultureInfoDict.Add(culture, cultureInfo);
                }
            }
        }

        private void generateCheckboxes()
        {
            checkBoxesPanel.Controls.Clear();
            m_CheckBoxDict = new Dictionary<string, CheckBox>();

            for (int i = 0; i < m_CultureInfoDict.Count; i++)
            {
                var currentCultureKey = m_CultureInfoDict.ElementAt(i).Key;
                var currentCultureInfo = m_CultureInfoDict.ElementAt(i).Value;

                var checkBox = new CheckBox
                {
                    Name = currentCultureKey,
                    Text = currentCultureInfo.DisplayName,
                    AutoSize = true,
                    Margin = new Padding(5),
                    Location = new System.Drawing.Point(10, 25 * i),
                    Checked = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == currentCultureKey
                };

                checkBox.Click += (sender, e) => OnCheckboxClick(checkBox, e);

                m_CheckBoxDict.Add(currentCultureKey, checkBox);

                checkBoxesPanel.Controls.Add(checkBox);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnCheckboxClick(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                m_PreviousCheckedLang = m_CurrentCheckedLang;
                m_CurrentCheckedLang = (sender as CheckBox).Name;

                if (m_CurrentCheckedLang == m_PreviousCheckedLang)
                {
                    (sender as CheckBox).Checked = true;
                    return;
                }

                m_CheckBoxDict[m_PreviousCheckedLang].Checked = false;
            }
        }
    }
}