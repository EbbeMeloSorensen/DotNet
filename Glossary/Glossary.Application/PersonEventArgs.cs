using System;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class PersonEventArgs : EventArgs
    {
        public readonly Record Record;

        public PersonEventArgs(
            Record record)
        {
            Record = record;
        }
    }
}