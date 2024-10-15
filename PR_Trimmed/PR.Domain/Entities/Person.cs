using System;

namespace PR.Domain.Entities
{
    public class Person : VersionedObject
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string? Surname { get; set; }
    }
}
