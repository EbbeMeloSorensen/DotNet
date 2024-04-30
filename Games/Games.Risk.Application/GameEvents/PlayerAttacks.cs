namespace Games.Risk.Application.GameEvents
{
    public class PlayerAttacks : GameEvent
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public int DefendingPlayerIndex { get; set; }
        public int CasualtiesAttacker { get; set; }
        public int CasualtiesDefender { get; set; }
        public bool TerritoryConquered { get; set; }
        public bool DefendingPlayerDefeated { get; set; }
        public int DiceRolledByAttacker { get; set; }
        public Card Card { get; set; }

        public PlayerAttacks(
            int playerIndex) : base(
            playerIndex, false)
        {
        }
    }
}