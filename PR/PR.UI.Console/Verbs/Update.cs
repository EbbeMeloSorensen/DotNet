﻿using CommandLine;

namespace PR.UI.Console.Verbs
{
    [Verb("update", HelpText = "Update an existing person.")]
    public sealed class Update
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('f', "firstname", Required = false, HelpText = "First Name")]
        public string FirstName { get; set; }
    }
}
