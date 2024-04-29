namespace Games.Risk.Application.GameEvents
{
    public class PlayerTransfersArmies : GameEvent
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public int ArmiesTransfered { get; set; }

        public PlayerTransfersArmies(
            int playerIndex,
            bool turnGoesToNextPlayer) : base(
                playerIndex, turnGoesToNextPlayer)
        {
        }
    }
}
