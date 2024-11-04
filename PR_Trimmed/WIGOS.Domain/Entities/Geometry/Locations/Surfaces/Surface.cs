namespace WIGOS.Domain.Entities.Geometry.Locations.Surfaces
{
    public abstract class Surface : Location
    {
        protected Surface(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}