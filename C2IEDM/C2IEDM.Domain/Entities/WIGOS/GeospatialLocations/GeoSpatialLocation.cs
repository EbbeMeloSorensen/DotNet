namespace C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

public class GeoSpatialLocation : VersionedObject
{
    public GeoSpatialLocation(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}