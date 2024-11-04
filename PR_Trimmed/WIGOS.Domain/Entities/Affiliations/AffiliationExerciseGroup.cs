namespace WIGOS.Domain.Entities.Affiliations
{
    public class AffiliationExerciseGroup : Affiliation
    {
        public string Name { get; set; }

        public AffiliationExerciseGroup()
        {
            Name = "";
        }
    }
}