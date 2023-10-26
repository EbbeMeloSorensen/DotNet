using System;
using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public class BodyStateExt : BodyState
    {
        public int LifeSpan { get; set; }
        public int CoolDown { get; set; }

        protected BodyStateExt(
            Body body) : base(body)
        {
        }

        public BodyStateExt(
            Body body,
            Vector2D position) : base(body, position)
        {
        }

        public override BodyState Clone()
        {
            return new BodyStateExt(Body, Position)
            {
                NaturalVelocity = NaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = Orientation,
                RotationalSpeed = RotationalSpeed,
                CoolDown = CoolDown,
                LifeSpan = LifeSpan,
            };
        }

        public override BodyState Propagate(
            double time,
            Vector2D force)
        {
            var acceleration = force / Body.Mass;
            var nextNaturalVelocity = NaturalVelocity + time * acceleration;
            var nextPosition = Position + time * NaturalVelocity;
            var nextOrientation = Orientation + time * RotationalSpeed;

            return new BodyStateExt(Body)
            {
                Position = nextPosition,
                NaturalVelocity = nextNaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = nextOrientation,
                RotationalSpeed = RotationalSpeed,
                CoolDown = Math.Max(0, CoolDown - 1),
                LifeSpan = Math.Max(0, LifeSpan - 1),
            };
        }
    }
}
