using CommandLine;

namespace PR.UI.Console.Verbs
{
    [Verb("details", HelpText = "Get person details.")]
    public sealed class Details
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('d', "databasetime", Required = false, HelpText = "Database Time", Default = "")]
        public string DatabaseTime { get; set; }
    }
}
