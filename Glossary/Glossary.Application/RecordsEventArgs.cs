using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class RecordsEventArgs : EventArgs
    {
        public readonly IEnumerable<Record> People;

        public RecordsEventArgs(
            IEnumerable<Record> people)
        {
            People = people;
        }
    }
}