namespace C2IEDM.Domain.Entities.Geometry;

public abstract class GeometricVolume : Location
{
    public Guid? LowerVerticalDistanceId{ get; set; }
    public virtual VerticalDistance? LowerVerticalDistance { get; set; }

    public Guid? UpperVerticalDistanceId{ get; set; }
    public virtual VerticalDistance? UpperVerticalDistance { get; set; }
}