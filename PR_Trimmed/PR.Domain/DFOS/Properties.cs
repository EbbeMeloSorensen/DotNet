using System.Collections.Generic;

namespace PR.Domain.DFOS
{
    public class Properties
    {
        public string ObjectType { get; set; }
        public Dictionary<string, ObservingFacility> Details { get; set; }
        public List<Identifier> Identifiers { get; set; }
    }
}
