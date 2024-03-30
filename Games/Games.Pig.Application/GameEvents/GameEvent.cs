namespace Games.Pig.Application.GameEvents
{
    public abstract class GameEvent : IGameEvent
    {
        public int PlayerIndex { get; }

        public string Description { get; }

        public bool TurnGoesToNextPlayer { get; }

        protected GameEvent(
            int playerIndex,
            string description, 
            bool turnGoesToNextPlayer)
        {
            PlayerIndex = playerIndex;
            Description = description;
            TurnGoesToNextPlayer = turnGoesToNextPlayer;
        }
    }
}