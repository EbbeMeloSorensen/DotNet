using System.Collections.Generic;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.IO
{
    public class StatDBData
    {
        public List<Station> Stations { get; set; }
        public List<Position> Positions { get; set; }
    }
}
