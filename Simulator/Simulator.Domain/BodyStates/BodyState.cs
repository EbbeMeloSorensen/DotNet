using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public abstract class BodyState
    {
        protected static readonly Vector2D _zeroVector = new Vector2D(0, 0);

        public Body Body { get; }
        public Vector2D Position { get; set; }
        public Vector2D NaturalVelocity { get; set; }
        public Vector2D ArtificialVelocity { get; set; }
        public double Orientation { get; set; }
        public double RotationalSpeed { get; set; }

        public Vector2D EffectiveArtificialVelocity => ArtificialVelocity.Rotate(-Orientation);
        public Vector2D Velocity => NaturalVelocity + EffectiveArtificialVelocity;

        protected BodyState(
            Body body)
        {
            Body = body;

            NaturalVelocity = _zeroVector;
            ArtificialVelocity = _zeroVector;
        }

        public BodyState(
            Body body,
            Vector2D position)
        {
            Body = body;
            Position = position;

            NaturalVelocity = _zeroVector;
            ArtificialVelocity = _zeroVector;
        }

        public abstract BodyState Clone();

        public abstract BodyState Propagate(
            double time,
            Vector2D force);
    }
}
