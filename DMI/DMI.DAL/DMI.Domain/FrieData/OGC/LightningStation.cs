namespace DMI.Domain.FrieData.OGC
{
    public class LightningStation
    {
        public string type { get; set; }
        public string _id { get; set; }
        public StationLocation geometry { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public LightningStationProperties properties { get; set; }
    }
}
