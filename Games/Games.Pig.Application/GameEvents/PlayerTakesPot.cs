namespace Games.Pig.Application.GameEvents
{
    public class PlayerTakesPot : IGameEvent
    {
        public int Player { get; set; }
    }
}
