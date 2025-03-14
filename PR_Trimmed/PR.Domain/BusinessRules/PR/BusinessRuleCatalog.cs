using Craft.Domain;

namespace PR.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            RegisterRule(new FirstNameIsValidRule());
            //RegisterRule(new StartIsRequiredRule());
            //RegisterRule(new EndMustBeLaterThanStartRule());
            //RegisterRule(new ValidTimeExtremaCannotBeInTheFutureRule());
            //_businessRuleCatalog.RegisterRule(new NonOverlappingValidTimeIntervalsRule());
        }
    }
}
