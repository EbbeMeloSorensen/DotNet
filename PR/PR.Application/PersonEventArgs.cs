using System;
using PR.Domain.Entities;

namespace PR.Application
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