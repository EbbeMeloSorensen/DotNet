using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class PeopleEventArgs : EventArgs
    {
        public readonly IEnumerable<Person> People;

        public PeopleEventArgs(
            IEnumerable<Person> people)
        {
            People = people;
        }
    }
}