using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.IO
{
    public class PRData
    {
        public List<Person> People { get; set; }
        public List<PersonAssociation> PersonAssociations { get; set; }
    }
}
