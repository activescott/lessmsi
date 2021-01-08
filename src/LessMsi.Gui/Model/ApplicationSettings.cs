using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace LessMsi.Gui.Model
{
	public class ApplicationSettings
	{
		static string applicationSettingsFile;
		static ApplicationSettings applicationSettings;

		public List<string> RecentFiles { get; set; } = new List<string>();


		public void Save()
		{
			try
			{
				using (TextWriter writer = new StreamWriter(ApplicationSettingsFile))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSettings));
					serializer.Serialize(writer, this);
				}
			}
			catch
			{
				// Should we nag the user?
			}
		}

		static string ApplicationSettingsFile
		{
			get
			{
				if (applicationSettingsFile == null)
				{
					string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					applicationSettingsFile = Path.Combine(directory, "lessmsi-gui-settings.xml");
				}
				return applicationSettingsFile;
			}
		}

		public static ApplicationSettings Default
		{
			get
			{
				if (applicationSettings == null)
				{
					try
					{
						using (FileStream fs = new FileStream(ApplicationSettingsFile, FileMode.Open))
						{
							XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSettings));
							applicationSettings = (ApplicationSettings)serializer.Deserialize(fs);
						}
					}
					catch
					{
						applicationSettings = null;
					}
					if (applicationSettings == null)
					{
						applicationSettings = new ApplicationSettings();
					}
				}

				return applicationSettings;
			}
		}
	}
}
