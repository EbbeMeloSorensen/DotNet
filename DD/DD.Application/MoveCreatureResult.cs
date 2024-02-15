using DD.Domain;

namespace DD.Application
{
    public class MoveCreatureResult
    {
        public int? IndexOfDestinationSquare { get; set; }
        public double? WalkingDistanceToDestinationSquare { get; set; }
        public Creature FinalClosestOpponent { get; set; }
        public double FinalDistanceToClosestOpponent { get; set; }
    }
}