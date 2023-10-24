using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public class BodyStateClassic : BodyState
    {
        public int Life { get; set; }

        protected BodyStateClassic(
            Body body) : base(body)
        {
        }

        public BodyStateClassic(
            Body body, 
            Vector2D position) : base(body, position)
        {
        }

        public override BodyState Clone()
        {
            return new BodyStateClassic(Body, Position)
            {
                NaturalVelocity = NaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = Orientation,
                RotationalSpeed = RotationalSpeed,
                CustomForce = CustomForce,
                Life = Life
            };
        }

        public override BodyState Propagate(
            double time,
            Vector2D force)
        {
            var acceleration = force / Body.Mass;
            var nextNaturalVelocity = NaturalVelocity + time * acceleration;
            var nextPosition = Position + time * Velocity;
            var nextOrientation = Orientation + time * RotationalSpeed;

            return new BodyStateClassic(Body)
            {
                Position = nextPosition,
                NaturalVelocity = nextNaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = nextOrientation,
                RotationalSpeed = RotationalSpeed,
                CustomForce = CustomForce,
                Life = Life
            };
        }
    }
}
