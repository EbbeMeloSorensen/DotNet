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
                FirstName = person.FirstName,
                Surname = string.IsNullOrEmpty(person.Surname) ? null : person.Surname,
                Nickname = string.IsNullOrEmpty(person.Nickname) ? null : person.Nickname,
                Address = string.IsNullOrEmpty(person.Nickname) ? null : person.Nickname,
                ZipCode = string.IsNullOrEmpty(person.Nickname) ? null : person.Nickname,
                City = string.IsNullOrEmpty(person.Nickname) ? null : person.Nickname,
                Birthday = birthday,
                Category = string.IsNullOrEmpty(person.Category) ? null : person.Category,
                Description = string.IsNullOrEmpty(person.Comments) ? null : person.Comments,
                Dead = null,
                Created = person.Created.ToUniversalTime()
            };

            return result;
        }
    }
}
