using System;

namespace PR.Domain
{
    public interface IVersionedObject
    {
        Guid ArchiveId { get; set; }
        DateTime Created { get; set; }
        DateTime Superseded { get; set; }
    }
}
