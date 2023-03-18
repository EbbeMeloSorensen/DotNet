using System.Collections.Generic;

namespace DMI.Domain.FrieData.OGC
{
    public class MeteorologicalStationProperties
    {
        public string country { get; set; }
        //public List<InstrumentParameter> instrumentParameter { get; set; } // NB: Er med i oceanographical
        public string name { get; set; }
        public string owner { get; set; }
        public List<string> parameterId { get; set; }
        public string regionId { get; set; }
        public string stationId { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public double? stationHeight { get; set; } // NB: Er ikke med i oceanographical
        public double? barometerHeight { get; set; } // NB: Er ikke med i oceanographical
        public string wmoCountryCode { get; set; }
        public string wmoStationId { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        public string operationFrom { get; set; }
        public string operationTo { get; set; }
        public string validFrom { get; set; }
        public string validTo { get; set; }
    }
}