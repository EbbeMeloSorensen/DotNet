namespace Games.Risk.Application.GameEvents
{
    public class PlayerPasses : GameEvent
    {
        public PlayerPasses(
            int playerIndex) : base(
                playerIndex, true)
        {
        }
    }
}
