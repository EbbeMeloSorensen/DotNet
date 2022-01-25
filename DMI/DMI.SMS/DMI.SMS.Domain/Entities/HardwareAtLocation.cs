using System;

namespace DMI.SMS.Domain.Entities
{
    public class HardwareAtLocation : ChildEntity
    {
        private SensorLocation _sensorLocation;
        private string _hardwaretype; // (500 chars)
        private string _make_model; // (500 chars)
        private string _serialnumber; // (255 chars)
        private string _version; // (500 chars)
        private string _ip_address; // (500 chars)
        private Status? _status;
        private DateTime? _datefrom;
        private DateTime? _dateto;
        private DateTime? _lastupdate;
        private string _comment; // (500 chars)
        private string _serviceid;
        private DateTime? _lastchange;

        public SensorLocation SensorLocation
        {
            get { return _sensorLocation; }
            set { _sensorLocation = value; }
        }

        public string HardwareType
        {
            get { return _hardwaretype; }
            set { _hardwaretype = value; }
        }

        public string MakeModel
        {
            get { return _make_model; }
            set { _make_model = value; }
        }

        public string SerialNumber
        {
            get { return _serialnumber; }
            set { _serialnumber = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public string IpAddress
        {
            get { return _ip_address; }
            set { _ip_address = value; }
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

        public DateTime? LastUpdate
        {
            get { return _lastupdate; }
            set { _lastupdate = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string ServiceId
        {
            get { return _serviceid; }
            set { _serviceid = value; }
        }

        public DateTime? LastChange
        {
            get { return _lastchange; }
            set { _lastchange = value; }
        }
    }
}
