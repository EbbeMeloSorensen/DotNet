using CommandLine;

namespace DMI.SMS.UI.Console.Verbs;

[Verb("list", HelpText = "List objects")]
public sealed class List
{
    [Option('o', "objects", Required = true, HelpText = "objects, such as stationinformations")]
    public string Objects { get; set; }
}