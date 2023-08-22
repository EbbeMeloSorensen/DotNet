namespace C2IEDM.Domain.Entities.Geometry;

public class Line : Location
{
    public virtual ICollection<LinePoint>? LinePoints { get; set; }
}