namespace C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public abstract class AbstractEnvironmentalMonitoringFacility : VersionedObject
{
    public Guid Id { get; set; }

    protected AbstractEnvironmentalMonitoringFacility(
        Guid objectId,
        DateTime created) : base(objectId, created)
    {
    }
}