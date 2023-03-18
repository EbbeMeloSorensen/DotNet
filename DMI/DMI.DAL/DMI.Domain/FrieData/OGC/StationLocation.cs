using System.Collections.Generic;

namespace DMI.Domain.FrieData.OGC
{
    public class StationLocation
    {
        public string type { get; set; }
        public List<double?> coordinates { get; set; }
    }
}
