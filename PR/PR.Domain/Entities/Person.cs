using System;
using System.Collections.Generic;

namespace PR.Domain.Entities
{
    public class Person
    {
        // Notice that we're using a guid here rather than an int.
        // Usually I have always used int, but then I came across the Udemy course "Complete guide to
        // building an app with .Net Core and React" by Neil Cummings, where he argues that there is
        // a benefit in using a guid as an id, since you can then create the id at the client side
        // and get on with business rather than having to wait for the server side to generate the id

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

        public DateTime Created { get; set; }

        public virtual ICollection<PersonAssociation>? ObjectPeople { get; set; }
        public virtual ICollection<PersonAssociation>? SubjectPeople { get; set; }

        public Person()
        {
            FirstName = "";
        }
    }
}
