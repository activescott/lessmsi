using LessMsi.Gui.Resources.Languages;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace LessMsi.Gui
{
    partial class AboutBox : Form
	{
		private const string AboutBoxFilePath = "../../aboutbox.rtf";

        public AboutBox()
		{
			InitializeComponent();
			this.Text = $"{Strings.About} {AssemblyTitle}";
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = $"{Strings.Version} {AssemblyVersion}";
			this.labelCopyright.Text = AssemblyCopyright;

            Icon = Properties.Resources.LessmsiIcon;

            createAboutBoxTextWithLocalLanguage();
        }

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion

		private void createAboutBoxTextWithLocalLanguage()
		{
            // Create a new RichTextBox control
            using (RichTextBox richTextBox = new RichTextBox())
            {
                // Load the RTF template into the RichTextBox
                string rtfTemplate = File.ReadAllText("../../aboutbox_template.rtf");
                richTextBox.Rtf = rtfTemplate;

                // Replace placeholders with actual values
                replacePlaceholder(richTextBox, "#CreatedBy#", Strings.CreatedBy);
                replacePlaceholder(richTextBox, "#WithContributionsFrom#", Strings.WithContributionsFrom);
                replacePlaceholder(richTextBox, "#AndOthers#", Strings.AndOthers);
                replacePlaceholder(richTextBox, "#ModifiedVersionText#", Strings.ModifiedVersionText);
                replacePlaceholder(richTextBox, "#Library#", Strings.Library);
                replacePlaceholder(richTextBox, "#ModifiedLibmspackText#", Strings.ModifiedLibmspackText);

                // Delete any old AboutBox.rtf file
                if (File.Exists(AboutBoxFilePath))
				{
					File.Delete(AboutBoxFilePath);
                }

                // Save the modified RTF content to a new file
                File.WriteAllText(AboutBoxFilePath, richTextBox.Rtf);
            }
        }

        static void replacePlaceholder(RichTextBox rtb, string placeholder, string value)
        {
            rtb.Rtf  = rtb.Rtf.Replace(placeholder, value);
        }

        private void AboutBox_Load(object sender, EventArgs e)
		{
			this.richTextBox.LoadFile("../../aboutbox.rtf", RichTextBoxStreamType.RichText);
        }

		private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Process.Start(e.LinkText);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var text = ((LinkLabel) sender).Text;
			var link = text.Substring(e.Link.Start, e.Link.Length);
			Process.Start(link);
		}
	}
}