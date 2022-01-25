using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMI.SMS.Domain.Entities
{
    public class ServiceVisitReport : ChildEntity
    {
        private StationInformation _stationinformation;
        private DateTime? _date;
        private string _serviceId;

        public StationInformation StationInformation
        {
            get { return _stationinformation; }
            set { _stationinformation = value; }
        }

        public DateTime? Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string ServiceId
        {
            get { return _serviceId; }
            set { _serviceId = value; }
        }
    }
}
