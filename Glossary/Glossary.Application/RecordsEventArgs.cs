using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class RecordsEventArgs : EventArgs
    {
        public readonly IEnumerable<Record> Records;

        public RecordsEventArgs(
            IEnumerable<Record> records)
        {
            Records = records;
        }
    }
}