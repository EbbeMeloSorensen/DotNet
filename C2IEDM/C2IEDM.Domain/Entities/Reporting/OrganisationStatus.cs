namespace C2IEDM.Domain.Entities.Reporting;

public enum OrganisationStatusOperationalStatusCode
{
    MarginallyOperational,
    NotOperational,
    Operational,
    SubstantiallyOperational,
    TemporarilyNotOperational,
    NotKnown
}

public enum OrganisationStatusOperationalStatusQualifierCode
{
    Destroyed,
    HeavilyDamaged,
    LackingVitalResources,
    LightlyDamaged,
    Lost,
    ModeratelyDamaged,
    NotKnown
}

public class OrganisationStatus : ObjectItemStatus
{
    public OrganisationStatusOperationalStatusCode OrganisationStatusOperationalStatusCode { get; set; }
    public OrganisationStatusOperationalStatusQualifierCode OrganisationStatusOperationalStatusQualifierCode { get; set; }
}