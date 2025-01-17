using System;

namespace PR.Domain.Entities.C2IEDM.Geometry
{
    public class LinePoint
    {
        public Guid LineID { get; set; }
        public Line Line { get; set; } = null!;

        public Guid PointId { get; set; }
        public Point Point { get; set; } = null!;

        public int Index { get; set; }
        public int SequenceQuantity { get; set; }
    }
}
