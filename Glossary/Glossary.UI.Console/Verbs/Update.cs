﻿using CommandLine;

namespace Glossary.UI.Console.Verbs
{
    [Verb("update", HelpText = "Update an existing record.")]
    public sealed class Update : RepositoryOperationVerb
    {
        [Option('i', "id", Required = true, HelpText = "Record ID")]
        public string ID { get; set; }

        [Option('f', "term", Required = false, HelpText = "Term")]
        public string Term { get; set; }
    }
}
