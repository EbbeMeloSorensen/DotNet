using System;
using System.Collections.Generic;
using System.Linq;

namespace Craft.Domain
{
    public abstract class BusinessRuleCatalogBase : IBusinessRuleCatalog
    {
        private readonly Dictionary<Type, List<object>> _rules = new Dictionary<Type, List<object>>();

        public void RegisterRule<T>(IBusinessRule<T> rule)
        {
            if (!_rules.ContainsKey(typeof(T)))
            {
                _rules[typeof(T)] = new List<object>();
            }

            _rules[typeof(T)].Add(rule);
        }

        public List<string> Validate<T>(T entity)
        {
            if (!_rules.ContainsKey(typeof(T))) return new List<string>();

            return _rules[typeof(T)]
                .Cast<IBusinessRule<T>>()
                .Where(rule => !rule.Validate(entity))
                .Select(rule => rule.RuleName + ": " + rule.ErrorMessage)
                .ToList();
        }
    }
}