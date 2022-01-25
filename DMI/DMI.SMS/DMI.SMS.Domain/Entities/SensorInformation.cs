using System;
using System.Collections.Generic;

namespace DMI.SMS.Domain.Entities
{
    public class SensorInformation : ChildEntity
    {
        private SensorLocation _sensorLocation;
        private SensorType? _sensortype; //
        private DateTime? _datefrom;
        private DateTime? _dateto;
        private Status? _status;
        private string _make_model;
        private string _serialnumber;
        private double? _heightoverterrain;
        private string _comment;
        private string _serviceid;
        private string _calibrationid;
        private string _imei;
        private DateTime? _lastchange;

        public SensorLocation SensorLocation
        {
            get { return _sensorLocation; }
            set { _sensorLocation = value; }
        }

        public SensorType? SensorType
        {
            get { return _sensortype; }
            set { _sensortype = value; }
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

        public Status? Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Make_model
        {
            get { return _make_model; }
            set { _make_model = value; }
        }

        public string SerialNumber
        {
            get { return _serialnumber; }
            set { _serialnumber = value; }
        }

        public double? HeightOverTerrain
        {
            get { return _heightoverterrain; }
            set { _heightoverterrain = value; }
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

        public string Calibrationid
        {
            get { return _calibrationid; }
            set { _calibrationid = value; }
        }

        public string Imei
        {
            get { return _imei; }
            set { _imei = value; }
        }

        public DateTime? LastChange
        {
            get { return _lastchange; }
            set { _lastchange = value; }
        }

        public SensorInformation()
        {
            SensorCalibrationInformations = new HashSet<SensorCalibrationInformation>();
        }

        public virtual ICollection<SensorCalibrationInformation> SensorCalibrationInformations { get; set; }
    }
}
