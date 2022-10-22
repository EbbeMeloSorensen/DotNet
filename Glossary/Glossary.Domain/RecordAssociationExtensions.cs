using System;
using System.Collections.Generic;
using Glossary.Domain.Entities;

namespace Glossary.Domain
{
    public static class RecordAssociationExtensions
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

        public static void LinkToSubjectRecord(
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

        public static void LinkToObjectRecord(
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
            this RecordAssociation recordAssociation,
            Record subjectRecord,
            Record objectRecord)
        {
            recordAssociation.LinkToSubjectRecord(subjectRecord);
            recordAssociation.LinkToObjectRecord(objectRecord);
        }

        public static void DecoupleFromSubjectRecord(
            this RecordAssociation recordAssociation)
        {
            recordAssociation.SubjectRecord.ObjectRecords.Remove(recordAssociation);
        }

        public static void DecoupleFromObjectRecord(
            this RecordAssociation recordAssociation)
        {
            recordAssociation.ObjectRecord.SubjectRecords.Remove(recordAssociation);
        }

        public static void DecoupleFromRecords(
            this RecordAssociation recordAssociation)
        {
            recordAssociation.DecoupleFromSubjectRecord();
            recordAssociation.DecoupleFromObjectRecord();
        }

        public static RecordAssociation ConvertFromLegacyPersonAssociation(
            this Foreign.PersonAssociation personAssociation,
            Dictionary<int, Guid> recordIdMap)
        {
            var result = new RecordAssociation
            {
                Id = Guid.NewGuid(),
                Description = personAssociation.Description,
                Created = personAssociation.Created.ToUniversalTime(),
                SubjectRecordId = recordIdMap[personAssociation.SubjectPersonId],
                ObjectRecordId = recordIdMap[personAssociation.ObjectPersonId]
            };

            return result;
        }
    }
}