using System;

namespace PR.Domain.Entities.PR
{
    public class PersonComment : IVersionedObject
    {
        public Guid ArchiveID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }

        public Guid ID { get; set; }

        public Guid PersonID { get; set; }
        public Guid PersonArchiveID { get; set; }
        public Person Person { get; set; }

        public string Text { get; set; }
    }
}
