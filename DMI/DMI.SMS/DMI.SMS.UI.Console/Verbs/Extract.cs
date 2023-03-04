using CommandLine;

namespace DMI.SMS.UI.Console.Verbs;

[Verb("extract", HelpText = "Extract station list for the Free Data project.")]
public sealed class Extract
{
    [Option('c', "category", Required = true, HelpText = "m (meteorological) or o (oceanographic)")]
    public string Category { get; set; }
}