using System.Collections.Generic;

namespace DMI.ObsDB.Domain.Entities
{
    public class ObservingFacility
    {
        public int Id { get; set; }

        public int StatId { get; set; }

        public virtual ICollection<TimeSeries>? TimeSeries { get; set; }
    }
}
