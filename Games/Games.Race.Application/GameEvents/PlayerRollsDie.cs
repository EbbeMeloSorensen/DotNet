namespace Games.Race.Application.GameEvents
{
    public class PlayerRollsDie : GameEvent
    {
        public int DieRoll { get; set; }

        public PlayerRollsDie(
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
