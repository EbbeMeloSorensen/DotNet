using PR.Domain.Entities;

namespace PR.Domain
{
    public static class PersonAssociationExtensions
    {
        public static PersonAssociation Clone(
            this PersonAssociation personAssociation)
        {
            return new PersonAssociation
            {
                Id = personAssociation.Id,
                ObjectId = personAssociation.ObjectId,
                SubjectPersonId = personAssociation.SubjectPersonId,
                SubjectPersonObjectId = personAssociation.SubjectPersonObjectId,
                ObjectPersonId = personAssociation.ObjectPersonId,
                ObjectPersonObjectId = personAssociation.ObjectPersonObjectId,
                Description = personAssociation.Description,
                Created = personAssociation.Created,
                Superseded = personAssociation.Superseded
            };
        }
    }
}