using System;

namespace DMI.ObsDB.Domain.Entities
{
    public class Observation
    {
        public int Id { get; set; }

        public int TimeSeriesId { get; set; }
        public TimeSeries TimeSeries { get; set; }

        public DateTime Time { get; set; }

        public double Value { get; set; }

        public override string ToString()
        {
            return $"{Time} - {Value}";
        }
    }
}
