using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Craft.Utils;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.FD.Domain;

namespace DMI.SMS.Application
{
    public static class Helpers
    {
        private static Dictionary<Country, string> _countryCodeMap = new Dictionary<Country, string>
        {
            { Country.Denmark, "DNK" },
            { Country.Greenland, "GRL" },
            { Country.FaroeIslands, "FRO" }
        };

        private static Dictionary<StationType, string> _stationTypeMap = new Dictionary<StationType, string>
        {
            { StationType.Synop, "Synop"},
            { StationType.Strømstation, "Strømstation"},
            { StationType.SVK_gprs, "SVK gprs"},
            { StationType.Vandstandsstation, "Tide Gauge"},
            { StationType.GIWS, "GIWS"},
            { StationType.Pluvio, "Pluvio"},
            { StationType.SHIP_AWS, "SHIP AWS"},
            { StationType.Temp_ship, "Temp ship"},
            { StationType.Lynpejlestation, "Lynpejlestation"},
            { StationType.Radar, "Radar"},
            { StationType.Radiosonde, "Radiosonde"},
            { StationType.Historisk_stationstype, "Historisk stationstype"},
            { StationType.Manuel_nedbør, "Manual precipitation"},
            { StationType.Bølgestation, "Bølgestation"},
            { StationType.Snestation, "Manual snow"}
        };

        private static Dictionary<StationOwner, string> _stationOwnerNameMap = new Dictionary<StationOwner, string>
        {
            { StationOwner.DMI, "DMI" },
            { StationOwner.SVK, "SVK" },
            { StationOwner.Havne_Kommuner_mv, "Havne Kommuner mv" },
            { StationOwner.GC_net_Greenland_Climate_data, "GC Net (Greenland Climate Data)" },
            { StationOwner.Danske_lufthavne, "Danske lufthavne" },
            { StationOwner.MITT_GRL_lufthavne, "MITT/GRL lufthavne" },
            { StationOwner.Vejdirektoratet, "Vejdirektoratet" },
            { StationOwner.Synop_Aarhus_Uni, "Synop - Århus Uni" },
            { StationOwner.Asiaq, "Asiaq" },
            { StationOwner.Kystdirektoratet, "Kystdirektoratet / Coastal Authority" },
            { StationOwner.PROMICE_GEUS_PROMICE_net_i_Grønland, "PROMICE" },
            { StationOwner.Forsvaret, "Forsvaret" }
        };

        private static Dictionary<string, string> _kdiStationIdMap = new Dictionary<string, string>
        {
            { "24007", "9004201" },
            { "24006", "9004203" },
            { "24018", "9004303" },
            { "24123", "9005101" },
            { "24122", "9005103" },
            { "24125", "9005104" },
            { "24124", "9005110" },
            { "24132", "9005113" },
            { "24343", "9005201" },
            { "24342", "9005203" },
            { "24344", "9005210" },
            { "24328", "9005212" },
            { "24353", "9005213" },
            { "25147", "9006401" },
            { "26361", "9006501" },
            { "26346", "9006601" },
            { "25343", "9006701" },
            { "25344", "9006703" },
            { "26136", "9006801" },
            { "26137", "9006802" },
            { "26143", "9006901" },
            { "26144", "9006902" },
            { "25346", "9007101" },
            { "25347", "9007102" },
            { "26088", "9010101" },
            { "26089", "9010102" },
            { "26473", "9010201" },
            { "26474", "9010202" },
            { "28003", "9020101" },
            { "28004", "9020102" },
            { "28366", "9020201" },
            { "28367", "9020202" },
            { "28397", "9020301" },
            { "28398", "9020302" },
            { "28198", "9020401" },
            { "28199", "9020402" },
            { "31171", "9030101" },
            { "31172", "9030102" },
            { "31243", "9030201" },
            { "31244", "9030202" },
            { "31493", "9030301" },
            { "31494", "9030302" },
            { "32096", "9030401" },
            { "32098", "9030402" },
            { "31342", "9030501" },
            { "31343", "9030502" }
        };

        private static Dictionary<string, string> _stationsThatAreExemptedFromTheThresholdRule = new Dictionary<string, string>
        {
            { "20375", "Brovst" },
            { "23360", "Haderslev" }
        };

        private static Dictionary<string, string> _stationsThatMeasurePrecipitationManually = new Dictionary<string, string>
        {
            { "34270", "Narsarsuaq" },
            { "34320", "Danmarkshavn" },
            { "34339", "Ittoqqortoormiit" }
        };

        private static Dictionary<string, string> _stationsWhereWeExcludeTemperatureMeasurements = new Dictionary<string, string>
        {
            { "20566", "Hobro Havn I" },
            { "22058", "Randers Havn I" },
            { "22121", "Grenå Havn I" },
            { "22598", "Hov Havn I" },
            { "29038", "Holbæk Havn I" },
            { "30202", "Vedbæk Havn I" },
            { "30361", "Dragør Havn I" },
            { "30407", "Roskilde Havn I" },
            { "30478", "Køge Havn I" }
        };

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

            var groupedByObjectId = stationInformations.GroupByObjectId();

            var result = new List<StationInformation>();

            var temp = groupedByObjectId.Select(kvp => new
            {
                MinDateFrom = kvp.Value.Where(x => x.GdbToDate.Year == 9999 && x.DateFrom.HasValue).Min(y => y.DateFrom),
                ObjectId = kvp.Key
            }).OrderBy(s => s.MinDateFrom).ToList();

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
                // Diagnostics
                if (kvp.Key == 51)
                {
                    var a = 0;
                }

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

                //if (latestRow.StationIDDMI == 5735)
                if (latestRow.ObjectId == 108116)
                {
                    int a = 0;
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

        // This method is used in conjunction with transforming observation counts read from file into a proper data structure
        public static Dictionary<string, List<string>> ConvertToParameterListMap(
            this Dictionary<string, Dictionary<string, int>> referenceMap,
            int threshold)
        {
            // If the number of observations is less than the threshold then we regard it as invalid data and assume that the real number is zero
            // Exceptions to this are Brovst (20375) and Haderslev (23360) that are quite new snow stations with few observations
            // Besides, we manually add precip_past24h for 3 stations from Greenland since they're not included in ObsDB.
            // Finally, we reset counts of tw observations for a number of stations
            var result = new Dictionary<string, List<string>>();

            // Iterate over all stations
            foreach (var kvp1 in referenceMap)
            {
                // iterate over all parameters
                foreach (var kvp2 in kvp1.Value)
                {
                    if (kvp2.Value > threshold ||
                        (kvp2.Value > 0 && _stationsThatAreExemptedFromTheThresholdRule.Keys.Contains(kvp1.Key)))
                    {
                        // Make sure we don't add the tw parameter for the stations where we need to disregard that parameter
                        if (_stationsWhereWeExcludeTemperatureMeasurements.Keys.Contains(kvp1.Key) &&
                            kvp2.Key == "tw")
                        {
                            continue;
                        }

                        // Make sure we don't the list is initialized
                        if (!result.ContainsKey(kvp1.Key))
                        {
                            result[kvp1.Key] = new List<string>();
                        }

                        result[kvp1.Key].Add(kvp2.Key);
                    }
                }

                if (_stationsThatMeasurePrecipitationManually.Keys.Contains(kvp1.Key))
                {
                    // Make sure we don't the list is initialized
                    if (!result.ContainsKey(kvp1.Key))
                    {
                        result[kvp1.Key] = new List<string>();
                    }

                    result[kvp1.Key].Add("precip_past24h");
                }
            }

            return result;
        }

        public static IList<StationInformation> RollbackToPreviousDate(
            this IList<StationInformation> stationInformationRows,
            DateTime timeOfInterest)
        {
            // Identify rows, that were created after the given date of interest
            var rowsCreatedLaterThanTimeOfInterest = stationInformationRows
                .Where(row => row.GdbFromDate > timeOfInterest)
                .ToList();

            // Remove these rows (rows created after the given date of interest)
            var result = stationInformationRows
                .Except(rowsCreatedLaterThanTimeOfInterest)
                .ToList();

            // For each of the rows created after the given date of interest, identify the row that was superseded,
            // and change its state to current (by setting the value of the gdb_to_date field to 31-12-9999).
            // Note that it is only possible to identify such rows for existing objects that were updated after
            // the given time of interest. No row will be identified for objects that were created after the given
            // date of interest
            rowsCreatedLaterThanTimeOfInterest
                .Select(row => row.ObjectId)
                .Distinct()
                .ToList()
                .ForEach(objectId =>
                {
                    // Identify the LATEST row representing the given object (if any) that we need to include in the result
                    // (If it exists, then this row represented the current state of the object at the given time of interest)
                    var rowsWithSameObjectId = result
                    .Where(row => row.ObjectId == objectId)
                    .ToList();

                    if (rowsWithSameObjectId.Count > 0)
                    {
                        // Undo the logical deletion of that row 
                        var rowWeNeedToManipulate = rowsWithSameObjectId
                            .OrderBy(row => row.GdbFromDate)
                            .Last();

                        rowWeNeedToManipulate.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59);
                    }
                });

            // Identify any rows that were deleted after the date of interest
            var rowsDeletedAfterDateOfInterest = result
                .Where(row => row.GdbToDate > timeOfInterest && row.GdbToDate.Year != 9999)
                .ToList();

            // Change the state of these rows to current
            rowsDeletedAfterDateOfInterest.
                ForEach(row =>
                {
                    row.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59);
                });

            return result;
        }

        public static Station ConvertToFrieDataStation(
            this StationInformation smsStation,
            bool convertFromDMIStationIdToKDIStationId)
        {
            var station = new Station();

            try
            {
                station._id = Guid.NewGuid().ToString();

                if (smsStation.Country.HasValue)
                {
                    station.country = _countryCodeMap[smsStation.Country.Value];
                }

                if (smsStation.Hhp.HasValue)
                {
                    station.instrumentParameter.Add(new InstrumentParameter
                    {
                        parameterId = "bar_height",
                        value = smsStation.Hhp.Value
                    });
                }

                station.location = new Location
                {
                    height = smsStation.Hha,
                    latitude = smsStation.Wgs_lat.TrimCoordinate(4),
                    longitude = smsStation.Wgs_long.TrimCoordinate(4)
                };

                if (!string.IsNullOrEmpty(smsStation.StationName))
                {
                    station.name = smsStation.StationName.FixCapitalization();
                }

                station.owner = smsStation.StationOwner.HasValue ? _stationOwnerNameMap[smsStation.StationOwner.Value] : "n/a";
                station.regionId = smsStation.Regionid;
                station.stationId = smsStation.StationIDDMI.ToString().PrefixWithZeroIf4CharactersLong();

                if (convertFromDMIStationIdToKDIStationId &&
                    station.owner == "Kystdirektoratet / Coastal Authority")
                {
                    station.stationId = station.stationId.ConvertFromDMIStationIdToKDIStationId();
                }

                if (smsStation.Status.HasValue)
                {
                    station.status = smsStation.Status == 0 ? "Inactive" : "Active";
                }

                if (smsStation.DateFrom.HasValue)
                {
                    station.timeOperationFrom = smsStation.DateFrom.Value.AsEpochInMicroSeconds();
                }

                if (smsStation.DateTo.HasValue)
                {
                    station.timeOperationTo = smsStation.DateTo.Value.AsEpochInMicroSeconds();
                }

                station.timeValidFrom = smsStation.GdbFromDate.AsEpochInMicroSeconds();

                station.wmoCountryCode = smsStation.Wmocountrycode;
                station.wmoStationId = smsStation.Wmostationid;

                if (smsStation.Stationtype.HasValue)
                {
                    station.type = _stationTypeMap[smsStation.Stationtype.Value];

                    if (station.type == "Tide Gauge")
                    {
                        if (station.name.Contains(" II"))
                        {
                            station.type = "Tide-gauge-secondary";
                        }
                        else
                        {
                            station.type = "Tide-gauge-primary";
                        }
                    }
                }
                else
                {
                    var historicalTideGaugeStationIds = new List<int>
                    {
                        20048,
                        20098,
                        21008,
                        22332,
                        23292,
                        25148,
                        26458,
                        28232,
                        28233,
                        29392,
                        30018,
                        30337,
                        30338,
                        30339,
                        31572,
                        31618
                    };

                    if (!string.IsNullOrEmpty(station.stationId) && historicalTideGaugeStationIds.Contains(int.Parse(station.stationId)))
                    {
                        station.type = "Tide-gauge-primary";
                    }
                }

                // Hvis det er en række, som er superseded..
                if (smsStation.GdbToDate.Year < 9999)
                {
                    // ..så skal vi også angive den som værende superseded i Frie Data versionen
                    var time = smsStation.GdbToDate.AsEpochInMicroSeconds();
                    station.timeValidTo = time;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return station;
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

        private static Dictionary<string, List<StationInformation>> GroupByGlobalId(
            this IEnumerable<StationInformation> stationInformations)
        {
            var dictionary = new Dictionary<string, List<StationInformation>>();

            stationInformations.ToList().ForEach(s =>
            {
                if (!dictionary.ContainsKey(s.GlobalId))
                {
                    dictionary[s.GlobalId] = new List<StationInformation>();
                }

                dictionary[s.GlobalId].Add(s);
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

        private static double? TrimCoordinate(
            this double? coordinate,
            int decimals)
        {
            if (coordinate.HasValue)
            {
                return System.Math.Round(coordinate.Value, decimals);
            }

            return null;
        }

        private static string FixCapitalization(
            this string text)
        {
            int? indexOfSlashCharacter = null;

            if (text.Contains("/"))
            {
                indexOfSlashCharacter = text.IndexOf("/");
                text = text.Replace('/', ' ');
            }

            IEnumerable<string> words = text.Split(' ');

            var result = words.Select(w =>
            {
                if (w.ToUpper() == "II")
                {
                    return "II";
                }

                var firstLetter = w.Substring(0, 1).ToUpper();
                var remainingLetters = w.Substring(1, w.Length - 1).ToLower();
                return firstLetter + remainingLetters;
            }).Aggregate((c, n) => $"{c} {n}");

            if (indexOfSlashCharacter.HasValue)
            {
                var sb = new StringBuilder(result);
                sb[indexOfSlashCharacter.Value] = '/';

                result = sb.ToString();
            }

            return result;
        }

        private static string PrefixWithZeroIf4CharactersLong(
            this string s)
        {
            if (s.Length == 4)
            {
                return "0" + s;
            }

            return s;
        }

        private static string ConvertFromDMIStationIdToKDIStationId(
            this string s)
        {
            if (!_kdiStationIdMap.ContainsKey(s))
            {
                throw new ArgumentException("station owned by KDI with unknown id");

                // The map only includes the KDI stations that we have decided to publish,
                // so if we encounter a KDI staition that we done publish, then we simply return
                // the station id
            }

            return _kdiStationIdMap[s];
        }
    }
}
