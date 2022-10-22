using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("list", HelpText = "List all records.")]
    public sealed class List : RepositoryOperationVerb
    {
    }
}
