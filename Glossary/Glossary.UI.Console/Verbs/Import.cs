using CommandLine;

namespace Glossary.UI.Console.Verbs;

[Verb("import", HelpText = "Import data file")]
public sealed class Import : RepositoryOperationVerb
{
    [Option('f', "filename", Required = true, HelpText = "File Name")]
    public string FileName { get; set; }
}