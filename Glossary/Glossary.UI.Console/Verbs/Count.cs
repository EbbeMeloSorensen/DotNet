using CommandLine;

namespace Glossary.UI.Console.Verbs;

[Verb("count", HelpText = "Count all records.")]
public sealed class Count : RepositoryOperationVerb
{
}