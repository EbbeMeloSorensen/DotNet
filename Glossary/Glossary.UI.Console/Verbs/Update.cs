using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("update", HelpText = "Update an existing person.")]
    public sealed class Update : RepositoryOperationVerb
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('f', "term", Required = false, HelpText = "Term")]
        public string Term { get; set; }
    }
}
