namespace Games.Risk.Application.PlayerOptions
{
    public class Attack : IPlayerOption
    {
        public int ActiveTerritoryIndex { get; set; }
        public int TargetTerritoryIndex { get; set; }
    }
}