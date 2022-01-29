using Craft.Math;
using Simulator.Domain.Boundaries;

namespace Simulator.Domain
{
    public class BoundaryCollisionReport
    {
        public Body Body { get; }
        public IBoundary Boundary { get; }
        public Vector2D EffectiveSurfaceNormal { get; }

        public BoundaryCollisionReport(
            Body body,
            IBoundary boundary,
            Vector2D effectiveSurfaceNormal)
        {
            Body = body;
            Boundary = boundary;
            EffectiveSurfaceNormal = effectiveSurfaceNormal;
        }
    }
}