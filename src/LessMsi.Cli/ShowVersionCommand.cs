using System;
using System.Collections.Generic;
using System.Globalization;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;
using NDesk.Options;

namespace LessMsi.Cli
{
	internal class ShowVersionCommand : LessMsiCommand
	{
		public override void Run(List<string> args)
		{
			// args[0]=v, args[1]=filename.msi
			if (args.Count < 2)
				throw new OptionException("You must specify an msi filename.", "v");
			var msiFileName = args[1];
			using (var msidb = new Database(msiFileName, OpenDatabase.ReadOnly))
			{
				if (TryWriteProductVersion(msidb))
					return;

				// Merge modules (.msm) have no ProductVersion property; their version is in ModuleSignature.
				if (TryWriteModuleVersion(msidb))
					return;

				Console.WriteLine("Version not found!");
			}
		}

		private static bool TryWriteProductVersion(Database msidb)
		{
			try
			{
				const string query = "SELECT `Property`, `Value` FROM `Property`";
				using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
				{
					foreach (var row in view.Records)
					{
						var property = (string)row[view.ColumnIndex("Property")];
						var value = row[view.ColumnIndex("Value")];
						if (string.Equals("ProductVersion", property, StringComparison.InvariantCultureIgnoreCase))
						{
							Console.WriteLine(value);
							return true;
						}
					}
				}
			}
			catch (Exception eNoPropertyTable)
			{
				System.Diagnostics.Trace.WriteLine("Could not read the Property table: " + eNoPropertyTable.Message);
			}
			return false;
		}

		private static bool TryWriteModuleVersion(Database msidb)
		{
			try
			{
				const string query = "SELECT `Version` FROM `ModuleSignature`";
				using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
				{
					foreach (var row in view.Records)
					{
						var value = row[view.ColumnIndex("Version")];
						if (value != null)
						{
							Console.WriteLine(value);
							return true;
						}
					}
				}
			}
			catch (Exception eNoModuleSignatureTable)
			{
				System.Diagnostics.Trace.WriteLine("Could not read the ModuleSignature table: " + eNoModuleSignatureTable.Message);
			}
			return false;
		}
	}
}