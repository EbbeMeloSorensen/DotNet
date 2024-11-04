namespace WIGOS.Domain.Entities.WIGOS
{
    public class Description : VersionedObject
    {
        public Description(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}