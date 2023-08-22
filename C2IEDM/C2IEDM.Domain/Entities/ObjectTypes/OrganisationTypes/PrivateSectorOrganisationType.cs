namespace C2IEDM.Domain.Entities.ObjectTypes.OrganisationTypes;

public enum PrivateSectorOrganisationTypeCategoryCode
{
    DefenceIndustry,
    Manufacturing,
    NewsMedia,
    Philantropic
}

public enum PrivateSectorOrganisationTypeMainActivityCode
{
    AgriculturePrograms,
    FoodPrograms,
    SocialPrograms
}

public class PrivateSectorOrganisationType : OrganisationType
{
    public PrivateSectorOrganisationTypeCategoryCode PrivateSectorOrganisationTypeCategoryCode { get; set; }
    public PrivateSectorOrganisationTypeMainActivityCode? PrivateSectorOrganisationTypeMainActivityCode { get; set; }
}