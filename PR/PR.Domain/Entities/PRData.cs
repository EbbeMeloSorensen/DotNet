using System.Collections.Generic;

namespace PR.Domain.Entities
{
    public class PRData
    {
        public List<Person> People { get; set; }
        public List<PersonAssociation> PersonAssociations { get; set; }
    }
}
