namespace Games.Race.Application.GameEvents
{
    public class PlayerTakesPot : GameEvent
    {
        public int NewScore { get; set; }

        public PlayerTakesPot(
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
