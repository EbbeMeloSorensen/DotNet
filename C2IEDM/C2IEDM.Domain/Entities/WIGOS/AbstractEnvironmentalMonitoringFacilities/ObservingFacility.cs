namespace C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public class ObservingFacility : AbstractEnvironmentalMonitoringFacility
{
    public string? Name { get; set; }
    public DateTime? DateEstablished { get; set; }
    public DateTime? DateClosed { get; set; }

    public ObservingFacility(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}