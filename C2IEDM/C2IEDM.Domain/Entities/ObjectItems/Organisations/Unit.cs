namespace C2IEDM.Domain.Entities.ObjectItems.Organisations;

public class Unit : Organisation
{
    public string FormalAbbreviatedName { get; set; }

    public Unit()
    {
        FormalAbbreviatedName = "";
    }
}