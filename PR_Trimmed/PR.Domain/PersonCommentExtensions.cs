using PR.Domain.Entities.PR;

namespace PR.Domain
{
    public static class PersonCommentExtensions
    {
        public static PersonComment Clone(
            this PersonComment personComment)
        {
            var clone = new PersonComment();
            clone.CopyAttributes(personComment);
            return clone;
        }

        public static void CopyAttributes(
            this PersonComment person,
            PersonComment other)
        {
            person.ArchiveID = other.ArchiveID;
            person.Created = other.Created;
            person.Superseded = other.Superseded;
            person.ID = other.ID;
            person.Text = other.Text;
        }
    }
}