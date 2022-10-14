using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Craft.Utils;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.FD.Domain;
using DMI.FD.Domain.OGC;

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

        private static List<string> _stationsThatOnlySwitchedNameBecauseATypoWasCorrected = new List<string>
        {
            "Angissoq",
            "Hevringsholm",
            "Hesseballe",
            "Oksvang Andrup"
        };

        private static Dictionary<string, string> _stationsTakenOverFromFarvandsvaesenet = new Dictionary<string, string>
        {
            { "20002", "Skagen Havn" },
            { "22121", "Grenå Havn I" },
            { "22122", "Grenå Havn II" },
            { "23132", "Juelsminde Havn" },
            { "27084", "Ballen Havn" },
            { "28548", "Bagenkop Havn" },
            { "29002", "Havnebyen/Sjællands Odde" },
            { "30357", "Drogden Fyr" },
            { "31063", "Rødvig Havn" }
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

        public static string ConvertFromSMSStationIdToKDIStationId(
            this string smsStationId)
        {
            if (_kdiStationIdMap.Keys.Contains(smsStationId))
            {
                return _kdiStationIdMap[smsStationId];
            }

            return smsStationId;
        }

        public static int ConvertFromKDIStationIdToSMSStationId(
            this string kdiStationId)
        {
            if (_kdiStationIdMap.Values.Contains(kdiStationId))
            {
                kdiStationId = _kdiStationIdMap.Single(kvp => kvp.Value == kdiStationId).Key;
            }

            return int.Parse(kdiStationId);
        }

        public static StationType ConvertToStationType(
            this string stationType)
        {
            if (stationType.Contains("Tide-gauge"))
            {
                return _stationTypeMap.Single(kvp => kvp.Value == "Tide Gauge").Key;
            }

            return _stationTypeMap.Single(kvp => kvp.Value == stationType).Key;
        }

        public static List<Station> AsFrieDataStationHistory(
            this List<StationInformation> smsStationHistory,
            bool overwriteCountry,
            bool overwriteOwner,
            bool overwriteRegionId,
            bool overwriteWMOCountryCode,
            bool overwriteWMOStationId,
            bool overwriteNameForStationsWithHistoricalTypos,
            bool setHeightToNullForTideGaugeStations,
            bool fillOutMissingStationHeightIfLocationUnchanged,
            bool fillOutMissingWgsCoordinatesIfUtmCoordinatesUnchanged,
            bool includeArchivedStations,
            DateTime? timeOfOldestObservationForStation,
            DateTime? timeOfMostRecentObservationForStation,
            bool convertFromDMIStationIdToKDIStationId,
            long timeNow)
        {
            var result = new List<Station>();

            var currentSMSRow = smsStationHistory.Last();
            var currentRow = smsStationHistory.Last().ConvertToFrieDataStation(convertFromDMIStationIdToKDIStationId);
            var currentStationName = currentRow.name;
            var currentStationId = currentRow.stationId;

            result.Add(currentRow);

            if (result.Single().type.Substring(0, 4).ToLower() == "tide" &&
                result.Single().status == "Inactive" &&
                result.Single().stationId != "20002")
            {
                // Vi har at gøre med en HISTORISK VANDSTANDSSTATION (altså de tusse-gamle, dvs ikke Skagen Havn 20002), som skal have særbehandling
                if (!timeOfOldestObservationForStation.HasValue ||
                    !timeOfMostRecentObservationForStation.HasValue)
                {
                    throw new InvalidOperationException("The station is inactive - therefore we need the time of the oldest and most recent observation to determine the period in which it was active");
                }

                var firstActiveDate = timeOfOldestObservationForStation.Value.Date;
                var firstActiveDateAsEpochInMicroSeconds = firstActiveDate.AsEpochInMicroSeconds();

                var firstInactiveDate = timeOfMostRecentObservationForStation.Value.Date + new TimeSpan(1, 0, 0, 0);
                var firstInactiveDateAsEpochInMicroSeconds = firstInactiveDate.AsEpochInMicroSeconds();

                result.Single().owner = "DMI";
                result.Single().status = "Active";
                result.Single().timeOperationFrom = firstActiveDateAsEpochInMicroSeconds;
                result.Single().timeValidFrom = firstActiveDateAsEpochInMicroSeconds;
                result.Single().timeOperationTo = firstInactiveDateAsEpochInMicroSeconds;
                result.Single().timeValidTo = firstInactiveDateAsEpochInMicroSeconds;

                Station newLatestFrieDataStationRow = result.Single().Clone();
                newLatestFrieDataStationRow.status = "Inactive";
                newLatestFrieDataStationRow.timeValidFrom = result.Last().timeOperationTo;
                newLatestFrieDataStationRow.timeValidTo = null;
                result.Add(newLatestFrieDataStationRow);
            }
            else
            {
                // Vi har at gøre med en almindelig station (dvs IKKE en historisk vandstandsstation)
                // .. eller også har vi fat i Skagen Havn

                // We need to identify the oldest date associated with the station
                // It will be represented either by the oldest gdb_date_from or datefrom
                long? oldestDate = null;
                long? mostRecentDate = currentRow.timeOperationTo.HasValue ? currentRow.timeOperationTo.Value : new long?();

                oldestDate = UpdateOldestDateTime(oldestDate, currentRow);
                mostRecentDate = UpdateMostRecentDateTime(mostRecentDate, currentRow);

                if (setHeightToNullForTideGaugeStations && result.Single().type.Substring(0, 4).ToLower() == "tide")
                {
                    result.Single().location.height = null;
                }

                if (currentStationId == "23327") // Kolding snestation
                {
                    // Kolding snestation har ikke nogen wgs-koordinater i sms,
                    // så vi sætter den manuelt (koordinater modtaget af Ib Damsgård pr mail den 10. december 2020)
                    var wgs_lat = 55.471557;
                    var wgs_long = 9.484661;

                    for (var i = 0; i < smsStationHistory.Count; i++)
                    {
                        smsStationHistory[i].Wgs_lat = wgs_lat;
                        smsStationHistory[i].Wgs_long = wgs_long;
                    }
                }

                var rowWasMigratedFromStatDB = false;

                for (var i = smsStationHistory.Count - 1; i > 0; i--)
                {
                    var rowBefore = smsStationHistory[i - 1];
                    var rowAfter = smsStationHistory[i];

                    if (rowBefore.ObjectId != rowAfter.ObjectId)
                    {
                        // We need this for resetting height values for old rows (migrated from statdb)
                        rowWasMigratedFromStatDB = true;

                        if (!includeArchivedStations)
                        {
                            // Hvis vi opererer med at stationen kun anskues som den samme, hvis den har samme OBJEKT ID, så er vi færdige nu
                            break;
                        }
                    }

                    if (rowWasMigratedFromStatDB && rowBefore.Hha != null)
                    {
                        rowBefore.Hha = null;
                    }

                    if (setHeightToNullForTideGaugeStations && result.Last().type.Substring(0, 4).ToLower() == "tide")
                    {
                        rowBefore.Hha = null;
                        rowAfter.Hha = null;
                    }

                    if (overwriteCountry)
                    {
                        rowBefore.Country = currentSMSRow.Country;
                    }

                    if (overwriteOwner)
                    {
                        rowBefore.StationOwner = currentSMSRow.StationOwner;
                    }

                    if (overwriteRegionId)
                    {
                        rowBefore.Regionid = currentSMSRow.Regionid;
                    }

                    if (overwriteWMOCountryCode)
                    {
                        rowBefore.Wmocountrycode = currentSMSRow.Wmocountrycode;
                    }

                    if (overwriteWMOStationId)
                    {
                        rowBefore.Wmostationid = currentSMSRow.Wmostationid;
                    }

                    if (overwriteNameForStationsWithHistoricalTypos)
                    {
                        if (_stationsThatOnlySwitchedNameBecauseATypoWasCorrected.Contains(rowAfter.StationName))
                        {
                            rowBefore.StationName = rowAfter.StationName;
                        }
                    }

                    // Hvis wgs-koordinaterne ikke fremgår af den ældste af de 2 rækker men i øvrigt gælder,
                    // at UTM-koordinaterne fremgår af begge rækker og er identiske, så kopierer vi
                    // wgs-koordinaterne fra den nyeste til den ældste række.
                    // Vi gør dog en undtagelse for Tarm
                    if (fillOutMissingWgsCoordinatesIfUtmCoordinatesUnchanged)
                    {
                        // Hvis wgs-koordinaterne går fra at være kendt til at være ukendt..
                        if ((rowAfter.Wgs_lat.HasValue && !rowBefore.Wgs_lat.HasValue) ||
                            (rowAfter.Wgs_long.HasValue && !rowBefore.Wgs_long.HasValue))
                        {
                            // .. så check om utm-koordinaterne er kendt i begge rækker ..
                            if (rowAfter.Si_utm == rowBefore.Si_utm &&
                                rowAfter.Si_northing == rowBefore.Si_northing &&
                                rowAfter.Si_easting == rowBefore.Si_easting &&
                                rowAfter.Si_geo_lat == rowBefore.Si_geo_lat &&
                                rowAfter.Si_geo_long == rowBefore.Si_geo_long)
                            {
                                // .. og hvis de er, så kopier wgs-koordinaterne til den ælste række
                                rowBefore.Wgs_lat = rowAfter.Wgs_lat;
                                rowBefore.Wgs_long = rowAfter.Wgs_long;
                            }
                            else
                            {
                                // Lige præcis for Tarm overskriver vi lokationen, selv om UTM-koordinaterne
                                // er forskellige .... det skyldes, at Tarm blev oprettet i november 2019,
                                // og at dens lokation først blev indtastet i december 2019
                                // Sørg dog for ikke at skrive Tarms lokationen for Borris, som var dens navn før
                                if (currentStationName.ToLower() == "tarm")
                                {
                                    if (rowBefore.StationName.ToLower() == "tarm")
                                    {
                                        rowBefore.Wgs_lat = rowAfter.Wgs_lat;
                                        rowBefore.Wgs_long = rowAfter.Wgs_long;
                                    }
                                    else if (rowBefore.StationName.ToLower() == "borris")
                                    {
                                        rowBefore.Wgs_lat = rowBefore.Si_geo_lat;
                                        rowBefore.Wgs_long = rowBefore.Si_geo_long;
                                    }
                                }
                            }
                        }
                    }

                    if (fillOutMissingStationHeightIfLocationUnchanged)
                    {
                        if (!rowBefore.Hha.HasValue &&
                            rowAfter.Hha.HasValue)
                        {
                            if (rowBefore.Wgs_lat.HasValue &&
                                rowBefore.Wgs_long.HasValue &&
                                rowAfter.Wgs_lat.HasValue &&
                                rowAfter.Wgs_long.HasValue)
                            {
                                var latDiff = System.Math.Abs(rowAfter.Wgs_lat.Value - rowBefore.Wgs_lat.Value);
                                var longDiff = System.Math.Abs(rowAfter.Wgs_long.Value - rowBefore.Wgs_long.Value);

                                if (latDiff < 0.002 &&
                                    longDiff < 0.002)
                                {
                                    rowBefore.Hha = rowAfter.Hha;
                                }
                            }
                        }
                    }

                    // For vandstandsstationer overskriver vi eventuelle gamle navne med det nyeste navn
                    if (result.First().type.Substring(0, 4).ToLower() == "tide")
                    {
                        rowBefore.StationName = result.First().name;
                    }


                    // Nu ser vi så, om de 2 rækker er forskellige
                    var columnsWithDifferentValues = rowBefore.Difference(rowAfter);

                    // Vi laver den uanset om den skal tilføjes, da vi skal bruge dens timeValidFrom i tilfælde af at den ikke skal tilføjes
                    var newEarliestFrieDataStationRowCandidate = rowBefore.ConvertToFrieDataStation(convertFromDMIStationIdToKDIStationId);

                    if (columnsWithDifferentValues.Count > 0)
                    {
                        // De 2 rækker er tilsyneladende forskellige - nu finder vi så lige ud af, om de også er det i Frie Data perspektiv

                        // Når der sker en opdatering af en station, som allerede var i databasen, da man startede, så passer gdb_date_from generelt ikke med gdb_date_to
                        // (det er de tidspunkter, der er fremhævet med rødt i viewet)
                        // Når det sker, så "lukker vi hullet" ved at overskrive gdb_date_from i den senest tilføjede række med 
                        // den dato, som vi læser i dateto-feltet (i praksis skulle der gerne gælde, at denne dato altid er der. hvis denne antagelse viser sig
                        // ikke at holde, skal vi havbe kigget på det, og så er det måske en mulighed at bruge værdien fra gdb_to_date-feltet, som altid er der)
                        if (rowAfter.GdbFromDate != rowBefore.GdbToDate)
                        {
                            if (!rowBefore.DateTo.HasValue)
                            {
                                throw new NotImplementedException("We don't expect this to occur");
                            }

                            result.Last().timeValidFrom = rowBefore.DateTo.Value.AsEpochInMicroSeconds();
                        }
                    }

                    // Opdaterer også "fødselsdag" og "dødsdag" (skal gøres uanset om der er forskel)
                    oldestDate = UpdateOldestDateTime(oldestDate, newEarliestFrieDataStationRowCandidate);
                    mostRecentDate = UpdateMostRecentDateTime(mostRecentDate, newEarliestFrieDataStationRowCandidate);

                    if (columnsWithDifferentValues.Count > 0)
                    {
                        // Check lige om det er en hhp, der er indtastet umiddelbart efter en hha
                        var timeSpanOfRowBefore = rowBefore.GdbToDate - rowBefore.GdbFromDate;

                        // Hvis der er mindre end 10 minutter mellem hha og hhp, så opererer vi med at de effektivt er indtastet samtidigt
                        // Hvis det kun er hha eller hhp, der har ændret sig, og forskellen mellem indtastningerne er under 10 minutter,
                        // så gør vi lige som vi normalt gør, når vi ikke anskuer den for at være anderledes i Frie Data perspektiv,
                        // dvs vi ignorerer den, men snupper tidspunktet for hvornår den blev lavet og overskriver værdierne i den række,
                        // der repræsenterer næste tidsperiode
                        if (timeSpanOfRowBefore < new TimeSpan(0, 10, 0) &&
                            columnsWithDifferentValues.Count == 1 &&
                            (columnsWithDifferentValues.Single() == "hha" || columnsWithDifferentValues.Single() == "hhp"))
                        {
                            result.Last().timeValidFrom = newEarliestFrieDataStationRowCandidate.timeValidFrom;

                            // Sørg også for at skrive både hha og hhp i rowbefore, så det slår igennem som en samlet ændring,
                            // når vi kigger på næste række - uanset om hha føres op pga uændret lokation
                            rowBefore.Hha = rowAfter.Hha;
                            rowBefore.Hhp = rowAfter.Hhp;
                        }
                        else
                        {
                            result.Add(newEarliestFrieDataStationRowCandidate);
                        }
                    }
                    else
                    {
                        // Hvis ikke den er anderledes i Frie Data Perspektiv, så ignorerer vi den bare, men snupper tidspunktet for hvornår den blev lavet
                        // og overskriver værdierne i den række, der repræsenterer næste tidsperiode
                        // (Vi klapper så at sige de 2 rækker sammen)
                        // Bemærk, at vi IKKE må gøre dette, hvis datoen er nyere end den, der overskrives. Det gælder nemlig, når vi har at gøre med
                        // en række, der er lavet i henhold til de nye brugsregler
                        if (newEarliestFrieDataStationRowCandidate.timeValidFrom < result.Last().timeValidFrom)
                        {
                            result.Last().timeValidFrom = newEarliestFrieDataStationRowCandidate.timeValidFrom;
                        }
                    }
                }

                // Vend rækkefølgen, så den seneste række ligger til sidst
                result.Reverse();

                // Vi bruger ældste fundne dato til at markere ældste starttid for valid time
                result.First().timeValidFrom = oldestDate;

                // Lav korrektioner, der gælder for alle rækker (for almindelige stationer)
                result.ForEach(s =>
                {
                    s.timeOperationFrom = oldestDate;
                    s.timeOperationTo = mostRecentDate;
                });
            }

            // Lav korrektioner, der gælder for alle rækker (uanset, om det er almindelige stationer eller historiske vandstandsstationer)
            result.ForEach(s =>
            {
                s._id = Guid.NewGuid().ToString();
                s.timeCreated = timeNow;
            });

            // Vi laver lige en sidste korrektion for stationer overtaget fra Farvandsvæsenet i 2012
            if (_stationsTakenOverFromFarvandsvaesenet.Keys.Contains(result.Last().stationId))
            {
                var minTimeForFarvandsvaesenetsStationer = new DateTime(2012, 1, 1).AsEpochInMicroSeconds();

                result = result.SkipWhile(s => s.timeValidTo < minTimeForFarvandsvaesenetsStationer).ToList();

                result.ForEach(s =>
                {
                    if (s.timeOperationFrom < minTimeForFarvandsvaesenetsStationer)
                    {
                        s.timeOperationFrom = minTimeForFarvandsvaesenetsStationer;
                        s.timeValidFrom = minTimeForFarvandsvaesenetsStationer;
                    }
                });

                if (result.Count() > 1)
                {
                    // Her er vi lige netop for Skagen 20002, derfor skal vi lave en sidste korrektion
                    // Rent faktisk så lader Skagen havn til at være den eneste station for hvilken der gælder,
                    // at Ib har sat en dødsdato for en station ca en måned tidligere, hvorefter han har markeret
                    // stationen som nedlagt
                    if (result.First().stationId != "20002" ||
                        result.Count() != 2)
                    {
                        // Hvis vi er her, er der noget galt - nok fordi der er sket mere med Skagen Havn
                        throw new NotImplementedException();
                    }
                    else
                    {
                        result.First().timeValidTo = result.First().timeOperationTo;
                        result.Last().timeValidFrom = result.Last().timeOperationTo;
                    }
                }
            }

            return result;
        }

        public static Station TrimLatLongCoordinates(
            this Station station,
            int decimals)
        {
            station.location.latitude = station.location.latitude.TrimCoordinate(decimals);
            station.location.longitude = station.location.longitude.TrimCoordinate(decimals);

            return station;
        }

        public static MeteorologicalStation ConvertToFrieDataOGCMeteorologicalStation(
            this Station freeDataStation,
            DateTime currentTime)
        {
            var station = new MeteorologicalStation();

            station.type = "Feature";
            station._id = Guid.NewGuid().ToString();
            station.lon = freeDataStation.location.longitude;
            station.lat = freeDataStation.location.latitude;
            station.timeValidTo = freeDataStation.timeValidTo;
            station.timeValidFrom = freeDataStation.timeValidFrom;

            station.geometry = new StationLocation
            {
                type = "Point",
                coordinates = new List<double?>
                {
                    freeDataStation.location.longitude,
                    freeDataStation.location.latitude
                }
            };

            station.properties = new MeteorologicalStationProperties
            {
                country = freeDataStation.country,
                //instrumentParameter = freeDataStation.instrumentParameter,
                name = freeDataStation.name,
                owner = freeDataStation.owner,
                parameterId = freeDataStation.parameterId,
                regionId = freeDataStation.regionId,
                stationId = freeDataStation.stationId,
                status = freeDataStation.status,
                type = freeDataStation.type,
                stationHeight = freeDataStation.location.height,
                wmoCountryCode = freeDataStation.wmoCountryCode,
                wmoStationId = freeDataStation.wmoStationId,
                created = currentTime.AsRFC3339(false),
                updated = null,
                operationFrom = freeDataStation.timeOperationFrom.HasValue ? freeDataStation.timeOperationFrom.Value.AsRFC3339() : null,
                operationTo = freeDataStation.timeOperationTo.HasValue ? freeDataStation.timeOperationTo.Value.AsRFC3339() : null,
                validFrom = freeDataStation.timeValidFrom.HasValue ? freeDataStation.timeValidFrom.Value.AsRFC3339() : null,
                validTo = freeDataStation.timeValidTo.HasValue ? freeDataStation.timeValidTo.Value.AsRFC3339() : null
            };

            var barHeightInstrumentParameter = freeDataStation.instrumentParameter.SingleOrDefault(s => s.parameterId == "bar_height");

            if (barHeightInstrumentParameter != null)
            {
                station.properties.barometerHeight = barHeightInstrumentParameter.value;
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

        // Acts like a Min operator, returning the oldest date of the one passed to the method
        // and the timeValidFrom and timeOperationFrom fields of the station row.
        // Note that an actual date overrides a null
        private static long? UpdateOldestDateTime(
            long? oldestSoFar,
            Station station)
        {
            var result = oldestSoFar;

            if (station.timeValidFrom.HasValue &&
                (result == null || result.Value > station.timeValidFrom.Value))
            {
                result = station.timeValidFrom.Value;
            }

            if (station.timeOperationFrom.HasValue &&
                (result == null || result.Value > station.timeOperationFrom.Value))
            {
                result = station.timeOperationFrom.Value;
            }

            return result;
        }

        // Acts like a Max operator, returning the most recent date of the one passed to the method
        // and the timeValidTo and timeOperationTo fields of the station row.
        // Note that a null overrides an actual date, since null indicates that the station is still active
        private static long? UpdateMostRecentDateTime(
            long? mostRecentSoFar,
            Station station)
        {
            var result = mostRecentSoFar;

            if (!result.HasValue)
            {
                return result;
            }

            //if (station.timeValidTo.HasValue &&
            //    result.Value < station.timeValidTo.Value)
            //{
            //    result = station.timeValidTo.Value;
            //}

            if (station.timeOperationTo.HasValue &&
                result.Value < station.timeOperationTo.Value)
            {
                result = station.timeOperationTo.Value;
            }

            return result;
        }

        private static HashSet<string> Difference(
            this StationInformation row1,
            StationInformation row2)
        {
            var columnsWithDifferentValues = new HashSet<string>();

            if (row1.StationName.FixCapitalization() != row2.StationName.FixCapitalization())
            {
                columnsWithDifferentValues.Add("stationname");
            }

            if (row1.StationIDDMI != row2.StationIDDMI)
            {
                columnsWithDifferentValues.Add("stationid_dmi");
            }

            if (row1.Stationtype != row2.Stationtype)
            {
                columnsWithDifferentValues.Add("stationtype");
            }

            if (row1.Country != row2.Country)
            {
                columnsWithDifferentValues.Add("country");
            }

            if (row1.Status != row2.Status)
            {
                columnsWithDifferentValues.Add("status");
            }

            if (row1.StationOwner != row2.StationOwner)
            {
                columnsWithDifferentValues.Add("stationowner");
            }

            if (row1.Wmostationid != row2.Wmostationid)
            {
                columnsWithDifferentValues.Add("wmostationid");
            }

            if (row1.Regionid != row2.Regionid)
            {
                columnsWithDifferentValues.Add("regionid");
            }

            if (row1.Wmocountrycode != row2.Wmocountrycode)
            {
                columnsWithDifferentValues.Add("wmocountrycode");
            }

            if (row1.Hha != row2.Hha)
            {
                columnsWithDifferentValues.Add("hha");
            }

            if (row1.Hhp != row2.Hhp)
            {
                columnsWithDifferentValues.Add("hhp");
            }

            if (row1.Wgs_lat.TrimCoordinate(4).AsString() != row2.Wgs_lat.TrimCoordinate(4).AsString())
            {
                columnsWithDifferentValues.Add("wgs_lat");
            }

            if (row1.Wgs_long.TrimCoordinate(4).AsString() != row2.Wgs_long.TrimCoordinate(4).AsString())
            {
                columnsWithDifferentValues.Add("wgs_long");
            }

            return columnsWithDifferentValues;
        }

        private static string AsString(
            this double? number)
        {
            return number.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", number) : "";
        }
    }
}
