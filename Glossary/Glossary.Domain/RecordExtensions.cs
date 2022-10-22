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
    }
}
