using System;
using System.Collections.Generic;

namespace DMI.SMS.Application
{
    public enum RowCondition
    {
        Current,
        OutDated,
        Deleted
    }

    public enum BusinessRule
    {
        ACurrentRowMustHaveAStationName,
        ACurrentRowMustNotHaveAStationNameWrittenInUpperCase,
        ACurrentRowMustHaveAStationType,
        ACurrentRowMustHaveACountry,
        ACurrentRowMustHaveAStationOwner,
        ACurrentRowMustHaveAStatus,
        ACurrentRowWithStatusInactiveMustHaveADateTo,
        ACurrentRowMustHaveWGSCoordinates,
        //ACurrentRowMustHaveAHeight,
        //ACurrentRowWithStationTypeSynopMustHaveABarometerHeight,
        //ACurrentRowHasAValidTimeIntervalSpanningSignificantChanges // Det checker vi endnu ikke for - det gælder f.eks. for Livgardens Kaserne - det findes ved at sammenholde med de uddaterede rækker
        ObjectWasSubjectedToChangeOfNameSinceCreation,
        ObjectWasSubjectedToChangeOfLocationSinceCreation,
        ObjectWasSubjectedToChangeOfHhaSinceCreation,
        ObjectWasSubjectedToChangeOfHhpSinceCreation,
        OverlappingCurrentRecordsWithSameStationIdExists
        // For Årslev er der overlap efter at man har promoted
        // Todo: Check for om der er overlap i tidsintervaller (datefrom-dateto) for gruppe af records med samme object id
        // Todo: Check for, om de observationer, der er foretaget under et givet stationsid, er dækket af de records, der har samme stations-id
    }

    public class RowCharacteristics
    {
        public RowCondition RowCondition { get; }
        public HashSet<BusinessRule> ViolatedBusinessRules { get; }
        public HashSet<string> FieldsUpdatedInSubsequentRecord { get; }
        public DateTime? LatestTimeInPastWhenHistoricallyRelevantFieldsWereChanged { get; set; } // Måske finder du lige er bedre navn hva?

        public RowCharacteristics(
            RowCondition rowCondition)
        {
            RowCondition = rowCondition;
            ViolatedBusinessRules = new HashSet<BusinessRule>();
            FieldsUpdatedInSubsequentRecord = new HashSet<string>();
        }

        public void AddViolatedBusinessRule(
            BusinessRule businessRule)
        {
            ViolatedBusinessRules.Add(businessRule);
        }

        public void AddUpdatedField(
            string field)
        {
            FieldsUpdatedInSubsequentRecord.Add(field);
        }
    }
}