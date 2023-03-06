using CommandLine;

namespace DMI.SMS.UI.Console.Verbs;

[Verb("export", HelpText = "Export data.")]
public sealed class Export
{
    [Option('f', "filename", Required = true, HelpText = "Name of file for export")]
    public string FileName { get; set; }
}