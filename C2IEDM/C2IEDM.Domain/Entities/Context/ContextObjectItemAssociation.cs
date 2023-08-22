using C2IEDM.Domain.Entities.ObjectItems;

namespace C2IEDM.Domain.Entities.Context;

public enum ContextObjectItemAssociationCategoryCode
{
    Includes,
    IsRelevantTo
}

public class ContextObjectItemAssociation
{
    public Guid ContextId { get; set; }
    public Context Context { get; set; } = null!;

    public Guid ObjectItemId { get; set; }
    public ObjectItem ObjectItem { get; set; } = null!;

    public ContextObjectItemAssociationCategoryCode ContextObjectItemAssociationCategoryCode { get; set; }
}