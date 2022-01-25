using System;
using System.Collections.Generic;

namespace DMI.SMS.Domain.Entities
{
    public class StationInformation : EntityBase
    {
        private string _stationname;
        private int? _stationid_dmi;
        private StationType? _stationtype;
        private string _accessaddress;
        private Country? _country;
        private Status? _status;
        private DateTime? _datefrom;
        private DateTime? _dateto;
        private StationOwner? _stationowner;
        private string _comment;
        private string _stationid_icao;
        private string _referencetomaintenanceagreement;
        private string _facilityid;
        private int? _si_utm;
        private double? _si_northing;
        private double? _si_easting;
        private double? _si_geo_lat;
        private double? _si_geo_long;
        private int? _serviceinterval;
        private DateTime? _lastservicedate;
        private DateTime? _nextservicedate;
        private DateTime? _addworkforcedate;
        private DateTime? _lastvisitdate;
        private string _altstationid;
        private string _wmostationid;
        private string _regionid;
        private string _wigosid;
        private string _wmocountrycode;
        private double? _hha;
        private double? _hhp;
        private int? _wmorbsn;
        private int? _wmorbcn;
        private int? _wmorbsnradio;
        private double? _wgs_lat;
        private double? _wgs_long;
        private string _shape; // This is actually of the type st_geometry which is unknown to Npgsql, but we can read and write it as text, and when we write as text it is interpreted as a Hex string

        public string StationName
        {
            get { return _stationname; }
            set { _stationname = value; }
        }

        public int? StationIDDMI
        {
            get { return _stationid_dmi; }
            set { _stationid_dmi = value; }
        }

        public StationType? Stationtype
        {
            get { return _stationtype; }
            set { _stationtype = value; }
        }

        public string AccessAddress
        {
            get { return _accessaddress; }
            set { _accessaddress = value; }
        }

        public Country? Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public Status? Status
        {
            get { return _status; }
            set { _status = value; }
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

        public StationOwner? StationOwner
        {
            get { return _stationowner; }
            set { _stationowner = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string Stationid_icao
        {
            get { return _stationid_icao; }
            set { _stationid_icao = value; }
        }

        public string Referencetomaintenanceagreement
        {
            get { return _referencetomaintenanceagreement; }
            set { _referencetomaintenanceagreement = value; }
        }

        public string Facilityid
        {
            get { return _facilityid; }
            set { _facilityid = value; }
        }

        public int? Si_utm
        {
            get { return _si_utm; }
            set { _si_utm = value; }
        }

        public double? Si_northing
        {
            get { return _si_northing; }
            set { _si_northing = value; }
        }

        public double? Si_easting
        {
            get { return _si_easting; }
            set { _si_easting = value; }
        }

        public double? Si_geo_lat
        {
            get { return _si_geo_lat; }
            set { _si_geo_lat = value; }
        }

        public double? Si_geo_long
        {
            get { return _si_geo_long; }
            set { _si_geo_long = value; }
        }

        public int? Serviceinterval
        {
            get { return _serviceinterval; }
            set { _serviceinterval = value; }
        }

        public DateTime? Lastservicedate
        {
            get { return _lastservicedate; }
            set { _lastservicedate = value; }
        }

        public DateTime? Nextservicedate
        {
            get { return _nextservicedate; }
            set { _nextservicedate = value; }
        }

        public DateTime? Addworkforcedate
        {
            get { return _addworkforcedate; }
            set { _addworkforcedate = value; }
        }

        public DateTime? Lastvisitdate
        {
            get { return _lastvisitdate; }
            set { _lastvisitdate = value; }
        }

        public string Altstationid
        {
            get { return _altstationid; }
            set { _altstationid = value; }
        }

        public string Wmostationid
        {
            get { return _wmostationid; }
            set { _wmostationid = value; }
        }

        public string Regionid
        {
            get { return _regionid; }
            set { _regionid = value; }
        }

        public string Wigosid
        {
            get { return _wigosid; }
            set { _wigosid = value; }
        }

        public string Wmocountrycode
        {
            get { return _wmocountrycode; }
            set { _wmocountrycode = value; }
        }

        public double? Hha
        {
            get { return _hha; }
            set { _hha = value; }
        }

        public double? Hhp
        {
            get { return _hhp; }
            set { _hhp = value; }
        }

        public int? Wmorbsn
        {
            get { return _wmorbsn; }
            set { _wmorbsn = value; }
        }

        public int? Wmorbcn
        {
            get { return _wmorbcn; }
            set { _wmorbcn = value; }
        }

        public int? Wmorbsnradio
        {
            get { return _wmorbsnradio; }
            set { _wmorbsnradio = value; }
        }

        public double? Wgs_lat
        {
            get { return _wgs_lat; }
            set { _wgs_lat = value; }
        }

        public double? Wgs_long
        {
            get { return _wgs_long; }
            set { _wgs_long = value; }
        }

        public string Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        public StationInformation()
        {
            ContactPersons = new HashSet<ContactPerson>();
            LegalOwners = new HashSet<LegalOwner>();
            SensorLocations = new HashSet<SensorLocation>();
            StationKeepers = new List<StationKeeper>();
            MaintenanceRegulations = new List<MaintenanceRegulation>();
            Errors = new List<Error>();
        }

        public virtual ICollection<SensorLocation> SensorLocations { get; set; }
        public virtual ICollection<ContactPerson> ContactPersons { get; set; }
        public virtual ICollection<LegalOwner> LegalOwners { get; set; }
        public virtual ICollection<StationKeeper> StationKeepers { get; set; }
        public virtual ICollection<MaintenanceRegulation> MaintenanceRegulations { get; set; }
        public virtual ICollection<Error> Errors { get; set; }
        //public virtual ICollection<ServiceVisitReport> ServiceVisitReports { get; set; }
    }
}
