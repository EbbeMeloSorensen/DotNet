namespace Games.Risk.Application.GameEvents
{
    public class PlayerTradesInCards : GameEvent
    {
        public int ArmiesReceivedForCards { get; set; }
        public int ArmiesReceivedForControlledTerritories { get; set; }

        public PlayerTradesInCards(
            int playerIndex) : base(
                playerIndex, false)
        {
        }
    }
}
