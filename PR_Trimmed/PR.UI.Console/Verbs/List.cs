using CommandLine;

namespace PR.UI.Console.Verbs
{
    [Verb("list", HelpText = "List all people.")]
    public sealed class List
    {
        [Option('d', "databasetime", Required = false, HelpText = "Database Time", Default = "")]
        public string DatabaseTime { get; set; }
    }
}
