namespace C2IEDM.IO
{
    public abstract class EntityBase
    {
        private int _gdb_archive_oid;
        private string _globalid;
        private int _objectid;
        private string? _created_user;
        private DateTime? _created_date;
        private string? _last_edited_user;
        private DateTime? _last_edited_date;
        private DateTime _gdb_from_date;
        private DateTime _gdb_to_date;

        public int GdbArchiveOid
        {
            get { return _gdb_archive_oid; }
            set { _gdb_archive_oid = value; }
        }

        public string GlobalId
        {
            get { return _globalid; }
            set { _globalid = value; }
        }

        public int ObjectId
        {
            get { return _objectid; }
            set { _objectid = value; }
        }

        public string? CreatedUser
        {
            get { return _created_user; }
            set { _created_user = value; }
        }

        public DateTime? CreatedDate
        {
            get { return _created_date; }
            set { _created_date = value; }
        }

        public string? LastEditedUser
        {
            get { return _last_edited_user; }
            set { _last_edited_user = value; }
        }

        public DateTime? LastEditedDate
        {
            get { return _last_edited_date; }
            set { _last_edited_date = value; }
        }

        public DateTime GdbFromDate
        {
            get { return _gdb_from_date; }
            set { _gdb_from_date = value; }
        }

        public DateTime GdbToDate
        {
            get { return _gdb_to_date; }
            set { _gdb_to_date = value; }
        }
    }
}
