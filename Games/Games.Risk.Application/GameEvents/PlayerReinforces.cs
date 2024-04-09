namespace Games.Risk.Application.GameEvents
{
    public class PlayerReinforces : GameEvent
    {
        public PlayerReinforces(
            int playerIndex, 
            string description, 
            bool turnGoesToNextPlayer) : base(
                playerIndex, 
                description, 
                turnGoesToNextPlayer)
        {
        }
    }
}
