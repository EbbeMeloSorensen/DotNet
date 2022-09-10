using PR.Domain.Entities;

namespace PR.Domain
{
    public static class PersonExtensions
    {
        public static Person Clone(
            this Person person)
        {
            var clone = new Person();
            clone.CopyAttributes(person);
            return clone;
        }

        public static void CopyAttributes(
            this Person person,
            Person other)
        {
            person.Id = other.Id;
            person.FirstName = other.FirstName;
        }
    }
}
