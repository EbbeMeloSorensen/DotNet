namespace Games.Risk.Application.GameEvents
{
    public class PlayerPasses : GameEvent
    {
        public PlayerPasses(
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
