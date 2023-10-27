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

        // Dette er den "samlede" velocity, der afhænger af, hvilken bodystate, der er tale om.
        // Nogle af operatorer, der regner på staten, har brug for denne, og de skal helst ikke kende til detaljerne i
        // hvordan det regnes ud, så det bør køres polymorfisk
        public abstract Vector2D Velocity { get; }

        protected BodyState(
            Body body)
        {
            Body = body;

            NaturalVelocity = _zeroVector;
        }

        public BodyState(
            Body body,
            Vector2D position)
        {
            Body = body;
            Position = position;

            NaturalVelocity = _zeroVector;
        }

        public abstract BodyState Clone();

        public abstract BodyState Propagate(
            double time,
            Vector2D force);
    }
}
