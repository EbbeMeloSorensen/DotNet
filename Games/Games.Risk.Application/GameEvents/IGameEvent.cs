namespace Games.Risk.Application.GameEvents
{
    public interface IGameEvent
    {
        int PlayerIndex { get; }
        bool TurnGoesToNextPlayer { get; }
    }
}
