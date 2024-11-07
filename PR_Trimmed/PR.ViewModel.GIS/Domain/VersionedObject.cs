using System;

namespace PR.ViewModel.GIS.Domain
{
    public abstract class VersionedObject
    {
        public Guid ObjectId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }

        public VersionedObject(
            Guid objectId,
            DateTime created)
        {
            ObjectId = objectId;
            Created = created;
            Superseded = DateTime.MaxValue;
        }
    }
}
