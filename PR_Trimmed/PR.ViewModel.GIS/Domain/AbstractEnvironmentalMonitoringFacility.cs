using System;

namespace PR.ViewModel.GIS.Domain
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
