namespace C2IEDM.Domain.Entities.Affiliation;

public enum AffiliationReligionCode
{
    Anglican,
    Catholic,
    Hindu,
    Muslim
}

public class AffiliationReligion : Affiliation
{
    public AffiliationReligionCode AffiliationReligionCode { get; set; }
}