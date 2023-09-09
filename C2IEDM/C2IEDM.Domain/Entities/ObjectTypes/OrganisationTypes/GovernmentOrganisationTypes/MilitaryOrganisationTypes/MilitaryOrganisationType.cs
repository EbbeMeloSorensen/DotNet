namespace C2IEDM.Domain.Entities.ObjectTypes.OrganisationTypes.GovernmentOrganisationTypes.MilitaryOrganisationTypes;

public enum MilitaryOrganisationTypeCategory
{
    NotOtherwiseSpecified,
    ExecutiveMilitaryOrganisationType,
    MilitaryPostType,
    TaskFormationType,
    UnitType
}

public enum ServiceCategory
{
    NotOtherwiseSpecified,
    NotKnown,
    AirForce,
    Army,
    BorderGuard,
    CoastGuard,
    Combined,
    Guerrilla,
    Joint,
    LocalDefenceForce,
    LocalMilitia,
    Marines,
    Navy,
    SpecialForce,
    TerritorialForce
}

public class MilitaryOrganisationType : GovernmentOrganisationType
{
    public MilitaryOrganisationTypeCategory MilitaryOrganisationTypeCategory { get; set; }
    public ServiceCategory ServiceCategory { get; set; }

    public MilitaryOrganisationType()
    {
        GovernmentOrganisationTypeCategory = GovernmentOrganisationTypeCategory.MilitaryOrganisationType;
    }
}