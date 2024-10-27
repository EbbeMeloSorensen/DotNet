using System;

namespace PR.Domain.Entities
{
    public class Person : VersionedObject
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string? Surname { get; set; }

        public string? Nickname { get; set; }

        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public string? City { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public bool? Dead { get; set; }
    }
}
