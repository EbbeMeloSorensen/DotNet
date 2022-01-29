using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DD.IO
{
    // Todo: Måske skal denne deles mellem projekter - den er kopieret fra en af dine andre
    public class MyContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;

        public MyContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
            {
                _ignores[type] = new HashSet<string>();
            }

            foreach (var prop in jsonPropertyNames)
            {
                _ignores[type].Add(prop);

            }
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
            {
                return false;
            }

            return _ignores[type].Contains(jsonPropertyName);
        }
    }
}
