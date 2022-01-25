namespace DMI.SMS.Domain.Entities
{
    public class SensorCalibrationInformation : ChildEntity
    {
        private SensorInformation _sensorInformation;

        public SensorInformation SensorInformation
        {
            get { return _sensorInformation; }
            set { _sensorInformation = value; }
        }
    }
}
