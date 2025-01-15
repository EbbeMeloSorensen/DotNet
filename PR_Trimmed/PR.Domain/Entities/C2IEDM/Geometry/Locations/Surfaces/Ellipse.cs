using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class Ellipse : Surface
    {
        public Guid CentrePointID { get; set; }
        public Point CentrePoint { get; set; } = null!;

        public Guid FirstConjugateDiameterPointID { get; set; }
        public Point FirstConjugateDiameterPoint { get; set; } = null!;

        public Guid SecondConjugateDiameterPointID { get; set; }
        public Point SecondConjugateDiameterPoint { get; set; } = null!;
    }
}
