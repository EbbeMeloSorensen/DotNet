using System;
using Glossary.Domain.Entities;

namespace Glossary.Domain
{
    public static class RecordExtensions
    {
        public static Record Clone(
            this Record record)
        {
            var clone = new Record();
            clone.CopyAttributes(record);
            return clone;
        }

        public static void CopyAttributes(
            this Record record,
            Record other)
        {
            record.Id = other.Id;
            record.Term= other.Term;
            record.Source = other.Source;
            record.Category = other.Category;
            record.Description = other.Description;
            record.Created = other.Created;
        }

        public static Record ConvertFromLegacyPerson(
            this Foreign.Person person)
        {
            var result = new Record
            {
                Id = Guid.NewGuid(),
                Term = person.FirstName,
                Source = string.IsNullOrEmpty(person.Address) ? null : person.Address,
                Category = string.IsNullOrEmpty(person.Category) ? null : person.Category,
                Description = string.IsNullOrEmpty(person.Comments) ? null : person.Comments,
                Created = person.Created.ToUniversalTime()
            };

            return result;
        }
    }
}
