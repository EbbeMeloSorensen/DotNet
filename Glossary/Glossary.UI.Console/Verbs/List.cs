using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("list", HelpText = "List all people.")]
    public sealed class List : RepositoryOperationVerb
    {
    }
}
