namespace Games.Pig.Application.GameEvents
{
    public class PlayerRollsDie : GameEvent
    {
        public int Player { get; set; }

        public int DieRoll { get; set; }

        public PlayerRollsDie(
            string description,
            bool turnGoesToNextPlayer) : base(
                description, 
                turnGoesToNextPlayer)
        {
        }
    }
}
