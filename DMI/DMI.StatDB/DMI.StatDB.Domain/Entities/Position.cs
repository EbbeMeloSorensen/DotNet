using System;

namespace DMI.StatDB.Domain.Entities
{
    public class Position : ChildEntity
    {
        private Station _station;
        private int _statid;
        private DateTime? _start_time;
        private DateTime? _end_time;
        private double? _lat;
        private double? _long;
        private double? _height;

        public Station Station
        {
            get { return _station; }
            set { _station = value; }
        }

        public int StatID
        {
            get { return _statid; }
            set { _statid = value; }
        }

        public DateTime? StartTime
        {
            get { return _start_time; }
            set { _start_time = value; }
        }

        public DateTime? EndTime
        {
            get { return _end_time; }
            set { _end_time = value; }
        }

        public double? Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }

        public double? Long
        {
            get { return _long; }
            set { _long = value; }
        }

        public double? Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
