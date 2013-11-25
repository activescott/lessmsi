using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi
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
				const string tableName = "Property";
				var query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM `{0}`", tableName);
				using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
				{
				    foreach (var value in from row in view.Records let property = (string)row[view.ColumnIndex("Property")] let value = row[view.ColumnIndex("Value")] where string.Equals("ProductVersion", property, StringComparison.InvariantCultureIgnoreCase) select value)
				    {
				        Console.WriteLine(value);
				        return;
				    }
				    Console.WriteLine("Version not found!");
				}   
			}
		}
	}
}