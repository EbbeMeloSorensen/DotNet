using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("delete", HelpText = "Delete an existing person.")]
    public sealed class Delete : RepositoryOperationVerb
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }
    }
}
