namespace MigrationScriptGenerator
{
    public class STATDB_Station
    {
        public int statid { get; set; }

        public DateTime start_time { get; set; }
        public int leeindex_n { get; set; }
        public int leeindex_ne { get; set; }
        public int leeindex_e { get; set; }
        public int leeindex_se { get; set; }
        public int leeindex_s { get; set; }
        public int leeindex_sw { get; set; }
        public int leeindex_w { get; set; }
        public int leeindex_nw { get; set; }
        public int leeindexindex { get; set; }

        public override string ToString()
        {
            return
                $"{statid,-10}, {start_time.ToShortDateString(),-10}, " +
                $"{leeindex_n,5}, {leeindex_ne,5}, {leeindex_e,5}, {leeindex_se,5}, {leeindex_s,5}, {leeindex_sw,5}, {leeindex_w,5}, {leeindex_nw,5}" +
                $"{leeindexindex,5}";
        }
    }
}