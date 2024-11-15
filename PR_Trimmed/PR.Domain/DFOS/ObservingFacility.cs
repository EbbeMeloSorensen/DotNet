namespace PR.Domain.DFOS
{
    public class ObservingFacility
    {
        public string FacilityName { get; set; }
        public string FacilityNameAlias { get; set; }
        public string Description { get; set; }
        public string AccessAddress { get; set; }
        public double FacilityHeightAmsl { get; set; }
        public double ElevationAnglesLeeIndex { get; set; }
        public string ElevationAnglesComment { get; set; }
        public Geometry GeoLocation { get; set; }
    }
}
