using CommandLine;

namespace Glossary.UI.Console.Verbs;

[Verb("count", HelpText = "Count all people.")]
public sealed class Count : RepositoryOperationVerb
{
}