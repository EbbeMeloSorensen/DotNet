namespace WIGOS.Domain.Entities.Geometry.CoordinateSystems
{
    public abstract class CoordinateSystem : VersionedObject
    {
        public Guid Id { get; set; }

        protected CoordinateSystem(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}