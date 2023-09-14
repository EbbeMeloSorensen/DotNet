namespace C2IEDM.Domain.Entities.Affiliations;

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