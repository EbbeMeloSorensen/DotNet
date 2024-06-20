using System;

namespace DMI.Data.Studio.Application
{
    public class Chunk
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ObservationCount { get; set; }

        public override string ToString()
        {
            return $"{ObservationCount}";
        }
    }
}
