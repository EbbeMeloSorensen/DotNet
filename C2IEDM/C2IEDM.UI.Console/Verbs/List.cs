using CommandLine;

namespace C2IEDM.UI.Console.Verbs;

[Verb("list", HelpText = "List all people.")]
public sealed class List
{
    [Option('e', "entity", Required = true, HelpText = "Entity")]
    public string Entity { get; set; }

}