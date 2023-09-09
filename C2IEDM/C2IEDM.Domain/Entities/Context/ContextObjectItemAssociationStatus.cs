using C2IEDM.Domain.Entities.ObjectItems.Organisations;

namespace C2IEDM.Domain.Entities.Context;

public enum ContextObjectItemAssociationStatusCategoryCode
{
    Start,
    End
}


public class ContextObjectItemAssociationStatus
{
    public Guid ContextId { get; set; }
    public Guid ObjectItemId { get; set; }
    public ContextObjectItemAssociation ContextObjectItemAssociation { get; set; } = null!;

    public int Index { get; set; }

    public Guid EstablishingOrganisationId { get; set; }
    public Organisation EstablishingOrganisation { get; set; } = null!;

    public ContextObjectItemAssociationStatusCategoryCode ContextObjectItemAssociationStatusCategoryCode { get; set; }

    public DateTime EffectiveDateTime { get; set; }
}