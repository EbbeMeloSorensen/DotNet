﻿namespace Games.Risk.Application.GameEvents
{
    public class PlayerAttacks : GameEvent
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }

        public PlayerAttacks(
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