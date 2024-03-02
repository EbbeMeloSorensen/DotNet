namespace Games.Race.Application.GameEvents
{
    public class PlayerAdvances : IGameEvent
    {
        public int Player { get; set; }
        public int Squares { get; set; }
        public int Total { get; set; }
    }
}