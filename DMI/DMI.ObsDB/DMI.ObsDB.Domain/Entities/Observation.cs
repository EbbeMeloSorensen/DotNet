using System;

namespace DMI.ObsDB.Domain.Entities
{
    public class Observation
    {
        public int Id { get; set; }

        public int StatId { get; set; }

        public string ParamId { get; set; }

        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}
