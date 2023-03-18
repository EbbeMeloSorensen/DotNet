using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMI.Domain.FrieData.OGC
{
    public class OceanographicalStation
    {
        public string type { get; set; }
        public string _id { get; set; }
        public double? lon { get; set; }
        public double? lat { get; set; }
        public long? timeValidFrom { get; set; }
        public long? timeValidTo { get; set; }
        public StationLocation geometry { get; set; }
        public OceanographicalStationProperties properties { get; set; }
    }
}
