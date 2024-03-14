namespace Games.Pig.Application.GameEvents
{
    public class PlayerRollsDie : IGameEvent
    {
        public int Player { get; set; }

        public int DieRoll { get; set; }
    }
}
