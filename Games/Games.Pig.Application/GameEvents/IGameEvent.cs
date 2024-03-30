namespace Games.Pig.Application.GameEvents
{
    public interface IGameEvent
    {
        string Description { get; }
        bool TurnGoesToNextPlayer { get; }
    }
}
