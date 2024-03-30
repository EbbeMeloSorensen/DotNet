namespace Games.Pig.Application.GameEvents
{
    public class PlayerTakesPot : GameEvent
    {
        public int Player { get; set; }
        public int NewScore { get; set; }

        public PlayerTakesPot(
            string description,
            bool turnGoesToNextPlayer) : base(
                description,
                turnGoesToNextPlayer)
        {
        }
    }
}
