using System;

namespace Glossary.Domain.Entities
{
    public class PersonAssociation
    {
        public Guid Id { get; set; }

        public Guid SubjectPersonId { get; set; }

        public Guid ObjectPersonId { get; set; }

        public Person SubjectPerson { get; set; }

        public Person ObjectPerson { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }
    }
}
