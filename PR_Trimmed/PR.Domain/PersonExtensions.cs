﻿using System;
using PR.Domain.Entities;

namespace PR.Domain
{
    public static class PersonExtensions
    {
        public static Person Clone(
            this Person person)
        {
            var clone = new Person();
            clone.CopyAttributes(person);
            return clone;
        }

        public static void CopyAttributes(
            this Person person,
            Person other)
        {
            person.Id = other.Id;
            person.ObjectId = other.ObjectId;
            person.FirstName = other.FirstName;
            person.Surname = other.Surname;
            person.Created = other.Created;
            person.Superseded = other.Superseded;
        }
    }
}