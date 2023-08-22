namespace C2IEDM.Domain.Entities.Context;

public enum OperationalInformationGroupCategoryCode
{
    ComposedPlan,
    CorrelatedEnemyAndUnknown,
    FriendlyAndNeutralOrganisational,
    GloballySignificant,
    FriendlyAndNeutralNonOrganisational,
    UncorrelatedEnemyAndUnknown
}

public class OperationalInformationGroup : Context
{
    public OperationalInformationGroupCategoryCode OperationalInformationGroupCategoryCode { get; set; }

    public OperationalInformationGroup() : base()
    {
        ContextCategoryCode = ContextCategoryCode.OperationalInformationGroup;
    }
}