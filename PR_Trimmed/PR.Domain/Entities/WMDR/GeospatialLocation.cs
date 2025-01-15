using System;

namespace PR.Domain.Entities.WMDR
{
    public abstract class GeospatialLocation : VersionedObject
    {
        public Guid Id { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Guid AbstractEnvironmentalMonitoringFacilityId { get; set; }
        public Guid AbstractEnvironmentalMonitoringFacilityObjectId { get; set; }
        public virtual AbstractEnvironmentalMonitoringFacility AbstractEnvironmentalMonitoringFacility { get; set; }

        public GeospatialLocation(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}