using System;

namespace Glossary.Domain.Entities
{
    public class RecordAssociation
    {
        public Guid Id { get; set; }

        public Guid SubjectRecordId { get; set; }

        public Guid ObjectRecordId { get; set; }

        public Record SubjectRecord { get; set; }

        public Record ObjectRecord { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }
    }
}
