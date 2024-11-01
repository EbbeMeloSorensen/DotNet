namespace WIGOS.Domain.Entities.ObjectItems.Organisations
{
    public class Organisation : ObjectItem
    {
        public string? NickName { get; set; }

        public Organisation(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
            NickName = "";
        }
    }
}