using System;

namespace DMI.StatDB.Domain.Entities
{
    public class StationName
    {
        private int _statid;
        private string _name;
        private DateTime _start_time;

        public int StatID
        {
            get { return _statid; }
            set { _statid = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public DateTime StartTime
        {
            get { return _start_time; }
            set { _start_time = value; }
        }
    }
}
