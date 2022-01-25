using System;

namespace DMI.SMS.Domain.Entities
{
    public class ImagesOfSensorLocation : ChildEntity
    {
        private SensorLocation _sensorLocation;
        private DateTime? _dateofimage;
        private string _serviceid;

        public SensorLocation SensorLocation
        {
            get { return _sensorLocation; }
            set { _sensorLocation = value; }
        }

        public DateTime? Dateofimage
        {
            get { return _dateofimage; }
            set { _dateofimage = value; }
        }

        public string ServiceId
        {
            get { return _serviceid; }
            set { _serviceid = value; }
        }
    }
}
