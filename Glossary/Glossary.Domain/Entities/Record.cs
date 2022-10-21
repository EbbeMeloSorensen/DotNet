using System;
using System.Collections.Generic;

namespace Glossary.Domain.Entities
{
    public class Record
    {
        // Notice that we're using a guid here rather than an int.
        // Usually I have always used int, but then I came across the Udemy course "Complete guide to
        // building an app with .Net Core and React" by Neil Cummings, where he argues that there is
        // a benefit in using a guid as an id, since you can then create the id at the client side
        // and get on with business rather than having to wait for the server side to generate the id

        public Guid Id { get; set; }

        public string Term { get; set; }

        public string? Source{ get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public virtual ICollection<RecordAssociation>? ObjectRecords { get; set; }
        public virtual ICollection<RecordAssociation>? SubjectRecords { get; set; }

        public Record()
        {
            Term = "";
        }
    }
}
