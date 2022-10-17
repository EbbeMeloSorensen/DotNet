using CommandLine;

namespace Glossary.UI.Console.Verbs;

[Verb("import", HelpText = "Import data file")]
public sealed class Import : RepositoryOperationVerb
{
    [Option('f', "filename", Required = true, HelpText = "File Name")]
    public string FileName { get; set; }

    [Option('l', "legacy", Required = false, Default = false, HelpText = "First Name")]
    public bool Legacy { get; set; }
}