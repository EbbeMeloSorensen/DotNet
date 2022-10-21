using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Domain
{
    public static class PersonAssociationExtensions
    {
        public static RecordAssociation Clone(
            this RecordAssociation recordAssociation)
        {
            return new RecordAssociation
            {
                Id = recordAssociation.Id,
                SubjectRecordId = recordAssociation.SubjectRecordId,
                ObjectRecordId = recordAssociation.ObjectRecordId,
                Description = recordAssociation.Description,
                Created = recordAssociation.Created
            };
        }

        public static void LinkToSubjectPerson(
            this RecordAssociation recordAssociation,
            Record subjectRecord)
        {
            recordAssociation.SubjectRecord = subjectRecord;

            if (subjectRecord.ObjectRecords == null)
            {
                subjectRecord.ObjectRecords = new List<RecordAssociation>();
            }

            subjectRecord.ObjectRecords.Add(recordAssociation);
        }

        public static void LinkToObjectPerson(
            this RecordAssociation recordAssociation,
            Record objectRecord)
        {
            recordAssociation.ObjectRecord = objectRecord;

            if (objectRecord.SubjectRecords == null)
            {
                objectRecord.SubjectRecords = new List<RecordAssociation>();
            }

            objectRecord.SubjectRecords.Add(recordAssociation);
        }

        public static void LinkToRecords(
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
            personAssociation.SubjectRecord.ObjectRecords.Remove(personAssociation);
        }

        public static void DecoupleFromObjectPerson(
            this RecordAssociation personAssociation)
        {
            personAssociation.ObjectRecord.SubjectRecords.Remove(personAssociation);
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