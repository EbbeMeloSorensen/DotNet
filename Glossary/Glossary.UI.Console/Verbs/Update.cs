using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("update", HelpText = "Update an existing person.")]
    public sealed class Update : RepositoryOperationVerb
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('f', "firstname", Required = false, HelpText = "First Name")]
        public string FirstName { get; set; }
    }
}
