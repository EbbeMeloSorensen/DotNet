using System;
using Glossary.Domain.Entities;

namespace Glossary.Application
{
    public class PersonEventArgs : EventArgs
    {
        public readonly Person Person;

        public PersonEventArgs(
            Person person)
        {
            Person = person;
        }
    }
}