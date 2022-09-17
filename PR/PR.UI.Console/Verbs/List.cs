using CommandLine;

namespace PR.UI.Console.Verbs
{
    [Verb("list", HelpText = "List all people.")]
    public sealed class List : RepositoryOperationVerb
    {
    }
}
