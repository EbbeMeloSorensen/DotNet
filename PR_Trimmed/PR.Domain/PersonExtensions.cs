using System;
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
            person.ArchiveId = other.ArchiveId;
            person.Id = other.Id;
            person.FirstName = other.FirstName;
            person.Surname = other.Surname;
            person.Nickname = other.Nickname;
            person.Address = other.Address;
            person.ZipCode = other.ZipCode;
            person.City = other.City;
            person.Birthday = other.Birthday;
            person.Category = other.Category;
            person.Description = other.Description;
            person.Dead = other.Dead;
            person.Created = other.Created;
            person.Superseded = other.Superseded;
        }
    }
}
