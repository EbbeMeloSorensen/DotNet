using System;

namespace PR.Domain
{
    public abstract class VersionedObject
    {
        public Guid ArchiveId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
    }
}
