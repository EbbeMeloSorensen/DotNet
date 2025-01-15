namespace PR.Domain.Entities.C2IEDM.ObjectItems.Organisations
{
    public class Unit : Organisation
    {
        public string FormalAbbreviatedName { get; set; }

        public Unit()
        {
            FormalAbbreviatedName = "";
        }
    }
}
