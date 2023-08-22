namespace C2IEDM.Domain.Entities.ObjectTypes.OrganisationTypes;

public enum GovernmentOrganisationTypeCategory
{
    NotOtherwiseSpecified,
    InternationalCivil,
    InternationalCivilMilitary,
    MilitaryOrganisationType,
    NationalCivil
}

public enum MainActivity
{
    NotOtherwiseSpecified,
    AgriculturePrograms,
    EducationPrograms,
    FoodPrograms,
    HealthPrograms,
    InfrastructureAndConstructionRepairPrograms,
    SocialPrograms
}

public class GovernmentOrganisationType : OrganisationType
{
    public GovernmentOrganisationTypeCategory GovernmentOrganisationTypeCategory { get; set; }
    public MainActivity? MainActivity { get; set; }

    public GovernmentOrganisationType() : base()
    {
        OrganisationTypeCategory = OrganisationTypeCategory.GovernmentOrganisationType;
    }
}