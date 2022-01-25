using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence
{
    public static class ContactPersonExtensions
    {
        public static ContactPerson Clone(
            this ContactPerson contactPerson)
        {
            var clone = new ContactPerson();
            clone.CopyAttributes(contactPerson);
            return clone;
        }

        public static void CopyAttributes(
            this ContactPerson contactPerson,
            ContactPerson other)
        {
            contactPerson.GdbArchiveOid = other.GdbArchiveOid;
            contactPerson.GlobalId = other.GlobalId;
            contactPerson.ObjectId = other.ObjectId;
            contactPerson.CreatedUser = other.CreatedUser;
            contactPerson.CreatedDate = other.CreatedDate;
            contactPerson.LastEditedUser = other.LastEditedUser;
            contactPerson.LastEditedDate = other.LastEditedDate;
            contactPerson.GdbFromDate = other.GdbFromDate;
            contactPerson.GdbToDate = other.GdbToDate;

            contactPerson.ParentGdbArchiveOid = other.ParentGdbArchiveOid;
            contactPerson.ParentGuid = other.ParentGuid;

            contactPerson.Name = other.Name;
            contactPerson.PhoneNumber = other.PhoneNumber;
            contactPerson.Email = other.Email;
            contactPerson.Date = other.Date;
            contactPerson.Description = other.Description;
        }
    }
}
