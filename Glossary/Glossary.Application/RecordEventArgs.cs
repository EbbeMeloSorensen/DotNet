using System;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class RecordEventArgs : EventArgs
    {
        public readonly Record Record;

        public RecordEventArgs(
            Record record)
        {
            Record = record;
        }
    }
}