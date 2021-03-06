using Craft.Math;

namespace Simulator.Domain
{
    public class BodyState
    {
        public Body Body { get; }
        public Vector2D Position { get; set; }
        public Vector2D NaturalVelocity { get; set; }
        public Vector2D ArtificialVelocity { get; set; }
        public Vector2D CustomForce { get; set; }
        public double Orientation { get; set; }
        public double RotationalSpeed { get; set; }
        public double Life { get; set; }

        public Vector2D EffectiveCustomForce => CustomForce.Rotate(-Orientation);
        public Vector2D EffectiveArtificialVelocity => ArtificialVelocity.Rotate(-Orientation);
        public Vector2D Velocity => NaturalVelocity + EffectiveArtificialVelocity;

        public BodyState(
            Body body,
            Vector2D position,
            Vector2D velocity)
        {
            Body = body;
            Position = position;
            NaturalVelocity = velocity;

            ArtificialVelocity = new Vector2D(0, 0);
            CustomForce = new Vector2D(0, 0);
        }

        public BodyState Clone()
        {
            return new BodyState(Body, Position, NaturalVelocity)
            {
                ArtificialVelocity = ArtificialVelocity,
                CustomForce = CustomForce,
                Orientation = Orientation,
                RotationalSpeed = RotationalSpeed,
                Life = Life
            };
        }
    }
}
