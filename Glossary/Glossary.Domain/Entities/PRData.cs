using System.Collections.Generic;

namespace Glossary.Domain.Entities
{
    public class PRData
    {
        public List<Record> People { get; set; }
        public List<RecordAssociation> PersonAssociations { get; set; }
    }
}
