using C2IEDM.Domain.Entities.ObjectItems;

namespace C2IEDM.Domain.Entities.Context;

public enum OperationalInformationGroupOrganisationAssociationStatusCategoryCode
{
    Start,
    End
}

public class OperationalInformationGroupOrganisationAssociationStatus
{
    public Guid OperationalInformationGroupId { get; set; }
    public Guid OrganisationId { get; set; }
    public int OperationalInformationGroupOrganisationAssociationIndex  { get; set; }
    public OperationalInformationGroupOrganisationAssociation OperationalInformationGroupOrganisationAssociation { get; set; } = null!;
    public int Index { get; set; }

    public Guid EstablishingOrganisationId { get; set; }
    public Organisation EstablishingOrganisation { get; set; } = null!;

    public OperationalInformationGroupOrganisationAssociationStatusCategoryCode OperationalInformationGroupOrganisationAssociationStatusCategoryCode { get; set; }
    public DateTime EffectiveDateTime { get; set; }
}