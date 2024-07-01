using System.Collections.Generic;

namespace DMI.StatDB.Domain.Entities
{
    public class Station
    {
        private int _statid;
        private string _icao_id;
        private string _country;
        private string _source;

        public int StatID
        {
            get { return _statid; }
            set { _statid = value; }
        }

        public string IcaoId
        {
            get { return _icao_id; }
            set { _icao_id = value; }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public Station()
        {
            //Positions = new HashSet<Position>();
        }

        public virtual ICollection<Position> Positions { get; set; }
    }
}
