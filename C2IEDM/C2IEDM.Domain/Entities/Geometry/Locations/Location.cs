namespace C2IEDM.Domain.Entities.Geometry.Locations;

public abstract class Location : VersionedObject
{
    public Guid Id { get; set; }

    protected Location(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}