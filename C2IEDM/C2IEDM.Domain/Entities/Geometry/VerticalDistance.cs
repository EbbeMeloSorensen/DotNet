namespace C2IEDM.Domain.Entities.Geometry;

public class VerticalDistance : VersionedObject
{
    public Guid Id { get; set; }
    public double Dimension { get; set; }

    public VerticalDistance(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}