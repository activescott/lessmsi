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
        private bool m_SaveBtnUsed;

        private string m_PreviousCheckedLang;
        private string m_CurrentCheckedLang;

        private Dictionary<string, CheckBox> m_CheckBoxDict;
        private Dictionary<string, CultureInfo> m_CultureInfoDict;

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

            var cultureCodes = cultureDirectories
                .Select(Path.GetFileName)
                .Where(dir => !string.IsNullOrEmpty(dir))
                .ToList();

            // Ensure English is included
            if (!cultureCodes.Contains("en"))
            {
                cultureCodes.Add("en");
            }

            var orderedCultures = cultureCodes
                .Distinct()
                .Select(code => {
                    try { return new CultureInfo(code); }
                    catch { return null; } // Prevent crashes caused by invalid folder names
                })
                .Where(ci => ci != null)
                .OrderBy(ci => ci.DisplayName)
                .ToList();

            if (orderedCultures.Any())
            {
                foreach (var cultureInfo in orderedCultures)
                {
                    if (!m_CultureInfoDict.ContainsKey(cultureInfo.Name))
                    {
                        m_CultureInfoDict.Add(cultureInfo.Name, cultureInfo);
                    }
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
                    // Adjust position slightly to avoid being too close to the edge
                    Location = new System.Drawing.Point(20, 10 + (30 * i)), 
                    Checked = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == currentCultureKey
                };

                checkBox.Click += (_, e) => OnCheckboxClick(checkBox, e);

                m_CheckBoxDict.Add(currentCultureKey, checkBox);

                checkBoxesPanel.Controls.Add(checkBox);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            m_SaveBtnUsed = true;
            Close();
        }

        private void OnCheckboxClick(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                m_PreviousCheckedLang = m_CurrentCheckedLang;
                m_CurrentCheckedLang = checkBox.Name;

                if (m_CurrentCheckedLang == m_PreviousCheckedLang)
                {
                    checkBox.Checked = true;
                    return;
                }

                // Uncheck the previous selection
                if (!string.IsNullOrEmpty(m_PreviousCheckedLang) && m_CheckBoxDict.ContainsKey(m_PreviousCheckedLang))
                {
                    m_CheckBoxDict[m_PreviousCheckedLang].Checked = false;
                }
            }
        }

        private void ChangeLanguageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_SaveBtnUsed)
            {
                m_CurrentCheckedLang = string.Empty;
            }
        }
    }
}
