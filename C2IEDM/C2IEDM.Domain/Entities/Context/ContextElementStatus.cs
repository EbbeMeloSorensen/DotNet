using C2IEDM.Domain.Entities.ObjectItems.Organisations;

namespace C2IEDM.Domain.Entities.Context;

public enum ContextElementStatusCategoryCode
{
    Start,
    End
}

public class ContextElementStatus
{
    public Guid ContextElementId { get; set; }
    public ContextElement ContextElement { get; set; } = null!;
    public int ContextElementIndex  { get; set; }
    public int Index { get; set; }

    public Guid EstablishingOrganisationId { get; set; }
    public Organisation EstablishingOrganisation { get; set; } = null!;

    public ContextElementStatusCategoryCode ContextElementStatusCategoryCode { get; set; }
    public DateTime EffectiveDateTime { get; set; }
}