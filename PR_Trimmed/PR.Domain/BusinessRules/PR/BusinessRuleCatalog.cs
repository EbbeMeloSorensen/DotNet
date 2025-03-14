using Craft.Domain;

namespace PR.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            RegisterRule(new FirstNameIsValidRule());
            RegisterRule(new DateRangeIsValidRule());
            //RegisterRule(new StartIsRequiredRule());
            //RegisterRule(new ValidTimeExtremaCannotBeInTheFutureRule());
            //_businessRuleCatalog.RegisterRule(new NonOverlappingValidTimeIntervalsRule());
        }
    }
}
