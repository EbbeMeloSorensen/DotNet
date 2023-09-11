namespace C2IEDM.Domain.Entities.Geometry.Locations.Line;

public class Line : Location
{
    public virtual ICollection<LinePoint>? LinePoints { get; set; }

    public Line(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}