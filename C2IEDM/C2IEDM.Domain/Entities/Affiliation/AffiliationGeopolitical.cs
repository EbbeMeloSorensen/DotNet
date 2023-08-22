namespace C2IEDM.Domain.Entities.Affiliation;

public enum AffiliationGeopoliticalCode
{
    Aruba,
    Denmark,
    France,
    Germany,
    India,
    Pakistan,
    Russia,
    Ukraine,
    UnitedKingdom,
    UnitedStates
}

public class AffiliationGeopolitical : Affiliation
{
    public AffiliationGeopoliticalCode AffiliationGeopoliticalCode { get; set; }
}