namespace C2IEDM.Domain.Entities.Geometry
{
    public class Ellipse : Surface
    {
        public Guid CentrePointId { get; set; }
        public Point CentrePoint { get; set; } = null!;

        public Guid FirstConjugateDiameterPointId { get; set; }
        public Point FirstConjugateDiameterPoint { get; set; } = null!;

        public Guid SecondConjugateDiameterPointId { get; set; }
        public Point SecondConjugateDiameterPoint { get; set; } = null!;
    }
}