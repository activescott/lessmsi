using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace LessMsi.Cli
{
    internal class ExtractCommand : LessMsiCommand
    {
        public override void Run(List<string> allArgs)
        {
            var args = allArgs.Skip(1).ToList();
            // "x msi_name [path_to_extract\] [file_names]+
            if (args.Count < 1)
                throw new OptionException("Invalid argument. Extract command must at least specify the name of an msi file.", "x");

            var i = 0;
            var msiFile = args[i++];
            if (!File.Exists(msiFile))
                throw new OptionException("Invalid argument. Specified msi file does not exist.", "x");
            var filesToExtract = new List<string>();
            var extractDir = "";
            if (i < args.Count)
            {
                if (extractDir == "" && (args[i].EndsWith("\\") || args[i].EndsWith("\"")))
                    extractDir = args[i];
                else
                    filesToExtract.Add(args[i]);
            }
            while (++i < args.Count)
                filesToExtract.Add(args[i]);

            Program.DoExtraction(msiFile, extractDir.TrimEnd('\"'), filesToExtract, getExtractionMode(allArgs[0]));
        }

        private ExtractionMode getExtractionMode(string commandArgument)
        {
            commandArgument = commandArgument.ToLowerInvariant();
            ExtractionMode extractionMode = ExtractionMode.PreserveDirectoriesExtraction;

            if (commandArgument[commandArgument.Length - 1] == 'o')
            {
                extractionMode = ExtractionMode.OverwriteFlatExtraction;
            }
            else if (commandArgument[commandArgument.Length - 1] == 'r')
            {
                extractionMode = ExtractionMode.RenameFlatExtraction;
            }

            return extractionMode;
        }
    }
}