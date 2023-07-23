using CommandLine;

namespace PR.UI.Console.Verbs
{
    [Verb("create", HelpText = "Create a new Person.")]
    public sealed class Create
    {
        [Option('f', "firstname", Required = true, HelpText = "First Name")]
        public string FirstName { get; set; }
    }
}
