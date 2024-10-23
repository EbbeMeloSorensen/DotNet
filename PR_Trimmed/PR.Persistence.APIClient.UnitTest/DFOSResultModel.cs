namespace PR.Persistence.APIClient.UnitTest
{
    public class DFOSResultModel
    {
        public string Type { get; set; }
        public List<Feature> Features { get; set; }
    }

   public class Feature
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
     }

    public class Properties
    {
        public string ObjectType { get; set; }
        public List<Identifier> Identifiers { get; set; }
    }

    public class Identifier
    {
        public IdentifierType IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string ValidPeriod { get; set; }
        public bool PrimaryWithinNamingScheme { get; set; }
    }

    public class IdentifierType
    {
        public string Uri { get; set; }
        public string Notation { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }

    public class ObservingFacility
    {
        public string FacilityName { get; set; }
        public string FacilityNameAlias { get; set; }
        public string Description { get; set; }
        public string AccessAddress { get; set; }
        public double FacilityHeightAmsl { get; set; }
        public double ElevationAnglesLeeIndex { get; set; }
        public string ElevationAnglesComment { get; set; }
    }
}