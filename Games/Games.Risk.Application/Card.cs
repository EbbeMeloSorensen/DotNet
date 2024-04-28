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
        public string Territory { get; }
        public CardType Type { get; }

        public Card(
            string territory,
            CardType type)
        {
            Territory = territory;
            Type = type;
        }
    }
}
