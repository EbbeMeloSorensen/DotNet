using System;

namespace PR.Domain.Entities
{
    public class PersonAssociation : VersionedObject
    {
        public Guid Id { get; set; }

        public Guid SubjectPersonId { get; set; }

        public Guid ObjectPersonId { get; set; }

        public Person SubjectPerson { get; set; }

        public Person ObjectPerson { get; set; }

        public string? Description { get; set; }
    }
}
