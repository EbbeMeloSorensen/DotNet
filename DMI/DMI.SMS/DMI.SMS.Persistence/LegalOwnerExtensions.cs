using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence
{
    public static class LegalOwnerExtensions
    {
        public static LegalOwner Clone(
            this LegalOwner legalOwner)
        {
            var clone = new LegalOwner();
            clone.CopyAttributes(legalOwner);
            return clone;
        }

        public static void CopyAttributes(
            this LegalOwner legalOwner,
            LegalOwner other)
        {
            legalOwner.GdbArchiveOid = other.GdbArchiveOid;
            legalOwner.GlobalId = other.GlobalId;
            legalOwner.ObjectId = other.ObjectId;
            legalOwner.CreatedUser = other.CreatedUser;
            legalOwner.CreatedDate = other.CreatedDate;
            legalOwner.LastEditedUser = other.LastEditedUser;
            legalOwner.LastEditedDate = other.LastEditedDate;
            legalOwner.GdbFromDate = other.GdbFromDate;
            legalOwner.GdbToDate = other.GdbToDate;

            legalOwner.ParentGdbArchiveOid = other.ParentGdbArchiveOid;
            legalOwner.ParentGuid = other.ParentGuid;

            legalOwner.Name = other.Name;
            legalOwner.PhoneNumber = other.PhoneNumber;
            legalOwner.Email = other.Email;
            legalOwner.Date = other.Date;
            legalOwner.Description = other.Description;
        }
    }
}
