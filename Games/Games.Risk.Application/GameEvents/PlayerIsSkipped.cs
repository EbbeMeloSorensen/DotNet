namespace Games.Risk.Application.GameEvents
{
    public class PlayerIsSkipped : GameEvent
    {
        public PlayerIsSkipped(
            int playerIndex) : base(
            playerIndex, 
            true)
        {
        }
    }
}
