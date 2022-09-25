using CommandLine;

namespace PR.UI.Console.Verbs;

[Verb("export", HelpText = "Export all people.")]
public sealed class Export : RepositoryOperationVerb
{
    [Option('f', "filename", Required = true, HelpText = "File Name")]
    public string FileName { get; set; }
}