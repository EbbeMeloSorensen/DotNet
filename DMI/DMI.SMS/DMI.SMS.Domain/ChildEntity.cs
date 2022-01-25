namespace DMI.SMS.Domain
{
    public abstract class ChildEntity : EntityBase
    {
        private string _parentguid;
        private int _parent_gdb_archive_oid;

        public string ParentGuid
        {
            get { return _parentguid; }
            set { _parentguid = value; }
        }

        public int ParentGdbArchiveOid
        {
            get { return _parent_gdb_archive_oid; }
            set { _parent_gdb_archive_oid = value; }
        }
    }
}