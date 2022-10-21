using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Domain
{
    public static class PersonAssociationExtensions
    {
        public static RecordAssociation Clone(
            this RecordAssociation personAssociation)
        {
            return new RecordAssociation
            {
                Id = personAssociation.Id,
                SubjectRecordId = personAssociation.SubjectRecordId,
                ObjectRecordId = personAssociation.ObjectRecordId,
                Description = personAssociation.Description,
                Created = personAssociation.Created
            };
        }

        public static void LinkToSubjectPerson(
            this RecordAssociation personAssociation,
            Record subjectPerson)
        {
            personAssociation.SubjectRecord = subjectPerson;

            if (subjectPerson.ObjectPeople == null)
            {
                subjectPerson.ObjectPeople = new List<RecordAssociation>();
            }

            subjectPerson.ObjectPeople.Add(personAssociation);
        }

        public static void LinkToObjectPerson(
            this RecordAssociation personAssociation,
            Record objectPerson)
        {
            personAssociation.ObjectRecord = objectPerson;

            if (objectPerson.SubjectPeople == null)
            {
                objectPerson.SubjectPeople = new List<RecordAssociation>();
            }

            objectPerson.SubjectPeople.Add(personAssociation);
        }

        public static void LinkToPeople(
            this RecordAssociation personAssociation,
            Record subjectPerson,
            Record objectPerson)
        {
            personAssociation.LinkToSubjectPerson(subjectPerson);
            personAssociation.LinkToObjectPerson(objectPerson);
        }

        public static void DecoupleFromSubjectPerson(
            this RecordAssociation personAssociation)
        {
            personAssociation.SubjectRecord.ObjectPeople.Remove(personAssociation);
        }

        public static void DecoupleFromObjectPerson(
            this RecordAssociation personAssociation)
        {
            personAssociation.ObjectRecord.SubjectPeople.Remove(personAssociation);
        }

        public static void DecoupleFromPeople(
            this RecordAssociation personAssociation)
        {
            personAssociation.DecoupleFromSubjectPerson();
            personAssociation.DecoupleFromObjectPerson();
        }

        public static RecordAssociation ConvertFromLegacyPersonAssociation(
            this Foreign.PersonAssociation personAssociation,
            Dictionary<int, Guid> personIdMap)
        {
            var result = new RecordAssociation
            {
                Id = Guid.NewGuid(),
                Description = personAssociation.Description,
                Created = personAssociation.Created.ToUniversalTime(),
                SubjectRecordId = personIdMap[personAssociation.SubjectPersonId],
                ObjectRecordId = personIdMap[personAssociation.ObjectPersonId]
            };

            return result;
        }
    }
}