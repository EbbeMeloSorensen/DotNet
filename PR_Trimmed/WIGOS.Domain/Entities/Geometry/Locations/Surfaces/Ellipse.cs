using WIGOS.Domain.Entities.Geometry.Locations.Points;

namespace WIGOS.Domain.Entities.Geometry.Locations.Surfaces
{
    public class Ellipse : Surface
    {
        public Guid CentrePointId { get; set; }
        public Point CentrePoint { get; set; } = null!;

        public Guid FirstConjugateDiameterPointId { get; set; }
        public Point FirstConjugateDiameterPoint { get; set; } = null!;

        public Guid SecondConjugateDiameterPointId { get; set; }
        public Point SecondConjugateDiameterPoint { get; set; } = null!;

        public Ellipse(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}