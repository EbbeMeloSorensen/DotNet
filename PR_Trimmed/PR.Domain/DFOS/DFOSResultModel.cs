using System.Collections.Generic;

namespace PR.Domain.DFOS
{
    public class DFOSResultModel
    {
        public string Type { get; set; }
        public List<Feature> Features { get; set; }
        public List<Link> Links { get; set; }
    }
}
