using System.Collections.Generic;

namespace Glossary.Domain.Foreign
{
    public class ContactData
    {
        public List<Person> People { get; set; }
        public List<PersonAssociation> PersonAssociations { get; set; }
    }
}
