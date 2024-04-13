namespace Games.Risk.Application.GameEvents
{
    public abstract class GameEvent : IGameEvent
    {
        public int PlayerIndex { get; }
        public bool TurnGoesToNextPlayer { get; }

        protected GameEvent(
            int playerIndex,
            bool turnGoesToNextPlayer)
        {
            PlayerIndex = playerIndex;
            TurnGoesToNextPlayer = turnGoesToNextPlayer;
        }
    }
}