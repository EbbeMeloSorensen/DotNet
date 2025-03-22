using Craft.Domain;

namespace PR.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            RegisterAtomicRule(new FirstNameIsValidRule());
            RegisterAtomicRule(new SurnameIsValidRule());
            RegisterAtomicRule(new NicknameIsValidRule());
            RegisterAtomicRule(new AddressIsValidRule());
            RegisterAtomicRule(new ZipCodeIsValidRule());
            RegisterAtomicRule(new CityIsValidRule());
            RegisterAtomicRule(new CategoryIsValidRule());
            RegisterAtomicRule(new BirthdayIsValidRule());

            RegisterAtomicRule(new DateRangeIsValidRule());

            //_businessRuleCatalog.RegisterRule(new NonOverlappingValidTimeIntervalsRule());
        }
    }
}
