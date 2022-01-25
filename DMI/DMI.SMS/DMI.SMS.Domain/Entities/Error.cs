using System;

namespace DMI.SMS.Domain.Entities
{
    public enum ErrorStatus
    {
        Loest,
        UnderBehandling
    }

    public class Error : ChildEntity
    {
        private StationInformation _stationinformation;
        private int? _stationid_dmi;
        private int? error_type; // Todo: Replace with enum (if you can find one...)
        private DateTime? _datefrom;
        private DateTime? _dateto;
        private string _description; // (500 chars)
        private ErrorStatus? _errorstatus;
        private string _skerpaasagen; // (100 chars)

        public StationInformation StationInformation
        {
            get { return _stationinformation; }
            set { _stationinformation = value; }
        }

        public int? StationIDDMI
        {
            get { return _stationid_dmi; }
            set { _stationid_dmi = value; }
        }

        public int? ErrorType
        {
            get { return error_type; }
            set { error_type = value; }
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

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ErrorStatus? ErrorStatus
        {
            get { return _errorstatus; }
            set { _errorstatus = value; }
        }

        public string SkerPaaSagen
        {
            get { return _skerpaasagen; }
            set { _skerpaasagen = value; }
        }
    }
}
