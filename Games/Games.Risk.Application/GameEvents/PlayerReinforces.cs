using System.Collections.Generic;

namespace Games.Risk.Application.GameEvents
{
    public class PlayerReinforces : GameEvent
    {
        public List<int> TerritoryIndexes { get; set; }

        public PlayerReinforces(
            int playerIndex, 
            string description, 
            bool turnGoesToNextPlayer) : base(
                playerIndex, 
                description, 
                turnGoesToNextPlayer)
        {
        }
    }
}
