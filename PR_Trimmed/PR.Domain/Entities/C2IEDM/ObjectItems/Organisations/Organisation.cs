namespace PR.Domain.Entities.C2IEDM.ObjectItems.Organisations
{
    public class Organisation : ObjectItem
    {
        public string? NickName { get; set; }

        public Organisation()
        {
            NickName = "";
        }
    }
}
