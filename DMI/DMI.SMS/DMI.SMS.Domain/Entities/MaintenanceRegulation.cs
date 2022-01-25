using System;

namespace DMI.SMS.Domain.Entities
{
    public class MaintenanceRegulation : ChildEntity
    {
        private StationInformation _stationinformation;
        private StationType? _stationtype;
        private DateTime? _datefrom;

        public StationInformation StationInformation
        {
            get { return _stationinformation; }
            set { _stationinformation = value; }
        }
    }
}
