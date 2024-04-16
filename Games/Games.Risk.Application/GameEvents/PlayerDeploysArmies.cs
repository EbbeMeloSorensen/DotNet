using System.Collections.Generic;

namespace Games.Risk.Application.GameEvents
{
    public class PlayerDeploysArmies : GameEvent
    {
        public List<int> TerritoryIndexes { get; set; }

        public PlayerDeploysArmies(
            int playerIndex,
            bool turnGoesToNextPlayer) : base(
                playerIndex, 
                turnGoesToNextPlayer)
        {
        }
    }
}
