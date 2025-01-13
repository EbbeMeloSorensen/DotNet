using System;

namespace PR.Domain.Wigos
{
    public abstract class AbstractEnvironmentalMonitoringFacility : VersionedObject
    {
        public Guid Id { get; set; }

        protected AbstractEnvironmentalMonitoringFacility(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}