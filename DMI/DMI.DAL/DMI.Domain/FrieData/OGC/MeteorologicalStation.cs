namespace DMI.Domain.FrieData.OGC
{
    public class MeteorologicalStation
    {
        public string type { get; set; }
        public string _id { get; set; }
        public double? lon { get; set; }
        public double? lat { get; set; }
        public long? timeValidFrom { get; set; }
        public long? timeValidTo { get; set; }
        public StationLocation geometry { get; set; }
        public MeteorologicalStationProperties properties { get; set; }
    }
}
