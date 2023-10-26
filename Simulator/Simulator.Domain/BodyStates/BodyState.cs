using Craft.Math;

namespace Simulator.Domain.BodyStates
{
    public abstract class BodyState
    {
        protected static readonly Vector2D _zeroVector = new Vector2D(0, 0);

        public Body Body { get; }
        public Vector2D Position { get; set; }
        
        // Dette er den velocity, man initialiserer en bodystate med, og derudover er det den,
        // der påvirkes af acceleration og dermed af kræfter, der virker på bodyen
        public Vector2D NaturalVelocity { get; set; }

        // Dette er en velocity, som kan sættes UAFHÆNGIGT AF HVILKE KRÆFTER, der virker på en body.
        // Den bruges både for bodies med orientering og til at styre en body med keyboardet 
        // Den bruges bl.a. i følgende scener:
        //   Rotation2: En kugle med orientering, der automatisk bevæger sig rundt i en cirkel
        //   Rotation4: Hvor man STYRER en kugle med orientering
        //   Platformer-spillene: Hvor man styrer en kasse
        //   Shoot'em up-spillene: Hvor man styrer en kugle
        public Vector2D ArtificialVelocity { get; set; }
        
        public double Orientation { get; set; }
        public double RotationalSpeed { get; set; }

        public Vector2D EffectiveArtificialVelocity => ArtificialVelocity.Rotate(-Orientation);

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
