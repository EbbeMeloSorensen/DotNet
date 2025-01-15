using System;

namespace PR.Domain.Entities.WMDR
{
    public class ObservingFacility : AbstractEnvironmentalMonitoringFacility
    {
        public string? Name { get; set; }
        public DateTime DateEstablished { get; set; }
        public DateTime DateClosed { get; set; }

        public ObservingFacility(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }

        public override string ToString()
        {
            return $"Observing Facility: {Name}";
        }
    }
}
