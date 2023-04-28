namespace ElevationAngleInspector
{
    public class SMS_Station
    {
        public int stationid_dmi { get; set; }
        
        public string stationname { get; set; }
        public double? wgs_lat { get; set; }
        public double? wgs_long { get; set; }
        public DateTime datefrom { get; set; }
        public int angle_n { get; set; }
        public int angle_ne { get; set; }
        public int angle_e { get; set; }
        public int angle_se { get; set; }
        public int angle_s { get; set; }
        public int angle_sw { get; set; }
        public int angle_w { get; set; }
        public int angle_nw { get; set; }
        public int angleindex { get; set; }

        public override string ToString()
        {
            return
                $"{stationid_dmi, -10}, {stationname, -40}, {wgs_lat, -12}, {wgs_long, -12}, {datefrom.ToShortDateString(), -10}, " + 
                $"{angle_n, 5}, {angle_ne, 5}, {angle_e, 5}, {angle_se, 5}, {angle_s, 5}, {angle_sw, 5}, {angle_w, 5}, {angle_nw, 5}" +
                $"{angleindex, 5}";
        }

        public string datefromAsDBString()
        {
            var year = datefrom.Year.ToString().PadLeft(2, '0');
            var month = datefrom.Month.ToString().PadLeft(2, '0');
            var day = datefrom.Day.ToString().PadLeft(2, '0');
            return $"{year}-{month}-{day} 00:00:00";
        }
    }
}