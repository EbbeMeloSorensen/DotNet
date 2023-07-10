using System;

namespace DMI.ObsDB.Domain.Entities
{
    public class Observation
    {
        private int _id;
        private int _statId;
        private int _paramId;
        private DateTime _time;
        private double _value;

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

        public int ParamId
        {
            get { return _paramId; }
            set { _paramId = value; }
        }

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
