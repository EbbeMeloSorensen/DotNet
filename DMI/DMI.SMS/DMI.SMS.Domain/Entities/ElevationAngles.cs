using System;

namespace DMI.SMS.Domain.Entities
{
    public class ElevationAngles : ChildEntity
    {
        private SensorLocation _sensorLocation;
        private DateTime? _datefrom;
        private int? _angle_n;
        private int? _angle_ne;
        private int? _angle_e;
        private int? _angle_se;
        private int? _angle_s;
        private int? _angle_sw;
        private int? _angle_w;
        private int? _angle_nw;
        private int? _angleindex;
        private string? _anglecomment;
        private string? _serviceid;

        public SensorLocation SensorLocation
        {
            get { return _sensorLocation; }
            set { _sensorLocation = value; }
        }

        public DateTime? DateFrom
        {
            get { return _datefrom; }
            set { _datefrom = value; }
        }

        public int? Angle_N
        {
            get { return _angle_n; }
            set { _angle_n = value; }
        }

        public int? Angle_NE
        {
            get { return _angle_ne; }
            set { _angle_ne = value; }
        }

        public int? Angle_E
        {
            get { return _angle_e; }
            set { _angle_e = value; }
        }

        public int? Angle_SE
        {
            get { return _angle_se; }
            set { _angle_se = value; }
        }

        public int? Angle_S
        {
            get { return _angle_s; }
            set { _angle_s = value; }
        }

        public int? Angle_SW
        {
            get { return _angle_sw; }
            set { _angle_sw = value; }
        }

        public int? Angle_W
        {
            get { return _angle_w; }
            set { _angle_w = value; }
        }

        public int? Angle_NW
        {
            get { return _angle_nw; }
            set { _angle_nw = value; }
        }

        public int? AngleIndex
        {
            get { return _angleindex; }
            set { _angleindex = value; }
        }

        public string? AngleComment
        {
            get { return _anglecomment; }
            set { _anglecomment = value; }
        }

        public string? ServiceId
        {
            get { return _serviceid; }
            set { _serviceid = value; }
        }
    }
}
