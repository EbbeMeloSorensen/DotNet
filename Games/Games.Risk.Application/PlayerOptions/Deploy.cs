namespace Games.Risk.Application.PlayerOptions
{
    public class Deploy : IPlayerOption
    {
        public int ActiveTerritoryIndex { get; set; }
        public int Armies { get; set; }
    }
}
