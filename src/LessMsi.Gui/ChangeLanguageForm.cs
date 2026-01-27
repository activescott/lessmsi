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
        private string m_CurrentCheckedLang;

        private Dictionary<string, RadioButton> m_RadioButtonDict;
        private Dictionary<string, CultureInfo> m_CultureInfoDict;

        public ChangeLanguageForm()
        {
            InitializeComponent();

            m_CurrentCheckedLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            setGUIData();

            fillCultureInfoDict();

            generateRadioButtons();
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

        private void generateRadioButtons()
        {
            radioButtonsPanel.Controls.Clear();
            m_RadioButtonDict = new Dictionary<string, RadioButton>();

            string currentTwoLetterLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            for (int i = 0; i < m_CultureInfoDict.Count; i++)
            {
                var currentCultureKey = m_CultureInfoDict.ElementAt(i).Key;
                var currentCultureInfo = m_CultureInfoDict.ElementAt(i).Value;

                var radioButton = new RadioButton
                {
                    Name = currentCultureKey,
                    Text = currentCultureInfo.DisplayName,
                    AutoSize = true,
                    Margin = new Padding(5),
                    Location = new System.Drawing.Point(10, 25 * i),
                    Checked = currentTwoLetterLang == currentCultureKey
                };

                radioButton.CheckedChanged += OnRadioButtonCheckedChanged;

                m_RadioButtonDict.Add(currentCultureKey, radioButton);

                radioButtonsPanel.Controls.Add(radioButton);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            m_SaveBtnUsed = true;
            Close();
        }

        private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                m_CurrentCheckedLang = radioButton.Name;
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
