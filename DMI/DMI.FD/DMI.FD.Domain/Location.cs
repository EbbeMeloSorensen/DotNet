namespace DMI.FD.Domain
{
    public class Location
    {
        public double? height { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }

        public Location Clone()
        {
            return new Location
            {
                height = height,
                latitude = latitude,
                longitude = longitude
            };
        }
    }
}