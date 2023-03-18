namespace DMI.Domain.SMS
{
    // This is the object type we use when retrieving current station information
    // for stations in the SMS system by means of its Rest API.
    // Notice that it only includes a subset of the available attributes 
    public class Station
    {
        public string globalid { get; set; }
        public string stationname { get; set; }
        public int stationid_dmi { get; set; }
        public int stationtype { get; set; }
        public int? country { get; set; }
        public int status { get; set; }
        public long? dateFrom { get; set; }
        public int? stationowner { get; set; }
        public string wmostationid { get; set; }
        public string regionid { get; set; }
        public string wmocountrycode { get; set; }
        public double? hha { get; set; }
        public double? hhp { get; set; }
        public double? wgs_lat { get; set; }
        public double? wgs_long { get; set; }
    }
}
