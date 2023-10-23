using Craft.Math;

namespace Simulator.Domain
{
    public class BodyState
    {
        private static readonly Vector2D _zeroVector = new Vector2D(0, 0);

        public Body Body { get; }
        public Vector2D Position { get; set; }
        public Vector2D NaturalVelocity { get; set; }
        public Vector2D ArtificialVelocity { get; set; }
        public Vector2D CustomForce { get; set; }
        public double Orientation { get; set; }
        public double RotationalSpeed { get; set; }
        public int Life { get; set; }

        public Vector2D EffectiveCustomForce => CustomForce.Rotate(-Orientation);
        public Vector2D EffectiveArtificialVelocity => ArtificialVelocity.Rotate(-Orientation);
        public Vector2D Velocity => NaturalVelocity + EffectiveArtificialVelocity;

        protected BodyState(
            Body body)
        {
            Body = body;
        }

        public BodyState(
            Body body,
            Vector2D position)
        {
            Body = body;
            Position = position;
            
            NaturalVelocity = _zeroVector;
            ArtificialVelocity = _zeroVector;
            CustomForce = _zeroVector;
        }

        public virtual BodyState Clone()
        {
            return new BodyState(Body, Position)
            {
                NaturalVelocity = NaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                CustomForce = CustomForce,
                Orientation = Orientation,
                RotationalSpeed = RotationalSpeed,
                Life = Life
            };
        }

        public virtual BodyState Propagate(
            double time,
            Vector2D force)
        {
            var acceleration = force / Body.Mass;
            var nextNaturalVelocity = NaturalVelocity + time * acceleration;
            var nextPosition = Position + time * Velocity;
            var nextOrientation = Orientation + time * RotationalSpeed;

            return new BodyState(Body)
            {
                Position = nextPosition,
                NaturalVelocity = nextNaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = nextOrientation,
                RotationalSpeed = RotationalSpeed,
                Life = Life,
                CustomForce = CustomForce
            };
        }
    }
}
