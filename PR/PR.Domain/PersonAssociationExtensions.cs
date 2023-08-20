using System;
using System.Collections.Generic;
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
                SubjectPersonId = personAssociation.SubjectPersonId,
                ObjectPersonId = personAssociation.ObjectPersonId,
                Description = personAssociation.Description,
                Created = personAssociation.Created
            };
        }
    }
}