using CommandLine;

namespace PR.UI.Console.Verbs
{
    public abstract class RepositoryOperationVerb
    {
        [Option('h', "host", Required = true, HelpText = "Host")]
        public string Host { get; set; }

        [Option('p', "port", Required = false, Default = "5432", HelpText = "Port")]
        public string Port { get; set; }

        [Option('d', "database", Required = false, Default = "People", HelpText = "Database")]
        public string Database { get; set; }

        [Option('s', "schema", Required = false, Default = "public", HelpText = "Schema")]
        public string Schema { get; set; }

        [Option('u', "user", Required = true, HelpText = "User")]
        public string User { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password")]
        public string Password { get; set; }
    }
}
