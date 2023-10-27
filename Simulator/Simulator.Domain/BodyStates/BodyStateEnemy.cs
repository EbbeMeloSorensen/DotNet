using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public class BodyStateEnemy : BodyState
    {
        public double DistanceCovered { get; set; }

        protected BodyStateEnemy(
            Body body) : base(body)
        {
        }

        public BodyStateEnemy(
            Body body, 
            Vector2D position) : base(body, position)
        {
        }

        public override BodyState Clone()
        {
            return new BodyStateEnemy(Body, Position)
            {
                DistanceCovered = DistanceCovered
            };
        }

        public override BodyState Propagate(
            double time,
            Vector2D force)
        {
            var displacement = time * NaturalVelocity;
            var nextPosition = Position + displacement;

            return new BodyStateEnemy(Body)
            {
                Position = nextPosition,
                NaturalVelocity = NaturalVelocity,
                DistanceCovered = DistanceCovered + displacement.Length
            };
        }
    }
}
