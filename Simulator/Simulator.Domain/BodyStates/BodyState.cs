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

        // Dette er den "samlede" velocity, der afhænger af, hvilken bodystate, der er tale om.
        // Nogle af operatorer, der regner på staten, har brug for denne, og de skal helst ikke kende til detaljerne i
        // hvordan det regnes ud, så det bør køres polymorfisk
        public abstract Vector2D Velocity { get; }

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
