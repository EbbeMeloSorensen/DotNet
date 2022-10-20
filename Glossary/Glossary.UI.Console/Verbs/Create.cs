using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("create", HelpText = "Create a new Person.")]
    public sealed class Create : RepositoryOperationVerb
    {
        [Option('f', "term", Required = true, HelpText = "Term")]
        public string Term { get; set; }
    }
}
