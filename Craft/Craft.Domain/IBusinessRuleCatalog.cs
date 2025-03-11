using System.Collections.Generic;

namespace Craft.Domain
{
    public interface IBusinessRuleCatalog
    {
        void RegisterRule<T>(IBusinessRule<T> rule);

        List<string> Validate<T>(T entity);
    }
}