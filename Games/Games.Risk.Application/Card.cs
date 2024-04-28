namespace Games.Risk.Application
{
    public enum CardType
    {
        Soldier,
        Horse,
        Cannon,
        Joker
    }

    public class Card
    {
        public int TerritoryIndex { get; }
        public CardType Type { get; }

        public Card(
            int territoryIndex,
            CardType type)
        {
            TerritoryIndex = territoryIndex;
            Type = type;
        }
    }
}
