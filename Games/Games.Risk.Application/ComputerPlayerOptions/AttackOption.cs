﻿namespace Games.Risk.Application.ComputerPlayerOptions
{
    public class AttackOption
    {
        public int IndexOfTerritoryWhereAttackOriginates { get; set; }
        public int IndexOfTerritoryUnderAttack { get; set; }
        public int OpportunityRating { get; set; }
    }
}