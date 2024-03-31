namespace Games.Race.Application.GameEvents
{
    public interface IGameEvent
    {
        int PlayerIndex { get; }
        string Description { get; }
        bool TurnGoesToNextPlayer { get; }
    }
}
