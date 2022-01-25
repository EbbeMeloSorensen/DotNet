using System;
using System.Collections.Generic;

namespace DMI.SMS.Domain.Entities
{
    public class SensorLocation : ChildEntity
    {
        private StationInformation _stationinformation;
        private int? _stationid_dmi;
        private string _accessaddress;
        private DateTime? _datefrom;
        private DateTime? _dateto;
        private string _comment;
        private double? _barolevel;
        private int? _status;
        private int? _sl_utm;
        private double? _sl_northing;
        private double? _sl_easting;
        private double? _sl_geo_lat;
        private double? _sl_geo_long;

        public StationInformation StationInformation
        {
            get { return _stationinformation; }
            set { _stationinformation = value; }
        }

        public int? StationidDMI
        {
            get { return _stationid_dmi; }
            set { _stationid_dmi = value; }
        }

        public string AccessAddress
        {
            get { return _accessaddress; }
            set { _accessaddress = value; }
        }

        public DateTime? DateFrom
        {
            get { return _datefrom; }
            set { _datefrom = value; }
        }

        public DateTime? DateTo
        {
            get { return _dateto; }
            set { _dateto = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public double? BaroLevel
        {
            get { return _barolevel; }
            set { _barolevel = value; }
        }

        public int? Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public int? Sl_utm
        {
            get { return _sl_utm; }
            set { _sl_utm = value; }
        }

        public double? Sl_northing
        {
            get { return _sl_northing; }
            set { _sl_northing = value; }
        }

        public double? Sl_easting
        {
            get { return _sl_easting; }
            set { _sl_easting = value; }
        }

        public double? Sl_geo_lat
        {
            get { return _sl_geo_lat; }
            set { _sl_geo_lat = value; }
        }

        public double? Sl_geo_long
        {
            get { return _sl_geo_long; }
            set { _sl_geo_long = value; }
        }

        public SensorLocation()
        {
            SensorInformations = new HashSet<SensorInformation>();
            ImagesOfSensorLocation = new HashSet<ImagesOfSensorLocation>();
            HardwareAtLocation = new HashSet<HardwareAtLocation>();
        }

        public virtual ICollection<SensorInformation> SensorInformations { get; set; }
        public virtual ICollection<ImagesOfSensorLocation> ImagesOfSensorLocation { get; set; }
        public virtual ICollection<HardwareAtLocation> HardwareAtLocation { get; set; }
    }
}
