﻿using Craft.Math;
using System;

namespace Simulator.Domain.BodyStates
{
    public class BodyStateExt : BodyState
    {
        public int LifeSpan { get; set; }
        public int CoolDown { get; set; }

        protected BodyStateExt(Body body) : base(body)
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
                CustomForce = CustomForce,
                Orientation = Orientation,
                RotationalSpeed = RotationalSpeed,
                Life = Life,
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
            var nextPosition = Position + time * Velocity;
            var nextOrientation = Orientation + time * RotationalSpeed;

            return new BodyStateExt(Body)
            {
                Position = nextPosition,
                NaturalVelocity = nextNaturalVelocity,
                ArtificialVelocity = ArtificialVelocity,
                Orientation = nextOrientation,
                RotationalSpeed = RotationalSpeed,
                Life = Life,
                CoolDown = Math.Max(0, CoolDown - 1),
                CustomForce = CustomForce,
                LifeSpan = Math.Max(0, LifeSpan - 1),
            };
        }
    }
}