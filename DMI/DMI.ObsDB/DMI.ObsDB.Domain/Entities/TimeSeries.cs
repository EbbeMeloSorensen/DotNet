﻿using System.Collections.Generic;

namespace DMI.ObsDB.Domain.Entities
{
    public class TimeSeries
    {
        public int Id { get; set; }

        public int ObservingFacilityId { get; set; }
        public ObservingFacility ObservingFacility { get; set; }

        public string ParamId { get; set; }

        public virtual ICollection<Observation>? Observations { get; set; }
    }
}
