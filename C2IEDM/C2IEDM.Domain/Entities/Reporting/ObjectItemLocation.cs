using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.ObjectItems;

namespace C2IEDM.Domain.Entities.Reporting;

public class ObjectItemLocation
{
    public Guid ObjectItemId { get; set; }
    public ObjectItem ObjectItem { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int ObjectItemLocationIndex { get; set; }

    public Guid ReportingDataId { get; set; }
    public ReportingData ReportingData { get; set; } = null!;

    public double AccuracyQuantity { get; set; }
    public double BearingAngle { get; set; }
    public double SpeedRate { get; set; }
}