namespace C2IEDM.Domain.Entities.ObjectTypes.OrganisationTypes;

public enum CivilianPostTypeCategoryCode
{
    AidAdministrator,
    GovernmentMinister,
    PoliceChief
}

public class CivilianPostType : OrganisationType
{
    public CivilianPostTypeCategoryCode CivilianPostTypeCategoryCode { get; set; }

    public CivilianPostType() : base()
    {
        OrganisationTypeCategory = OrganisationTypeCategory.CivilianPostType;
    }
}