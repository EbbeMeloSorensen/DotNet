using Craft.Domain;

namespace PR.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            RegisterRule(new FirstNameIsValidRule());
            RegisterRule(new SurnameIsValidRule());
            RegisterRule(new NicknameIsValidRule());
            RegisterRule(new AddressIsValidRule());
            RegisterRule(new ZipCodeIsValidRule());
            RegisterRule(new CityIsValidRule());
            RegisterRule(new CategoryIsValidRule());
            RegisterRule(new BirthdayIsValidRule());
            
            RegisterRule(new DateRangeIsValidRule());

            //_businessRuleCatalog.RegisterRule(new NonOverlappingValidTimeIntervalsRule());
        }
    }
}
