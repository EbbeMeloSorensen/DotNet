using System;

namespace Glossary.Domain.Entities
{
    public class RecordAssociation
    {
        public Guid Id { get; set; }

        public Guid SubjectPersonId { get; set; }

        public Guid ObjectPersonId { get; set; }

        public Record SubjectPerson { get; set; }

        public Record ObjectPerson { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }
    }
}
