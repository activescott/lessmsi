using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;
using NDesk.Options;

namespace LessMsi.Cli
{
	internal class ListTableCommand : LessMsiCommand
	{
		public override void Run(List<string> args)
		{
			/* examples:
			 *	lessmsi l -t Component c:\theinstall.msi
			 *	lessmsi l -t Property c:\theinstall.msi
			*/
			args = args.Skip(1).ToList();
			var tableName = "";
			var options = new OptionSet {
				{ "t=", "Specifies the table to list.", t => tableName = t }
			};
			var extra = options.Parse(args);
			if (extra.Count < 1)
				throw new OptionException("You must specify the msi file to list from.", "l");
			if (string.IsNullOrEmpty(tableName))
				throw new OptionException("You must specify the table name to list.", "t");
			
			var csv = new StringBuilder();
			Debug.Print("Opening msi file '{0}'.", extra[0]);
			using (var msidb = new Database(extra[0], OpenDatabase.ReadOnly))
			{
				Debug.Print("Opening table '{0}'.", tableName);
				var query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM `{0}`", tableName);
				using (var view = new ViewWrapper(msidb.OpenExecuteView(query)))
				{
					for (var index = 0; index < view.Columns.Length; index++)
					{
						var col = view.Columns[index];
						if (index > 0)
							csv.Append(',');
						csv.Append(col.Name);
					}
					csv.AppendLine();
					foreach (var row in view.Records)
					{
						for (var colIndex = 0; colIndex < row.Length; colIndex++)
						{
							if (colIndex > 0)
								csv.Append(',');
							var val = Convert.ToString(row[colIndex], CultureInfo.InvariantCulture);
							var newLine = Environment.NewLine;
							string[] requireEscapeChars = { ",", newLine };
							Array.ForEach(requireEscapeChars, s => {
								if (val.Contains(s))
									val = "\"" + val + "\"";
							});
							csv.Append(val);
						}
						csv.AppendLine();
					}
				}
			}
			Console.Write(csv.ToString());
		}
	}
}