using System;

namespace DMI.StatDB.Domain.Entities
{
    public class Status
    {
        private int _statid;
        private DateTime _start_time;
        private bool _active;

        public int StatID
        {
            get { return _statid; }
            set { _statid = value; }
        }

        public DateTime StartTime
        {
            get { return _start_time; }
            set { _start_time = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
    }
}
