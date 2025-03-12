using System.Collections.Generic;

namespace Craft.Domain
{
    public interface IBusinessRuleCatalog
    {
        void RegisterRule<T>(IBusinessRule<T> rule);

        Dictionary<string, string> Validate<T>(T entity);
    }
}