using CommandLine;

namespace DMI.StatDB.UI.Console.Verbs;

[Verb("import", HelpText = "Import data from json to repository")]
public sealed class Import
{
    [Option('f', "filename", Required = true, HelpText = "Name of file for import")]
    public string FileName { get; set; }
}