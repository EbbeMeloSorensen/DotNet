using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("delete", HelpText = "Delete an existing record.")]
    public sealed class Delete : RepositoryOperationVerb
    {
        [Option('i', "id", Required = true, HelpText = "Record ID")]
        public string ID { get; set; }
    }
}
