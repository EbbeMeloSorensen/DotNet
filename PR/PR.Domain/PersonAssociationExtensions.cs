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

        public static void LinkToSubjectPerson(
            this PersonAssociation personAssociation,
            Person subjectPerson)
        {
            personAssociation.SubjectPerson = subjectPerson;

            if (subjectPerson.ObjectPeople == null)
            {
                subjectPerson.ObjectPeople = new List<PersonAssociation>();
            }

            subjectPerson.ObjectPeople.Add(personAssociation);
        }

        public static void LinkToObjectPerson(
            this PersonAssociation personAssociation,
            Person objectPerson)
        {
            personAssociation.ObjectPerson = objectPerson;

            if (objectPerson.SubjectPeople == null)
            {
                objectPerson.SubjectPeople = new List<PersonAssociation>();
            }

            objectPerson.SubjectPeople.Add(personAssociation);
        }

        public static void LinkToPeople(
            this PersonAssociation personAssociation,
            Person subjectPerson,
            Person objectPerson)
        {
            personAssociation.LinkToSubjectPerson(subjectPerson);
            personAssociation.LinkToObjectPerson(objectPerson);
        }

        public static void DecoupleFromSubjectPerson(
            this PersonAssociation personAssociation)
        {
            personAssociation.SubjectPerson.ObjectPeople.Remove(personAssociation);
        }

        public static void DecoupleFromObjectPerson(
            this PersonAssociation personAssociation)
        {
            personAssociation.ObjectPerson.SubjectPeople.Remove(personAssociation);
        }

        public static void DecoupleFromPeople(
            this PersonAssociation personAssociation)
        {
            personAssociation.DecoupleFromSubjectPerson();
            personAssociation.DecoupleFromObjectPerson();
        }
    }
}