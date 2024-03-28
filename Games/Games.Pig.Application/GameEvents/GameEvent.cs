namespace Games.Pig.Application.GameEvents
{
    public abstract class GameEvent : IGameEvent
    {
        public string Description { get; }

        protected GameEvent(
            string description)
        {
            Description = description;
        }
    }
}