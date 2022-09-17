using CommandLine;

namespace PR.UI.Console.Verbs
{
    public abstract class RepositoryOperationVerb
    {
        [Option('u', "user", Required = true, HelpText = "Your username")]
        public string User { get; set; }

        [Option('p', "password", Required = true, HelpText = "Your password")]
        public string Password { get; set; }
    }
}
