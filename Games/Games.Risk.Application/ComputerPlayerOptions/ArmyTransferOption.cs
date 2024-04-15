namespace Games.Risk.Application.ComputerPlayerOptions
{
    public class ArmyTransferOption
    {
        public int InitialTerritoryIndex { get; set; }
        public int DestinationTerritoryIndex { get; set; }
        public double OpportunityRating { get; set; }

        public override string ToString()
        {
            return $"From {InitialTerritoryIndex} to {DestinationTerritoryIndex} (Rating: {OpportunityRating})";
        }
    }
}