namespace Games.Risk.Application.GameEvents
{
    public class PlayerReinforces : GameEvent
    {
        public PlayerReinforces(
            int playerIndex, 
            bool turnGoesToNextPlayer) : base(
                playerIndex,
                turnGoesToNextPlayer)
        {
        }
    }
}
