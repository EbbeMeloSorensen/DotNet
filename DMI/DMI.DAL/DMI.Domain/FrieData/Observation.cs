namespace DMI.Domain.FrieData
{
    public abstract class Observation
    {
        public string _id { get; set; }
        public string parameterId { get; set; }
        public string stationId { get; set; }
        public long timeCreated { get; set; }
        public long timeObserved { get; set; }
    }
}
