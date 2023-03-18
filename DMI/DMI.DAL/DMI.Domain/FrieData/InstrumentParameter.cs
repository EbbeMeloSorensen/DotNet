namespace DMI.Domain.FrieData
{
    public class InstrumentParameter
    {
        public string parameterId { get; set; }
        public double value { get; set; }

        public InstrumentParameter Clone()
        {
            return new InstrumentParameter
            {
                parameterId = parameterId,
                value = value
            };
        }
    }
}
