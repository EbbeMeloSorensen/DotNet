using System.Collections.Generic;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Line
{
    public class Line: Location
    {
        public virtual ICollection<LinePoint>? LinePoints { get; set; }
    }
}
