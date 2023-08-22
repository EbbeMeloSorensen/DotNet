namespace C2IEDM.Domain.Entities.ObjectItems;

public class Unit : Organisation
{
    public string FormalAbbreviatedName { get; set; }
    
    public Unit()
    {
        FormalAbbreviatedName = "";
    }
}