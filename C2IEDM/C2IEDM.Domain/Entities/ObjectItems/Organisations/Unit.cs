namespace C2IEDM.Domain.Entities.ObjectItems.Organisations;

public class Unit : Organisation
{
    public string FormalAbbreviatedName { get; set; }

    public Unit(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
        FormalAbbreviatedName = "";
    }
}