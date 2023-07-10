using System;
using System.Linq;
using System.Collections.Generic;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;

namespace DMI.SMS.Application
{
    public static class Helpers
    {
        public static List<StationInformation> Sort(
            this IEnumerable<StationInformation> stationInformations)
        {
            // Before, we did like this, but that does not yield a chronologic order while grouping records with same object id
            //var result = stationInformations
            //    .OrderBy(s => s.StationIDDMI)
            //    .ThenBy(s => s.ObjectId)
            //    .ThenBy(s => s.DateFrom)
            //    .ThenBy(s => s.GdbFromDate)
            //    .ToList();

            // Ideen er, at vi gerne vil sortere rækkerne på en måde så
            // man f.eks for Livgardens Kaserne ser de ældste objekter først,
            // Så man får et gant style view
            // Du ser imidlertid, at stationer ikke sorteres efter stationsid,
            // og det skal de helst være.. de er nemlig blot sorteret efter
            // MinDateFrom. Du vil gerne gøre det at du primært sorterer efter
            // stationsid og sekundært efter MinDateFrom

            var groupedByObjectId = stationInformations.GroupByObjectId();

            var result = new List<StationInformation>();

            // Her identificerer vi for hvert objekt id: evt stationsid, evt MinDateFrom 
            var temp = groupedByObjectId.Select(kvp => new
            {
                ObjectId = kvp.Key,
                StationId = kvp.Value.Last().StationIDDMI,
                MinDateFrom = kvp.Value.Where(x => x.GdbToDate.Year == 9999 && x.DateFrom.HasValue).Min(y => y.DateFrom)
            })
            .OrderBy(s => s.StationId)
            .ThenBy(s => s.MinDateFrom)
            .ToList();

            temp.ForEach(x =>
            {
                result.AddRange(groupedByObjectId[x.ObjectId].OrderBy(s => s.DateFrom).ThenBy(s => s.GdbFromDate));
            });

            return result;
        }

        // Denne funktion genererer et map, der for individuelle rækker angiver, om de er current, slettede eller uddaterede.
        // En "Current" række er kendetegnet ved at gdb_to_date er lig med datoen 9999-12-31 23:59:59
        // En "Outdated" række har en reel dato for gdb_to_date, og så haves der i øvrigt en current række, som er nyere
        // En "Deleted" række har OGSÅ en reel dato for gdb_to_date, men den har IKKE en nyere række.
        // Det betyder, at man kun kan skelne mellem outdated of deleted rækker, hvis man også har current rækker med
        public static Dictionary<int, RowCharacteristics> GenerateRowCharacteristicsMap(
            this IList<StationInformation> stationInformations)
        {
            var groupedByObjectId = stationInformations.GroupByObjectId();

            var result = new Dictionary<int, RowCharacteristics>();

            foreach (var kvp in groupedByObjectId)
            {
                var orderedChronologically = kvp.Value.OrderBy(s => s.GdbFromDate).ToList();
                var count = orderedChronologically.Count();

                if (count > 1)
                {
                    // Alle rækker med samme objekt id, pånær den sidste, markeres som værende outdated
                    foreach(var stationInformation in orderedChronologically.Take(count - 1))
                    {
                        result[stationInformation.GdbArchiveOid] = new RowCharacteristics(RowCondition.OutDated);
                    }
                }

                // Den sidste række markeres som værende current, hvis gdb_to_date er 9999-12-31 og ellers som deleted
                var latestRow = orderedChronologically.Last();
                var latestRowIsCurrent = latestRow.GdbToDate.Year == 9999;
                result[latestRow.GdbArchiveOid] = new RowCharacteristics(latestRowIsCurrent ? RowCondition.Current : RowCondition.Deleted);

                // Nu skal outdatede rækker markeres med information om, hvilke logiske felter der er blevet opdateret i en efterfølgende række
                if (count > 1)
                {
                    StationInformation previousStationInformationRecord = null;

                    // Denne tid skal bruges som start på virkningstidsintervaller for promotede records
                    // Vi initialiserer det som DateFrom for den første record i serien

                    var latestTimeInPastWhenHistoricallyRelevantFieldsWereChanged = new DateTime?();
                        
                    if (orderedChronologically.First().DateFrom.HasValue)
                    {
                        latestTimeInPastWhenHistoricallyRelevantFieldsWereChanged = orderedChronologically.First().DateFrom.Value;
                    }

                    foreach (var stationInformation in orderedChronologically)
                    {
                        var rowCharacteristicsCurrent = result[stationInformation.GdbArchiveOid];

                        if (previousStationInformationRecord == null)
                        {
                            // Vi er i gang med den første record med det givne objekt id
                            rowCharacteristicsCurrent.LatestTimeInPastWhenHistoricallyRelevantFieldsWereChanged = latestTimeInPastWhenHistoricallyRelevantFieldsWereChanged;
                        }
                        else
                        {
                            var rowCharacteristicsBefore = result[previousStationInformationRecord.GdbArchiveOid];

                            foreach (var updatedField in stationInformation.Compare(previousStationInformationRecord, false))
                            {
                                rowCharacteristicsBefore.AddUpdatedField(updatedField);
                            }

                            if (stationInformation.Compare(previousStationInformationRecord, true).Count > 0)
                            {
                                latestTimeInPastWhenHistoricallyRelevantFieldsWereChanged = previousStationInformationRecord.GdbToDate;
                            }

                            rowCharacteristicsCurrent.LatestTimeInPastWhenHistoricallyRelevantFieldsWereChanged = latestTimeInPastWhenHistoricallyRelevantFieldsWereChanged;
                        }

                        previousStationInformationRecord = stationInformation;
                    }
                }

                // Nu løber vi så rækkerne fra gruppen igennem for at checke, om INDIVIDUELLE rækker overtræder business rules
                if (latestRowIsCurrent)
                {
                    if (string.IsNullOrEmpty(latestRow.StationName))
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveAStationName);
                    }
                    else if (latestRow.StationName.ToUpper() == latestRow.StationName)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustNotHaveAStationNameWrittenInUpperCase);
                    }

                    if (!latestRow.Stationtype.HasValue)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveAStationType);
                    }

                    if (!latestRow.Country.HasValue)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveACountry);
                    }

                    if (!latestRow.Status.HasValue)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveAStatus);
                    }

                    if (!latestRow.StationOwner.HasValue)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveAStationOwner);
                    }

                    if (latestRow.Status.HasValue && latestRow.Status.Value == Status.Inactive)
                    {
                        if (!latestRow.DateTo.HasValue || latestRow.DateTo.Value.Year == 9999)
                        {
                            result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                                .Add(BusinessRule.ACurrentRowWithStatusInactiveMustHaveADateTo);
                        }
                    }

                    if (!latestRow.Wgs_lat.HasValue || !latestRow.Wgs_long.HasValue)
                    {
                        result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.ACurrentRowMustHaveWGSCoordinates);
                    }

                    if (!latestRow.Hha.HasValue)
                    {
                        // Not checking for now, because there are a lot of violations
                    }

                    if (latestRow.Stationtype.HasValue && latestRow.Stationtype.Value == StationType.Synop && !latestRow.Hhp.HasValue)
                    {
                        // Not checking for now, because there are a lot of violations
                    }
                }

                // Nu vil vi gerne checke, om der er entydighed for udvalgte historisk relevante properties
                if (count > 1 && latestRowIsCurrent && latestRow.DateFrom.HasValue)
                {
                    foreach (var stationInformation in orderedChronologically.Take(count - 1))
                    {
                        if (stationInformation.DateTo.HasValue && stationInformation.DateTo < stationInformation.GdbToDate)
                        {
                            // Vi har at gøre med en outdated record, der blev opdateret senere end det virkningstidsinterval, som den repræsenterer
                            // ..den er pr definition ikke i konflikt med den gældende række
                            continue;
                        }

                        if (stationInformation.StationName != latestRow.StationName &&  // Vi har en outdated record med et andet STATIONSNAVN
                            stationInformation.GdbToDate > latestRow.DateFrom)          // pågældende record blev opdateret i virkningstidsintervallet for den gældende række
                        {
                            // Vi vil ikke støje med de mange ændringer, hvor man blot har ændret fra store bogstaver til almindelig
                            if (!string.IsNullOrEmpty(stationInformation.StationName) &&
                                !string.IsNullOrEmpty(latestRow.StationName) &&
                                stationInformation.StationName.ToUpper() == latestRow.StationName.ToUpper())
                            {
                                continue;
                            }

                            result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                                .Add(BusinessRule.ObjectWasSubjectedToChangeOfNameSinceCreation);
                        }

                        if ((stationInformation.Wgs_lat != latestRow.Wgs_lat ||
                             stationInformation.Wgs_long != latestRow.Wgs_long) && // Vi har en outdated record med en anden LOKATION
                            stationInformation.GdbToDate > latestRow.DateFrom)     // pågældende record blev opdateret i virkningstidsintervallet for den gældende række
                        {
                            result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                                .Add(BusinessRule.ObjectWasSubjectedToChangeOfLocationSinceCreation);
                        }

                        if (stationInformation.Hha != latestRow.Hha &&         // Vi har en outdated record med en anden HHA (height)
                            stationInformation.GdbToDate > latestRow.DateFrom) // pågældende record blev opdateret i virkningstidsintervallet for den gældende række
                        {
                            result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                                .Add(BusinessRule.ObjectWasSubjectedToChangeOfHhaSinceCreation);
                        }

                        if (stationInformation.Hhp != latestRow.Hhp &&         // Vi har en outdated record med en anden HHP (barometer height)
                            stationInformation.GdbToDate > latestRow.DateFrom) // pågældende record blev opdateret i virkningstidsintervallet for den gældende række
                        {
                            result[latestRow.GdbArchiveOid].ViolatedBusinessRules
                                .Add(BusinessRule.ObjectWasSubjectedToChangeOfHhpSinceCreation);
                        }
                    }
                }
            }

            // Nu kigger vi på grupper af current records, der deler samme stationsid
            // for at se, om de overlapper i virkningstidsinterval (datefrom - dateto) ... genbesøg lige det her

            var groupedByStationId = stationInformations
                .Where(s => result[s.GdbArchiveOid].RowCondition == RowCondition.Current)
                .GroupByStationId();

            foreach (var kvp in groupedByStationId)
            {
                if (kvp.Value.Count <= 1)
                {
                    continue;
                }

                var ordered = kvp.Value
                    .Where(s => s.DateFrom.HasValue)
                    .Select(s => new {StartTime = s.DateFrom, EndTime = s.DateTo ?? DateTime.MaxValue})
                    .OrderBy(a => a.StartTime)
                    .ToList();

                var overlapIdentified = false;

                for (var i = 0; i < ordered.Count - 1; i++)
                {
                    if (ordered[i].EndTime <= ordered[i + 1].StartTime)
                    {
                        continue;
                    }

                    // Hvis vi er her, er det fordi vi har identificeret et overlap,
                    overlapIdentified = true;
                    break;
                }

                if (overlapIdentified)
                {
                    kvp.Value.ForEach(s =>
                    {
                        result[s.GdbArchiveOid].ViolatedBusinessRules
                            .Add(BusinessRule.OverlappingCurrentRecordsWithSameStationIdExists);
                    });
                }
            }

            return result;
        }

        private static Dictionary<int, List<StationInformation>> GroupByObjectId(
            this IEnumerable<StationInformation> stationInformations)
        {
            var dictionary = new Dictionary<int, List<StationInformation>>();

            stationInformations.ToList().ForEach(s =>
            {
                if (!dictionary.ContainsKey(s.ObjectId))
                {
                    dictionary[s.ObjectId] = new List<StationInformation>();
                }

                dictionary[s.ObjectId].Add(s);
            });

            return dictionary;
        }

        private static Dictionary<int, List<StationInformation>> GroupByStationId(
            this IEnumerable<StationInformation> stationInformations)
        {
            var dictionary = new Dictionary<int, List<StationInformation>>();

            stationInformations.ToList().ForEach(s =>
            {
                if (!s.StationIDDMI.HasValue)
                {
                    return;
                }

                if (!dictionary.ContainsKey(s.StationIDDMI.Value))
                {
                    dictionary[s.StationIDDMI.Value] = new List<StationInformation>();
                }

                dictionary[s.StationIDDMI.Value].Add(s);
            });

            return dictionary;
        }
    }
}
