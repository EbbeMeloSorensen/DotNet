namespace DMI.ObsDB.Domain.Entities
{
    public class ObservingFacility
    {
        private int _id;
        private int _statId;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int StatId
        {
            get { return _statId; }
            set { _statId = value; }
        }
    }
}
