namespace Games.Risk.Application.GameEvents
{
    public abstract class GameEvent : IGameEvent
    {
        public int PlayerIndex { get; }

        protected GameEvent(
            int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}