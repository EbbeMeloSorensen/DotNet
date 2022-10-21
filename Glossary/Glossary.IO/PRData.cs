using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.IO
{
    public class GlossaryData
    {
        public List<Record> People { get; set; }
        public List<RecordAssociation> RecordAssociations { get; set; }
    }
}
