namespace DMI.ObsDB.Domain.Entities
{
    public class TimeSeries
    {
        private int _id;
        private string _paramId;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string ParamId
        {
            get { return _paramId; }
            set { _paramId = value; }
        }
    }
}
