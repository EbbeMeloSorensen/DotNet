using System.Collections.Generic;

namespace DMI.Domain.FrieData.OGC
{
    public class ObservationResponse
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}
