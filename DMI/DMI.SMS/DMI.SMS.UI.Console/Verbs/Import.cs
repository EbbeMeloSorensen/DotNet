using CommandLine;

namespace DMI.SMS.UI.Console.Verbs
{
    [Verb("import", HelpText = "Import data from json to repository")]
    public class Import
    {
        [Option('f', "filename", Required = true, HelpText = "Name of file for import")]
        public string FileName { get; set; }
    }
}
