using System;
using System.Collections.Generic;
using System.Text;

namespace DMI.FD.Domain.OGC
{
    public class StationLocation
    {
        public string type { get; set; }
        public List<double?> coordinates { get; set; }
    }
}
