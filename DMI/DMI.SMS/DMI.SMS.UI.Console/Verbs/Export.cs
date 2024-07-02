using CommandLine;

namespace DMI.SMS.UI.Console.Verbs;

[Verb("export", HelpText = "Export data from repository to json")]
public sealed class Export
{
    [Option('f', "filename", Required = true, HelpText = "Name of file for export")]
    public string FileName { get; set; }

    [Option('x', "exclude superceded rows", Required = false, Default = true, HelpText = "Exclude superceded rows")]
    public bool ExcludeSupercededRows { get; set; }
}