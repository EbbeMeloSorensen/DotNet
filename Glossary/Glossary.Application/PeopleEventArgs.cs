using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class PeopleEventArgs : EventArgs
    {
        public readonly IEnumerable<Record> People;

        public PeopleEventArgs(
            IEnumerable<Record> people)
        {
            People = people;
        }
    }
}