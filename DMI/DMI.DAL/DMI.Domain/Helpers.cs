using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Craft.Logging;
using DMI.Utils;

namespace DMI.Domain
{
    public static class Helpers
    {
        private static Dictionary<int, string> _countryCodeMap = new Dictionary<int, string>
        {
            { 0, "DNK" },
            { 1, "GRL" },
            { 2, "FRO" },
        };

        private static Dictionary<int, string> _countryMap = new Dictionary<int, string>
        {
            { 0, "Denmark" },
            { 1, "Greenland" },
            { 2, "Faroe Islands" },
        };

        private static Dictionary<int, string> _statusMap = new Dictionary<int, string>
        {
            { 0, "Inactive" },
            { 1, "Active" },
        };

        private static Dictionary<int, string> _stationTypeMap = new Dictionary<int, string>
        {
            {0, "Synop"},
            {1, "Strømstation"},
            {2, "SVK gprs"},
            {3, "Tide Gauge"},
            {4, "GIWS"},
            {5, "Pluvio"},
            {6, "SHIP AWS"},
            {7, "Temp ship"},
            {8, "Lynpejlestation"},
            {9, "Radar"},
            {10, "Radiosonde"},
            {11, "Historisk stationstype"},
            {12, "Manual precipitation"},
            {13, "Bølgestation"},
            {14, "Manual snow"}
        };

        private static Dictionary<int, string> _sensorTypeNameMap = new Dictionary<int, string>
        {
            {0, "Vindsensor" },                                     // OK
            {1, "Vindsensor2" },                                    // OK
            {2, "Temperatur/Fugt" },                                // OK
            {3, "Temperatur/Fugt2" },                               // OK
            {4, "Nedbørmåler" },                                    // OK
            {5, "Barometer" },                                      // OK
            {6, "Barometer2" },                                     // OK
            {7, "Græstermometer" },                                 // OK
            {8, "Jordtermometer 10 cm" },                           // OK
            {9, "Jordtermometer 30 cm" },                           // OK
            {10, "Bladfugtmåler" },                                 // OK
            {11, "Globalstråling" },                                // OK
            {12, "PWS" },                                           // OK
            {13, "Ceilometer" },                                    // OK
            {14, "CTD/STD" },                                       // OK
            {15, "Radar (Vandstand)" },                             // OK
            {16, "Sonar" },                                         // OK
            {17, "Trykmåler" },                                     // OK
            {18, "Boblemåler" },                                    // OK
            {19, "Radiosonde" },                                    // OK
            {20, "ADCP" },                                          // OK
            {21, "Lynpejler" },                                     // OK
            {22, "Vandtemperatur" }                                 // OK
        };

        private static Dictionary<int, string> _stationOwnerNameMap = new Dictionary<int, string>
        {
            {0, "DMI" },
            {1, "SVK" },
            {2, "Havne Kommuner mv" },
            {3, "GC Net (Greenland Climate Data)" },
            {4, "Danske lufthavne" },
            {5, "MITT/GRL lufthavne" },
            {6, "Vejdirektoratet" },
            {7, "Synop - Århus Uni" },
            {8, "Asiaq" },
            {9, "Kystdirektoratet / Coastal Authority" },
            {10, "PROMICE" },
            {11, "Forsvaret" }
        };

        /*
        private static Dictionary<string, Tuple<double, double>> _historicStationLocationMap = new Dictionary<string, Tuple<double, double>>
        {
            {"20048", new Tuple<double, double>(57.59509360, 9.96251940) }, // ok // den her står ellers i databasen som den eneste..
            {"20098", new Tuple<double, double>(57.43583597, 10.54753875) }, // ok
            {"21008", new Tuple<double, double>(57.12159767, 8.60146647) }, // ok
            {"22332", new Tuple<double, double>(56.16926174, 10.22357630) }, // ok
            {"23292", new Tuple<double, double>(55.56016565, 9.75318509) }, // ok
            {"25148", new Tuple<double, double>(55.46016995, 8.43969464) }, // ok
            {"26458", new Tuple<double, double>(54.99473248, 9.98498412) }, // ok
            {"28232", new Tuple<double, double>(55.28798021, 10.82682190) }, // ok
            {"28233", new Tuple<double, double>(55.28798021, 10.82682190) }, // ok
            {"29392", new Tuple<double, double>(55.33546381, 11.13898406) }, // ok
            {"30018", new Tuple<double, double>(56.09339233, 12.45713855) }, // ok
            {"30337", new Tuple<double, double>(55.68943269, 12.59919553) }, // ok
            {"30338", new Tuple<double, double>(55.68943269, 12.59919553) }, // ok
            {"30339", new Tuple<double, double>(55.68959615, 12.59928098) }, // ok
            {"31572", new Tuple<double, double>(54.65511093, 11.34736453) }, // ok
            {"31618", new Tuple<double, double>(54.57233595, 11.92411157) }// ok
        };
        */

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

        private static Dictionary<string, int> _lightningSensorIdMap = new Dictionary<string, int>
        {
            { "30026", 1 },
            { "32249", 2 },
            { "26129", 3 },
            { "31622", 4 },
            { "24021", 5 },
            { "20077", 6 }
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

        private static List<string> _stationsThatOnlySwitchedNameBecauseATypoWasCorrected = new List<string>
        {
            "Angissoq",
            "Hevringsholm",
            "Hesseballe",
            "Oksvang Andrup"
        };

        public static string ConvertToStationType(
            this int stationType)
        {
            return _stationTypeMap[stationType];
        }

        public static int ConvertToStationTypeCode(
            this string stationType)
        {
            if (stationType.Contains("Tide-gauge"))
            {
                return _stationTypeMap.Single(kvp => kvp.Value == "Tide Gauge").Key;
            }

            return _stationTypeMap.Single(kvp => kvp.Value == stationType).Key;
        }

        public static int ConvertToSMSStationId(
            this string s)
        {
            if (_kdiStationIdMap.Values.Contains(s))
            {
                s = _kdiStationIdMap.Single(kvp => kvp.Value == s).Key;
            }

            return int.Parse(s);
        }

        public static string ConvertFromDMIStationIdToKDIStationId(
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

        public static FrieData.Station ConvertToFrieDataStation(
            this SMS.StationInformationRow smsStation,
            bool convertFromDMIStationIdToKDIStationId)
        {
            var station = new FrieData.Station();

            try
            {
                station._id = Guid.NewGuid().ToString();

                if (smsStation.country.HasValue)
                {
                    station.country = _countryCodeMap[smsStation.country.Value];
                }

                if (smsStation.hhp != null)
                {
                    station.instrumentParameter.Add(new FrieData.InstrumentParameter
                    {
                        parameterId = "bar_height",
                        value = smsStation.hhp.Value
                    });
                }

                station.location = new FrieData.Location
                {
                    height = smsStation.hha,
                    latitude = smsStation.wgs_lat.TrimCoordinate(4),
                    longitude = smsStation.wgs_long.TrimCoordinate(4)
                };

                if (smsStation.stationname != null)
                {
                    station.name = smsStation.stationname.FixCapitalization();
                }

                station.owner = smsStation.stationowner.HasValue ? _stationOwnerNameMap[smsStation.stationowner.Value] : "n/a";
                station.regionId = smsStation.regionid;
                station.stationId = smsStation.stationid_dmi.ToString().PrefixWithZeroIf4CharactersLong();

                if (convertFromDMIStationIdToKDIStationId &&
                    station.owner == "Kystdirektoratet / Coastal Authority")
                {
                    station.stationId = station.stationId.ConvertFromDMIStationIdToKDIStationId();
                }

                if (smsStation.status.HasValue)
                {
                    station.status = smsStation.status == 0 ? "Inactive" : "Active";
                }

                if (smsStation.datefrom.HasValue)
                {
                    station.timeOperationFrom = smsStation.datefrom.Value.AsEpochInMicroSeconds();
                }

                if (smsStation.dateto.HasValue)
                {
                    station.timeOperationTo = smsStation.dateto.Value.AsEpochInMicroSeconds();
                }

                if (smsStation.gdb_from_date.HasValue)
                {
                    station.timeValidFrom = smsStation.gdb_from_date.Value.AsEpochInMicroSeconds();
                }

                station.wmoCountryCode = smsStation.wmocountrycode;
                station.wmoStationId = smsStation.wmostationid;

                if (smsStation.stationtype.HasValue)
                {
                    station.type = _stationTypeMap[smsStation.stationtype.Value];

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
                if (smsStation.gdb_to_date.HasValue && smsStation.gdb_to_date.Value.Year < 9999)
                {
                    // ..så skal vi også angive den som værende superseded i Frie Data versionen
                    var time = smsStation.gdb_to_date.Value.AsEpochInMicroSeconds();
                    station.timeValidTo = time;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return station;
        }

        public static FrieData.OGC.LightningStation ConvertToFrieDataOGCLightningStation(
            this SMS.StationInformationRow smsStation,
            DateTime currentTime)
        {
            var station = new FrieData.OGC.LightningStation();

            station.type = "Feature";
            station._id = Guid.NewGuid().ToString();

            station.geometry = new FrieData.OGC.StationLocation
            {
                type = "Point",
                coordinates = new List<double?>
                {
                    smsStation.wgs_long.TrimCoordinate(4),
                    smsStation.wgs_lat.TrimCoordinate(4)
                }
            };

            station.lat = smsStation.wgs_lat.TrimCoordinate(4).Value;
            station.lon = smsStation.wgs_long.TrimCoordinate(4).Value;

            station.properties = new FrieData.OGC.LightningStationProperties
            {
                stationId = smsStation.stationid_dmi.ToString().PrefixWithZeroIf4CharactersLong(),
                sensorId = _lightningSensorIdMap[smsStation.stationid_dmi.ToString()].ToString(),
                name = smsStation.stationname,
                country = "DNK",
                owner = "DMI",
                status = smsStation.status.HasValue && smsStation.status.Value == 1 ? "Active" : "Inactive",
                type = "Lightning",
                timeOperationFrom = smsStation.datefrom.HasValue ? smsStation.datefrom.Value.AsRFC3339(false) : "",
                timeOperationTo = smsStation.dateto.HasValue ? smsStation.dateto.Value.AsRFC3339(false) : "",
                //timeValidFrom = smsStation.gdb_from_date.HasValue ? smsStation.gdb_from_date.Value.AsRFC3339() : "",
                timeValidFrom = smsStation.datefrom.HasValue ? smsStation.datefrom.Value.AsRFC3339(false) : "",
                timeValidTo = "",
                timeCreated = currentTime.AsRFC3339(false),
                timeUpdated = ""
            };

            return station;
        }

        public static FrieData.OGC.MeteorologicalStation ConvertToFrieDataOGCMeteorologicalStation(
            this FrieData.Station freeDataStation,
            DateTime currentTime)
        {
            var station = new FrieData.OGC.MeteorologicalStation();

            station.type = "Feature";
            station._id = Guid.NewGuid().ToString();
            station.lon = freeDataStation.location.longitude;
            station.lat = freeDataStation.location.latitude;
            station.timeValidTo = freeDataStation.timeValidTo;
            station.timeValidFrom = freeDataStation.timeValidFrom;

            station.geometry = new FrieData.OGC.StationLocation
            {
                type = "Point",
                coordinates = new List<double?>
                {
                    freeDataStation.location.longitude,
                    freeDataStation.location.latitude
                }
            };

            station.properties = new FrieData.OGC.MeteorologicalStationProperties
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

        public static FrieData.OGC.OceanographicalStation ConvertToFrieDataOGCOceanographicalStation(
            this FrieData.Station freeDataStation,
            DateTime currentTime)
        {
            var station = new FrieData.OGC.OceanographicalStation();

            station.type = "Feature";
            station._id = Guid.NewGuid().ToString();
            station.lon = freeDataStation.location.longitude;
            station.lat = freeDataStation.location.latitude;
            station.timeValidTo = freeDataStation.timeValidTo;
            station.timeValidFrom = freeDataStation.timeValidFrom;

            station.geometry = new FrieData.OGC.StationLocation
            {
                type = "Point",
                coordinates = new List<double?>
                {
                    freeDataStation.location.longitude,
                    freeDataStation.location.latitude
                }
            };

            station.properties = new FrieData.OGC.OceanographicalStationProperties
            {
                country = freeDataStation.country,
                instrumentParameter = freeDataStation.instrumentParameter,
                name = freeDataStation.name,
                owner = freeDataStation.owner,
                parameterId = freeDataStation.parameterId,
                regionId = freeDataStation.regionId,
                stationId = freeDataStation.stationId,
                status = freeDataStation.status,
                type = freeDataStation.type,
                //stationHeight = freeDataStation.location.height,
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

            //if (barHeightInstrumentParameter != null)
            //{
            //    station.properties.barometerHeight = barHeightInstrumentParameter.value;
            //}

            return station;
        }

        public static FrieData.Station TrimLatLongCoordinates(
            this FrieData.Station station,
            int decimals)
        {
            station.location.latitude = station.location.latitude.TrimCoordinate(decimals);
            station.location.longitude = station.location.longitude.TrimCoordinate(decimals);

            return station;
        }

        public static FrieData.Station ConvertToFrieDataStation(
            this SMS.Station smsStation)
        {
            var station = new FrieData.Station();

            station._id = smsStation.globalid
                .Replace("{", "")
                .Replace("}", "")
                .ToLower();

            station.country = smsStation.country == 0 ? "DNK" : "GRL";

            if (smsStation.hhp != null)
            {
                station.instrumentParameter.Add(new Domain.FrieData.InstrumentParameter
                {
                    parameterId = "bar_height",
                    value = smsStation.hhp.Value
                });
            }

            station.location = new Domain.FrieData.Location
            {
                height = smsStation.hha,
                latitude = smsStation.wgs_lat,
                longitude = smsStation.wgs_long
            };

            station.name = smsStation.stationname;
            station.owner = smsStation.stationowner == 0 ? "DMI" : "n/a";
            station.regionId = smsStation.regionid;
            station.stationId = smsStation.stationid_dmi.ToString().PrefixWithZeroIf4CharactersLong();
            station.status = smsStation.status == 0 ? "Inactive" : "Active";

            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long nowAsEpoch = (long)timeSpan.TotalMilliseconds * 1000;
            station.timeCreated = nowAsEpoch;

            if (smsStation.dateFrom.HasValue)
            {
                station.timeOperationFrom = smsStation.dateFrom.Value * 1000;
                station.timeValidFrom = smsStation.dateFrom.Value * 1000;
            }

            station.type = smsStation.stationtype.ConvertToStationType();
            station.wmoCountryCode = smsStation.wmocountrycode;
            station.wmoStationId = smsStation.wmostationid;

            return station;
        }

        public static string AsFrieDataStationId(
            this int stationId)
        {
            // Dette bruges til at generere Frie Data stationsid'er ud fra nogle trukket fra tabellen stat_parameter
            return stationId.ToString().Substring(0, stationId.ToString().Length - 2).PrefixWithZeroIf4CharactersLong();
        }

        // NB: Det kan være et KDI stationsid, der passes her
        public static string AsSeaDBStationId(
            this string stationId)
        {
            if (stationId.Length == 7 &&
                stationId.Substring(0, 2) == "90")
            {
                stationId = _kdiStationIdMap.Single(kvp => kvp.Value == stationId).Key;
            }

            if (stationId.Length == 5)
            {
                return stationId + "50";
            }

            throw new ArgumentException("Apparently not a valid station id");
        }

        public static string AsReferenceMapKey(
            this string stationId)
        {
            if (_kdiStationIdMap.Keys.Contains(stationId))
            {
                return _kdiStationIdMap[stationId];
            }

            return stationId;
        }

        public static string ConvertCountryCodeToString(
            this int countryCode)
        {
            return _countryMap[countryCode];
        }

        public static string ConvertSensorTypeCodeToString(
            this int sensorTypeCode)
        {
            return _sensorTypeNameMap[sensorTypeCode];
        }

        public static string ConvertStatusCodeToString(
            this int statusCode)
        {
            return _statusMap[statusCode];
        }

        public static string ConvertStationOwnerCodeToString(
            this int stationOwnerCode)
        {
            return _stationOwnerNameMap[stationOwnerCode];
        }

        public static HashSet<string> Difference(
            this SMS.StationInformationRow row1,
            SMS.StationInformationRow row2)
        {
            var columnsWithDifferentValues = new HashSet<string>();

            if (row1.objectid != row2.objectid)
            {
                //columnsWithDifferentValues.Add("objectId");
            }

            if (row1.stationname.FixCapitalization() != row2.stationname.FixCapitalization())
            {
                columnsWithDifferentValues.Add("stationname");
            }

            if (row1.stationid_dmi != row2.stationid_dmi)
            {
                columnsWithDifferentValues.Add("stationid_dmi");
            }

            if (row1.stationtype != row2.stationtype)
            {
                columnsWithDifferentValues.Add("stationtype");
            }

            if (row1.accessaddress != row2.accessaddress)
            {
                //columnsWithDifferentValues.Add("accessaddress");
            }

            if (row1.country != row2.country)
            {
                columnsWithDifferentValues.Add("country");
            }

            if (row1.status != row2.status)
            {
                columnsWithDifferentValues.Add("status");
            }

            if (row1.datefrom != row2.datefrom)
            {
                //columnsWithDifferentValues.Add("datefrom");
            }

            if (row1.dateto != row2.dateto)
            {
                //columnsWithDifferentValues.Add("dateto");
            }

            if (row1.stationowner != row2.stationowner)
            {
                columnsWithDifferentValues.Add("stationowner");
            }

            if (row1.comment != row2.comment)
            {
                //columnsWithDifferentValues.Add("comment");
            }

            if (row1.stationid_icao != row2.stationid_icao)
            {
                //columnsWithDifferentValues.Add("stationid_icao");
            }

            if (row1.referencetomaintenanceagreement != row2.referencetomaintenanceagreement)
            {
                //columnsWithDifferentValues.Add("referencetomaintenanceagreement");
            }

            if (row1.facilityid != row2.facilityid)
            {
                //columnsWithDifferentValues.Add("facilityid");
            }

            if (row1.si_utm != row2.si_utm)
            {
                //columnsWithDifferentValues.Add("si_utm");
            }

            if (row1.si_northing != row2.si_northing)
            {
                //columnsWithDifferentValues.Add("si_northing");
            }

            if (row1.si_easting != row2.si_easting)
            {
                //columnsWithDifferentValues.Add("si_easting");
            }

            if (row1.si_geo_lat != row2.si_geo_lat)
            {
                //columnsWithDifferentValues.Add("si_geo_lat");
            }

            if (row1.si_geo_long != row2.si_geo_long)
            {
                //columnsWithDifferentValues.Add("si_geo_long");
            }

            if (row1.serviceinterval != row2.serviceinterval)
            {
                //columnsWithDifferentValues.Add("serviceinterval");
            }

            if (row1.lastservicedate != row2.lastservicedate)
            {
                //columnsWithDifferentValues.Add("lastservicedate");
            }

            if (row1.nextservicedate != row2.nextservicedate)
            {
                //columnsWithDifferentValues.Add("nextservicedate");
            }

            if (row1.addworkforcedate != row2.addworkforcedate)
            {
                //columnsWithDifferentValues.Add("addworkforcedate");
            }

            if (row1.globalid != row2.globalid)
            {
                //columnsWithDifferentValues.Add("globalid");
            }

            if (row1.created_user != row2.created_user)
            {
                //columnsWithDifferentValues.Add("created_user");
            }

            if (row1.created_date != row2.created_date)
            {
                //columnsWithDifferentValues.Add("created_date");
            }

            if (row1.last_edited_user != row2.last_edited_user)
            {
                //columnsWithDifferentValues.Add("last_edited_user");
            }

            if (row1.last_edited_date != row2.last_edited_date)
            {
                //columnsWithDifferentValues.Add("last_edited_date");
            }

            if (row1.gdb_archive_oid != row2.gdb_archive_oid)
            {
                //columnsWithDifferentValues.Add("gdb_archive_oid");
            }

            if (row1.gdb_from_date != row2.gdb_from_date)
            {
                //columnsWithDifferentValues.Add("gdb_from_date");
            }

            if (row1.gdb_to_date != row2.gdb_to_date)
            {
                //columnsWithDifferentValues.Add("gdb_to_date");
            }

            if (row1.lastvisitdate != row2.lastvisitdate)
            {
                //columnsWithDifferentValues.Add("lastvisitdate");
            }

            if (row1.altstationid != row2.altstationid)
            {
                //columnsWithDifferentValues.Add("altstationid");
            }

            if (row1.wmostationid != row2.wmostationid)
            {
                columnsWithDifferentValues.Add("wmostationid");
            }

            if (row1.regionid != row2.regionid)
            {
                columnsWithDifferentValues.Add("regionid");
            }

            if (row1.wigosid != row2.wigosid)
            {
                //columnsWithDifferentValues.Add("wigosid");
            }

            if (row1.wmocountrycode != row2.wmocountrycode)
            {
                columnsWithDifferentValues.Add("wmocountrycode");
            }

            if (row1.hha != row2.hha)
            {
                columnsWithDifferentValues.Add("hha");
            }

            if (row1.hhp != row2.hhp)
            {
                columnsWithDifferentValues.Add("hhp");
            }

            if (row1.wmorbsn != row2.wmorbsn)
            {
                //columnsWithDifferentValues.Add("wmorbsn");
            }

            if (row1.wmorbcn != row2.wmorbcn)
            {
                //columnsWithDifferentValues.Add("wmorbcn");
            }

            if (row1.wmorbsnradio != row2.wmorbsnradio)
            {
                //columnsWithDifferentValues.Add("wmorbsnradio");
            }

            if (row1.wgs_lat.TrimCoordinate(4).AsString() != row2.wgs_lat.TrimCoordinate(4).AsString())
            {
                columnsWithDifferentValues.Add("wgs_lat");
            }

            if (row1.wgs_long.TrimCoordinate(4).AsString() != row2.wgs_long.TrimCoordinate(4).AsString())
            {
                columnsWithDifferentValues.Add("wgs_long");
            }

            return columnsWithDifferentValues;
        }

        public static List<FrieData.Station> AsFrieDataStationHistory(
            this List<SMS.StationInformationRow> smsStationHistory,
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
            long timeNow,
            ILogger logger)
        {
            var result = new List<FrieData.Station>();

            var currentSMSRow = smsStationHistory.Last();
            var currentRow = smsStationHistory.Last().ConvertToFrieDataStation(convertFromDMIStationIdToKDIStationId);
            var currentStationName = currentRow.name;
            var currentStationId = currentRow.stationId;

            logger?.WriteLine(LogMessageCategory.Information, $"Establishing Free Data History for station {currentStationName} ({currentStationId})");

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

                //var latitude = _historicStationLocationMap[result.Last().stationId].Item1.TrimCoordinate(4);
                //var longitude = _historicStationLocationMap[result.Last().stationId].Item2.TrimCoordinate(4);

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

                FrieData.Station newLatestFrieDataStationRow = result.Single().Clone();
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

                //if (currentStationId == "06183") // Drogden Fyr
                //{
                //    // Det her gør vi ikke alligevel, fordi vi ikke i Frie Data påtager os at rette op på fejl i sms

                //    // Ifølge SMS så går Drogden Fyr tilbage til 1937,
                //    // men i ObsDB ligger der kun data fra Januar 1962, så det sætter vi den til i Frie Data
                //    oldestDate = new DateTime(1962, 1, 1).AsEpochInMicroSeconds();

                //    // I øvrigt er der i forbindelse med ingres-afviklingen blevet indsat en gammel højde.
                //    // Den nulstiller vi her

                //    for (var i = 0; i < smsStationHistory.Count; i++)
                //    {
                //        smsStationHistory[i].datefrom = new DateTime(1962, 1, 1);

                //        if (smsStationHistory[i].hha == 18)
                //        {
                //            smsStationHistory[i].hha = null;
                //        }
                //    }
                //}

                if (currentStationId == "34339") // Ittoqqortoormiit
                {
                    // Ifølge SMS så går Ittoqqortoormiit tilbage til 17. august 2005,
                    // men i ClimaDB ligger der kun data fra 1. September 2014, så det sætter vi den til i Frie Data
                    // (og det gør vi så ikke alligevel, fordi vi har besluttet, at det ikke er op til os at rette op på fejl i sms)
                    //oldestDate = new DateTime(2014, 9, 1).AsEpochInMicroSeconds();

                    //for (var i = 0; i < smsStationHistory.Count; i++)
                    //{
                    //    smsStationHistory[i].datefrom = new DateTime(2014, 9, 1);
                    //}
                }

                if (currentStationId == "23327") // Kolding snestation
                {
                    // Kolding snestation har ikke nogen wgs-koordinater i sms,
                    // så vi sætter den manuelt (koordinater modtaget af Ib Damsgård pr mail den 10. december 2020)
                    var wgs_lat = 55.471557;
                    var wgs_long = 9.484661;

                    for (var i = 0; i < smsStationHistory.Count; i++)
                    {
                        smsStationHistory[i].wgs_lat = wgs_lat;
                        smsStationHistory[i].wgs_long = wgs_long;
                    }
                }

                var rowWasMigratedFromStatDB = false;

                for (var i = smsStationHistory.Count - 1; i > 0; i--)
                {
                    var rowBefore = smsStationHistory[i - 1];
                    var rowAfter = smsStationHistory[i];

                    if (rowBefore.objectid != rowAfter.objectid)
                    {
                        // We need this for resetting height values for old rows (migrated from statdb)
                        rowWasMigratedFromStatDB = true;

                        if (!includeArchivedStations)
                        {
                            // Hvis vi opererer med at stationen kun anskues som den samme, hvis den har samme OBJEKT ID, så er vi færdige nu
                            break;
                        }
                    }

                    if (rowWasMigratedFromStatDB && rowBefore.hha != null)
                    {
                        rowBefore.hha = null;
                    }

                    if (setHeightToNullForTideGaugeStations && result.Last().type.Substring(0, 4).ToLower() == "tide")
                    {
                        rowBefore.hha = null;
                        rowAfter.hha = null;
                    }

                    if (overwriteCountry)
                    {
                        rowBefore.country = currentSMSRow.country;
                    }

                    if (overwriteOwner)
                    {
                        rowBefore.stationowner = currentSMSRow.stationowner;
                    }

                    if (overwriteRegionId)
                    {
                        rowBefore.regionid = currentSMSRow.regionid;
                    }

                    if (overwriteWMOCountryCode)
                    {
                        rowBefore.wmocountrycode = currentSMSRow.wmocountrycode;
                    }

                    if (overwriteWMOStationId)
                    {
                        rowBefore.wmostationid = currentSMSRow.wmostationid;
                    }

                    if (overwriteNameForStationsWithHistoricalTypos)
                    {
                        if (_stationsThatOnlySwitchedNameBecauseATypoWasCorrected.Contains(rowAfter.stationname))
                        {
                            rowBefore.stationname = rowAfter.stationname;
                        }
                    }

                    // Hvis wgs-koordinaterne ikke fremgår af den ældste af de 2 rækker men i øvrigt gælder,
                    // at UTM-koordinaterne fremgår af begge rækker og er identiske, så kopierer vi
                    // wgs-koordinaterne fra den nyeste til den ældste række.
                    // Vi gør dog en undtagelse for Tarm
                    if (fillOutMissingWgsCoordinatesIfUtmCoordinatesUnchanged)
                    {
                        // Hvis wgs-koordinaterne går fra at være kendt til at være ukendt..
                        if ((rowAfter.wgs_lat.HasValue && !rowBefore.wgs_lat.HasValue) ||
                            (rowAfter.wgs_long.HasValue && !rowBefore.wgs_long.HasValue))
                        {
                            // .. så check om utm-koordinaterne er kendt i begge rækker ..
                            if (rowAfter.si_utm == rowBefore.si_utm &&
                                rowAfter.si_northing == rowBefore.si_northing &&
                                rowAfter.si_easting == rowBefore.si_easting &&
                                rowAfter.si_geo_lat == rowBefore.si_geo_lat &&
                                rowAfter.si_geo_long == rowBefore.si_geo_long)
                            {
                                // .. og hvis de er, så kopier wgs-koordinaterne til den ælste række
                                rowBefore.wgs_lat = rowAfter.wgs_lat;
                                rowBefore.wgs_long = rowAfter.wgs_long;
                            }
                            else
                            {
                                // Lige præcis for Tarm overskriver vi lokationen, selv om UTM-koordinaterne
                                // er forskellige .... det skyldes, at Tarm blev oprettet i november 2019,
                                // og at dens lokation først blev indtastet i december 2019
                                // Sørg dog for ikke at skrive Tarms lokation for Borris, som var dens navn før!
                                // Her skal vi derimod tage lokationen fra kolonnerne si_geo_lat og si_geo_long, 
                                // hvor de tilsyneladende er blevet indtastet....
                                // Bemærk, at dette er en afvigelse fra at vi bare "bobler op", hvilket ellers er det eneste, der sker her
                                if (currentStationName.ToLower() == "tarm")
                                {
                                    if (rowBefore.stationname.ToLower() == "tarm")
                                    {
                                        rowBefore.wgs_lat = rowAfter.wgs_lat;
                                        rowBefore.wgs_long = rowAfter.wgs_long;
                                    }
                                    else if (rowBefore.stationname.ToLower() == "borris")
                                    {
                                        rowBefore.wgs_lat = rowBefore.si_geo_lat;
                                        rowBefore.wgs_long = rowBefore.si_geo_long;
                                    }
                                }
                                else
                                {
                                    logger?.WriteLine(LogMessageCategory.Information, $"Warning: Unable to determine historic location of station {currentStationName} ({currentStationId})");
                                }
                            }
                        }
                    }

                    if (fillOutMissingStationHeightIfLocationUnchanged)
                    {
                        if (!rowBefore.hha.HasValue &&
                            rowAfter.hha.HasValue)
                        {
                            if (rowBefore.wgs_lat.HasValue &&
                                rowBefore.wgs_long.HasValue &&
                                rowAfter.wgs_lat.HasValue &&
                                rowAfter.wgs_long.HasValue)
                            {
                                var latDiff = System.Math.Abs(rowAfter.wgs_lat.Value - rowBefore.wgs_lat.Value);
                                var longDiff = System.Math.Abs(rowAfter.wgs_long.Value - rowBefore.wgs_long.Value);

                                if (latDiff < 0.002 &&
                                    longDiff < 0.002)
                                {
                                    rowBefore.hha = rowAfter.hha;
                                }
                            }
                        }
                    }

                    // For vandstandsstationer overskriver vi eventuelle gamle navne med det nyeste navn
                    if (result.First().type.Substring(0, 4).ToLower() == "tide")
                    {
                        rowBefore.stationname = result.First().name;
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
                        // Når det sker, så "lukker vi hullet" ved at overskrive gdb_date_from i den seneste række
                        if (rowAfter.gdb_from_date != rowBefore.gdb_to_date)
                        {
                            result.Last().timeValidFrom = rowBefore.gdb_to_date.Value.AsEpochInMicroSeconds();
                        }
                    }

                    // Opdaterer også "fødselsdag" og "dødsdag" (skal gøres uanset om der er forskel)
                    oldestDate = UpdateOldestDateTime(oldestDate, newEarliestFrieDataStationRowCandidate);
                    mostRecentDate = UpdateMostRecentDateTime(mostRecentDate, newEarliestFrieDataStationRowCandidate);

                    if (columnsWithDifferentValues.Count > 0)
                    {
                        // Check lige om det er en hhp, der er indtastet umiddelbart efter en hha
                        var timeSpanOfRowBefore = rowBefore.gdb_to_date.Value - rowBefore.gdb_from_date.Value;

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
                            rowBefore.hha = rowAfter.hha;
                            rowBefore.hhp = rowAfter.hhp;
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
                        result.Last().timeValidFrom = newEarliestFrieDataStationRowCandidate.timeValidFrom;
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
                    if (result.First().stationId == "20002" && result.Count() == 2)
                    {
                        // For Skagen havn har Ib sat en dødsdato en måned tidligere, hvorefter han har nedlagt stationen
                        // derfor gør vi dette for at få virkningstidsintervallerne til at passe
                        result.First().timeValidTo = result.First().timeOperationTo;
                        result.Last().timeValidFrom = result.Last().timeOperationTo;
                    }
                    else if (result.First().stationId == "31063" && result.Count() == 2)
                    {
                        // Rødvig Havn har ændret sig en gang. Ikke sikkert vi behøver gøre noget...
                    }
                    else
                    {
                        // Her burde vi ikke lande under nogen omstændigheder
                        throw new NotImplementedException();
                    }
                }
            }

            return result;
        }

        public static List<FrieData.Station> AsFrieDataStationHistoryCollapsedToActual(
            this List<FrieData.Station> stationsWithHistory)
        {
            var result = stationsWithHistory
                .Where(s => s.timeValidTo == null)
                .ToList();

            result.ForEach(s =>
            {
                s.timeValidFrom = s.timeOperationFrom;
                s.timeValidTo = s.timeOperationTo;
            });

            return result;
        }

        // Acts like a Min operator, returning the oldest date of the one passed to the method
        // and the timeValidFrom and timeOperationFrom fields of the station row.
        // Note that an actual date overrides a null
        private static long? UpdateOldestDateTime(
            long? oldestSoFar,
            FrieData.Station station)
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
            FrieData.Station station)
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

        public static SMS.ElevationAnglesRow CorrectFields(
            this SMS.ElevationAnglesRow original,
            DateTime now)
        {
            //var newGlobalId = $"{{{Guid.NewGuid().ToString().ToUpper()}}}";
            var newUser = "ebs@DOIT";

            return new SMS.ElevationAnglesRow
            {
                objectid = original.objectid,         // same as original
                datefrom = original.datefrom,         // same as original
                angle_n = original.angle_s,           // opposite
                angle_ne = original.angle_sw,         // opposite
                angle_e = original.angle_w,           // opposite
                angle_se = original.angle_nw,         // opposite
                angle_s = original.angle_n,           // opposite
                angle_sw = original.angle_ne,         // opposite
                angle_w = original.angle_e,           // opposite
                angle_nw = original.angle_se,         // opposite
                angleindex = original.angleindex,     // same as original
                anglecomment = original.anglecomment, // kommentar om at de er korrigeret af mig
                serviceid = original.serviceid,       // same as original
                parentguid = original.parentguid,     // same as original
                //globalid = newGlobalId.ToString(),  // create new
                globalid = original.globalid,         // same as original
                created_user = original.created_user, // same as original
                created_date = original.created_date, // same as original
                last_edited_user = newUser,           // Ebbe
                last_edited_date = now,               // current time
                //gdb_archive_oid = newGdbArchiveOID,   // New one - apparently it acts as primary key and has to be unique
                gdb_from_date = now,                  // current time
                gdb_to_date = original.gdb_to_date    // same as original
            };
        }

        public static bool IsStationGottenFromFarvandsvaesenetIn2012(
            this string stationId)
        {
            return _stationsTakenOverFromFarvandsvaesenet.ContainsKey(stationId);
        }
    }
}
