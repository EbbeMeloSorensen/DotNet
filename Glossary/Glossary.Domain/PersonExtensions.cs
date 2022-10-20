using System;
using Glossary.Domain.Entities;

namespace Glossary.Domain
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
            person.Term= other.Term;
            person.Address = other.Address;
            person.Category = other.Category;
            person.Description = other.Description;
            person.Created = other.Created;
        }

        public static Person ConvertFromLegacyPerson(
            this Foreign.Person person)
        {
            var birthday = person.Birthday.HasValue
                ? new DateTime(
                    person.Birthday.Value.Year,
                    person.Birthday.Value.Month,
                    person.Birthday.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : new DateTime?();

            var result = new Person
            {
                Id = Guid.NewGuid(),
                Term = person.FirstName,
                Address = string.IsNullOrEmpty(person.Address) ? null : person.Address,
                Category = string.IsNullOrEmpty(person.Category) ? null : person.Category,
                Description = string.IsNullOrEmpty(person.Comments) ? null : person.Comments,
                Created = person.Created.ToUniversalTime()
            };

            return result;
        }
    }
}
