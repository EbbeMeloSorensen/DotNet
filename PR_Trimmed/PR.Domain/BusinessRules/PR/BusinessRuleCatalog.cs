using Craft.Domain;

namespace PR.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            RegisterRule(new FirstNameIsRequiredRule());
            RegisterRule(new StartIsRequiredRule());
            RegisterRule(new ValidTimeExtremaCannotBeInFutureRule());
            //_businessRuleCatalog.RegisterRule(new NonOverlappingValidTimeIntervalsRule());
        }
    }
}
