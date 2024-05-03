using System.Collections.Generic;

namespace Games.Risk.Application.PlayerOptions
{
    public class TradeInCards : IPlayerOption
    {
        public List<Card> Cards { get; set; }
    }
}
