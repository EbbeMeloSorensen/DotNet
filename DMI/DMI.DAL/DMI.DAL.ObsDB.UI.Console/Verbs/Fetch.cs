using CommandLine;

namespace DMI.DAL.ObsDB.UI.Console.Verbs;

[Verb("fetch", HelpText = "Fetch data.")]
public sealed class Fetch
{
    [Option('h', "host", Required = true, HelpText = "Host")]
    public string Host { get; set; }

    [Option('d', "database", Required = true, HelpText = "Database")]
    public string Database { get; set; }

    [Option('u', "user", Required = true, HelpText = "User")]
    public string User { get; set; }

    [Option('p', "password", Required = true, HelpText = "Password")]
    public string Password { get; set; }
}