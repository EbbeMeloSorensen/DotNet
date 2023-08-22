namespace C2IEDM.Domain.Entities.Affiliation;

public enum AffiliationFunctionalGroupCode
{
    Criminal,
    Multinational,
    Terrorist
}

public class AffiliationFunctionalGroup : Affiliation
{
    public AffiliationFunctionalGroupCode AffiliationFunctionalGroupCode { get; set; }
    public string Name { get; set; }

    public AffiliationFunctionalGroup()
    {
        Name = "";
    }    
}