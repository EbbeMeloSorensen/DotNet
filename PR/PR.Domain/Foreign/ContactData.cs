using System.Collections.Generic;

namespace PR.Domain.Foreign
{
    public class ContactData
    {
        public List<Person> People { get; set; }
        public List<PersonAssociation> PersonAssociations { get; set; }
    }
}
