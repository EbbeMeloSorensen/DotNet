﻿using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR.AtomicRules
{
    public class ZipCodeIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "ZipCode";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            Person person)
        {
            if (!string.IsNullOrEmpty(person.ZipCode) && person.ZipCode.Length > 20)
            {
                ErrorMessage = "Zip code too long (max 20 characters)";
                return false;
            }

            return true;
        }
    }
}