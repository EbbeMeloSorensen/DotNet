namespace WIGOS.Domain.Entities.ObjectTypes
{
    public enum ObjectTypeCategoryCode
    {
        NotKnown,
        FacilityType,
        FeatureType,
        MaterialType,
        OrganisationType,
        PersonType
    }

    public class ObjectType
    {
        public Guid Id { get; set; }
        public ObjectTypeCategoryCode ObjectTypeCategoryCode { get; set; }
        public bool DummyIndicatorCode { get; set; }
        public string Name { get; set; }

        public ObjectType()
        {
            Name = "";
        }
    }
}