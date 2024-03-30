namespace Games.Pig.Application.GameEvents
{
    public abstract class GameEvent : IGameEvent
    {
        public string Description { get; }
        public bool TurnGoesToNextPlayer { get; }

        protected GameEvent(
            string description, 
            bool turnGoesToNextPlayer)
        {
            Description = description;
            TurnGoesToNextPlayer = turnGoesToNextPlayer;
        }
    }
}