namespace DMI.FD.Domain
{
    public class Parameter
    {
        public string _id { get; set; }
        public string description { get; set; }
        public long? number { get; set; }
        public string parameterId { get; set; }
        public long? timeCreated { get; set; }
        public long? timeUpdated { get; set; }
        public long? timeValidFrom { get; set; }
        public long? timeValidTo { get; set; }
        public string unit { get; set; }
    }
}
