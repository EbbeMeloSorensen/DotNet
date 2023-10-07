using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

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