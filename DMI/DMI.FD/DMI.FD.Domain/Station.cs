using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DMI.FD.Domain
{
    public class Station
    {
        public static Dictionary<string, string> _countryMap = new Dictionary<string, string>
        {
            {"Grønland", "GRL"},
            {"Danmark", "DNK"}
        };

        public static string GenerateCSVHeader(bool includeStatus)
        {
            return
                $"{"stationId,",-10}" + " " +
                $"{"name,",-32}" + " " +
                $"{"country,",-11}" + " " +
                $"{"owner,",-39}" + " " +
                $"{"stationType,",-23}" + " " +
                (includeStatus ? $"{"status,",-11}" + " " : "") +
                $"{"height,",-11}" + " " +
                $"{"barHeight,",-10}" + " " +
                $"{"latitude,",-17}" + " " +
                $"{"longitude,",-18}" + " " +
                $"{"region,",-7}" + " " +
                $"{"timeOperationFrom,",-18}" + " " +
                $"{"wmoCountryCode,",-17}" + " " +
                $"{"wmoStationId",-13}";
        }

        public static string GenerateCSVHeaderFull(bool includeStatus)
        {
            return
                $"{"_id,",-39}" + " " +
                $"{"stationId,",-10}" + " " +
                $"{"name,",-32}" + " " +
                $"{"country,",-11}" + " " +
                $"{"owner,",-39}" + " " +
                $"{"stationType,",-23}" + " " +
                (includeStatus ? $"{"status,",-11}" + " " : "") +
                $"{"height,",-11}" + " " +
                $"{"barHeight,",-10}" + " " +
                $"{"latitude,",-17}" + " " +
                $"{"longitude,",-18}" + " " +
                $"{"region,",-7}" + " " +
                $"{"timeCreated,",-24}" + " " +
                $"{"timeOperationFrom,",-18}" + " " +
                $"{"timeOperationTo,",-18}" + " " +
                $"{"timeUpdated,",-24}" + " " +
                $"{"timeValidFrom,",-24}" + " " +
                $"{"timeValidTo,",-24}" + " " +
                $"{"wmoCountryCode,",-17}" + " " +
                $"{"wmoStationId",-13}";
        }

        public static List<string> GenerateCSVHeaderFullAsListOfString()
        {
            return new List<string>
            {
                //"_id",
                "stationId",
                "name",
                "country",
                "owner",
                "stationType",
                "status",
                "height",
                "barHeight",
                "latitude",
                "longitude",
                "region",
                "wmoCountryCode",
                "wmoStationId",
                "timeOperationFrom",
                "timeOperationTo",
                "timeValidFrom",
                "timeValidTo",
                "timeCreated",
                "timeUpdated",
            };
        }

        public static List<string> GenerateHeaderAsListOfString()
        {
            return new List<string>
            {
                "timeValidFrom",
                "timeValidTo",
                "timeCreated",
                "timeUpdated",
                "timeOperationFrom",
                "timeOperationTo",
                "stationId",
                "name",
                "country",
                "owner",
                "stationType",
                "status",
                "height",
                "barHeight",
                "latitude",
                "longitude",
                "region",
                "wmoCountryCode",
                "wmoStationId",
                "_id"
            };
        }

        public string _id { get; set; }
        public string country { get; set; }
        public List<InstrumentParameter> instrumentParameter { get; set; }
        public Location location { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public List<string> parameterId { get; set; }
        public string regionId { get; set; }
        public string stationId { get; set; }
        public string status { get; set; }
        public long? timeCreated { get; set; } // Det her lader til at være tidspunktet for hvornår den er sat ind
        public long? timeOperationFrom { get; set; }  // tilsyneladende identisk med timeOperationTo
        public long? timeOperationTo { get; set; } // tilsyneladende altid null
        public long? timeUpdated { get; set; } // tilsyneladende altid null
        public long? timeValidFrom { get; set; } // tilsyneladende identisk med timeOperationFrom
        public long? timeValidTo { get; set; }// tilsyneladende altid null
        public string type { get; set; }
        public string wmoCountryCode { get; set; }
        public string wmoStationId { get; set; }

        public Station()
        {
            instrumentParameter = new List<InstrumentParameter>();
            parameterId = new List<string>();
        }

        public Station Clone()
        {
            return new Station
            {
                _id = _id,
                country = country,
                instrumentParameter = instrumentParameter.Select(ip => ip.Clone()).ToList(),
                location = location.Clone(),
                name = name,
                owner = owner,
                parameterId = parameterId.Select(p => p).ToList(),
                regionId = regionId,
                stationId = stationId,
                status = status,
                timeCreated = timeCreated,
                timeOperationFrom = timeOperationFrom,
                timeOperationTo = timeOperationTo,
                timeUpdated = timeUpdated,
                timeValidFrom = timeValidFrom,
                timeValidTo = timeValidTo,
                type = type,
                wmoCountryCode = wmoCountryCode,
                wmoStationId = wmoStationId
            };
        }

        public string GenerateCSVLine(bool includeStatus)
        {
            var heightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.height);
            var barHeightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", instrumentParameter.SingleOrDefault()?.value);
            var latitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.latitude);
            var longitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.longitude);
            var regionIdAsString = string.Format("{0}", regionId);

            var date = timeOperationFrom.Value.ConvertFromEpochInMicrosecondsToDateTime();
            var timeOperationFromAsString = date.ToShortDateString();

            var wmoCountryCodeAsString = wmoCountryCode;
            var wmoStationIdAsString = wmoStationId;

            if (heightAsString == null || heightAsString == "")
            {
                heightAsString = "n/a";
            }

            if (barHeightAsString == null || barHeightAsString == "")
            {
                barHeightAsString = "n/a";
            }

            if (latitudeAsString == null || latitudeAsString == "")
            {
                latitudeAsString = "n/a";
            }

            if (longitudeAsString == null || longitudeAsString == "")
            {
                longitudeAsString = "n/a";
            }

            if (regionIdAsString == null || regionIdAsString == "")
            {
                regionIdAsString = "n/a";
            }

            if (timeOperationFromAsString == null || timeOperationFromAsString == "")
            {
                timeOperationFromAsString = "n/a";
            }

            if (wmoCountryCodeAsString == null || wmoCountryCodeAsString == "")
            {
                wmoCountryCodeAsString = "n/a";
            }

            if (wmoStationIdAsString == null || wmoStationIdAsString == "")
            {
                wmoStationIdAsString = "n/a";
            }

            return
                $"{"\"" + stationId + "\",",-10}" + " " +
                $"{"\"" + name + "\",",-32}" + " " +
                $"{"\"" + country + "\",",-11}" + " " +
                $"{"\"" + owner + "\",",-39}" + " " +
                $"{"\"" + type + "\",",-23}" + " " +
                (includeStatus ? $"{"\"" + status + "\",",-11}" + " " : "") +
                $"{heightAsString + ",",-11}" + " " +
                $"{barHeightAsString + ",",-10}" + " " +
                $"{latitudeAsString + ",",-17}" + " " +
                $"{longitudeAsString + ",",-18}" + " " +
                $"{regionIdAsString + ",",-7}" + " " +
                $"{timeOperationFromAsString + ",",-18}" + " " +
                $"{"\"" + wmoCountryCodeAsString + "\",",-17}" + " " +
                $"{"\"" + wmoStationIdAsString + "\"",-13}";
        }

        public string GenerateCSVLineFull(bool includeStatus)
        {
            var heightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.height);
            var barHeightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", instrumentParameter.SingleOrDefault()?.value);
            var latitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.latitude);
            var longitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.longitude);
            var regionIdAsString = string.Format("{0}", regionId);

            var timeCreatedAsString = timeCreated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeOperationFromAsString = timeOperationFrom.ConvertFromEpochInMicrosecondsToDateString();
            var timeOperationToAsString = timeOperationTo.ConvertFromEpochInMicrosecondsToDateString();
            var timeUpdatedAsString = timeUpdated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidFromAsString = timeValidFrom.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidToAsString = timeValidTo.ConvertFromEpochInMicrosecondsToDateTimeString(true);

            var wmoCountryCodeAsString = wmoCountryCode;
            var wmoStationIdAsString = wmoStationId;

            if (heightAsString == null || heightAsString == "")
            {
                heightAsString = "n/a";
            }

            if (barHeightAsString == null || barHeightAsString == "")
            {
                barHeightAsString = "n/a";
            }

            if (latitudeAsString == null || latitudeAsString == "")
            {
                latitudeAsString = "n/a";
            }

            if (longitudeAsString == null || longitudeAsString == "")
            {
                longitudeAsString = "n/a";
            }

            if (regionIdAsString == null || regionIdAsString == "")
            {
                regionIdAsString = "n/a";
            }

            if (timeCreatedAsString == null || timeCreatedAsString == "")
            {
                timeCreatedAsString = "n/a";
            }

            if (timeOperationFromAsString == null || timeOperationFromAsString == "")
            {
                timeOperationFromAsString = "n/a";
            }

            if (timeOperationToAsString == null || timeOperationToAsString == "")
            {
                timeOperationToAsString = "n/a";
            }

            if (timeUpdatedAsString == null || timeUpdatedAsString == "")
            {
                timeUpdatedAsString = "n/a";
            }

            if (timeValidFromAsString == null || timeValidFromAsString == "")
            {
                timeValidFromAsString = "n/a";
            }

            if (timeValidToAsString == null || timeValidToAsString == "")
            {
                timeValidToAsString = "n/a";
            }

            if (wmoCountryCodeAsString == null || wmoCountryCodeAsString == "")
            {
                wmoCountryCodeAsString = "n/a";
            }

            if (wmoStationIdAsString == null || wmoStationIdAsString == "")
            {
                wmoStationIdAsString = "n/a";
            }

            return
                $"{"\"" + _id + "\",",-39}" + " " +
                $"{"\"" + stationId + "\",",-10}" + " " +
                $"{"\"" + name + "\",",-32}" + " " +
                $"{"\"" + country + "\",",-11}" + " " +
                $"{"\"" + owner + "\",",-39}" + " " +
                $"{"\"" + type + "\",",-23}" + " " +
                (includeStatus ? $"{"\"" + status + "\",",-11}" + " " : "") +
                $"{heightAsString + ",",-11}" + " " +
                $"{barHeightAsString + ",",-10}" + " " +
                $"{latitudeAsString + ",",-17}" + " " +
                $"{longitudeAsString + ",",-18}" + " " +
                $"{regionIdAsString + ",",-7}" + " " +
                $"{timeCreatedAsString + ",",-24}" + " " +
                $"{timeOperationFromAsString + ",",-18}" + " " +
                $"{timeOperationToAsString + ",",-18}" + " " +
                $"{timeUpdatedAsString + ",",-24}" + " " +
                $"{timeValidFromAsString + ",",-24}" + " " +
                $"{timeValidToAsString + ",",-24}" + " " +
                $"{"\"" + wmoCountryCodeAsString + "\",",-17}" + " " +
                $"{"\"" + wmoStationIdAsString + "\"",-13}";
        }

        public List<String> AsCSVListOfStrings()
        {
            var heightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.height);
            var barHeightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", instrumentParameter.SingleOrDefault()?.value);
            var latitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.latitude);
            var longitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.longitude);
            var regionIdAsString = string.Format("{0}", regionId);

            var timeCreatedAsString = timeCreated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeOperationFromAsString = timeOperationFrom.ConvertFromEpochInMicrosecondsToDateString();
            var timeOperationToAsString = timeOperationTo.ConvertFromEpochInMicrosecondsToDateString();
            var timeUpdatedAsString = timeUpdated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidFromAsString = timeValidFrom.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidToAsString = timeValidTo.ConvertFromEpochInMicrosecondsToDateTimeString(true);

            if (heightAsString == null || heightAsString == "")
            {
                heightAsString = "n/a";
            }

            if (barHeightAsString == null || barHeightAsString == "")
            {
                barHeightAsString = "n/a";
            }

            if (latitudeAsString == null || latitudeAsString == "")
            {
                latitudeAsString = "n/a";
            }

            if (longitudeAsString == null || longitudeAsString == "")
            {
                longitudeAsString = "n/a";
            }

            if (regionIdAsString == null || regionIdAsString == "")
            {
                regionIdAsString = "n/a";
            }

            if (timeCreatedAsString == null || timeCreatedAsString == "")
            {
                timeCreatedAsString = "n/a";
            }

            if (timeOperationFromAsString == null || timeOperationFromAsString == "")
            {
                timeOperationFromAsString = "n/a";
            }

            if (timeOperationToAsString == null || timeOperationToAsString == "")
            {
                timeOperationToAsString = "n/a";
            }

            if (timeUpdatedAsString == null || timeUpdatedAsString == "")
            {
                timeUpdatedAsString = "n/a";
            }

            if (timeValidFromAsString == null || timeValidFromAsString == "")
            {
                timeValidFromAsString = "n/a";
            }

            if (timeValidToAsString == null || timeValidToAsString == "")
            {
                timeValidToAsString = "n/a";
            }

            return new List<string>
            {
                //_id,
                stationId == null ? "n/a" : stationId,
                name == null ? "n/a" : name,
                country == null ? "n/a" : country,
                owner == null ? "n/a" : owner,
                type == null ? "n/a" : type,
                status == null ? "n/a" : status,
                heightAsString,
                barHeightAsString,
                latitudeAsString,
                longitudeAsString,
                regionIdAsString,
                wmoCountryCode == null ? "n/a" : wmoCountryCode,
                wmoStationId == null ? "n/a" : wmoStationId,
                timeOperationFromAsString,
                timeOperationToAsString,
                timeValidFromAsString,
                timeValidToAsString,
                timeCreatedAsString,
                timeUpdatedAsString,
            };
        }

        public List<String> AsListOfStrings()
        {
            var heightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.height);
            var barHeightAsString = string.Format(CultureInfo.InvariantCulture, "{0}", instrumentParameter.SingleOrDefault()?.value);
            var latitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.latitude);
            var longitudeAsString = string.Format(CultureInfo.InvariantCulture, "{0}", location.longitude);
            var regionIdAsString = string.Format("{0}", regionId);

            var timeCreatedAsString = timeCreated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeOperationFromAsString = timeOperationFrom.ConvertFromEpochInMicrosecondsToDateString();
            var timeOperationToAsString = timeOperationTo.ConvertFromEpochInMicrosecondsToDateString();
            var timeUpdatedAsString = timeUpdated.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidFromAsString = timeValidFrom.ConvertFromEpochInMicrosecondsToDateTimeString(true);
            var timeValidToAsString = timeValidTo.ConvertFromEpochInMicrosecondsToDateTimeString(true);

            var wmoCountryCodeAsString = wmoCountryCode;
            var wmoStationIdAsString = wmoStationId;

            if (heightAsString == null || heightAsString == "")
            {
                heightAsString = "n/a";
            }

            if (barHeightAsString == null || barHeightAsString == "")
            {
                barHeightAsString = "n/a";
            }

            if (latitudeAsString == null || latitudeAsString == "")
            {
                latitudeAsString = "n/a";
            }

            if (longitudeAsString == null || longitudeAsString == "")
            {
                longitudeAsString = "n/a";
            }

            if (regionIdAsString == null || regionIdAsString == "")
            {
                regionIdAsString = "n/a";
            }

            if (timeCreatedAsString == null || timeCreatedAsString == "")
            {
                timeCreatedAsString = "n/a";
            }

            if (timeOperationFromAsString == null || timeOperationFromAsString == "")
            {
                timeOperationFromAsString = "n/a";
            }

            if (timeOperationToAsString == null || timeOperationToAsString == "")
            {
                timeOperationToAsString = "n/a";
            }

            if (timeUpdatedAsString == null || timeUpdatedAsString == "")
            {
                timeUpdatedAsString = "n/a";
            }

            if (timeValidFromAsString == null || timeValidFromAsString == "")
            {
                timeValidFromAsString = "n/a";
            }

            if (timeValidToAsString == null || timeValidToAsString == "")
            {
                timeValidToAsString = "n/a";
            }

            if (wmoCountryCodeAsString == null || wmoCountryCodeAsString == "")
            {
                wmoCountryCodeAsString = "n/a";
            }

            if (wmoStationIdAsString == null || wmoStationIdAsString == "")
            {
                wmoStationIdAsString = "n/a";
            }

            return new List<string>
            {
                timeValidFromAsString,
                timeValidToAsString,
                timeCreatedAsString,
                timeUpdatedAsString,
                timeOperationFromAsString,
                timeOperationToAsString,
                stationId == null ? "n/a" : stationId,
                name == null ? "n/a" : name,
                country == null ? "n/a" : country,
                owner == null ? "n/a" : owner,
                type == null ? "n/a" : type,
                status == null ? "n/a" : status,
                heightAsString,
                barHeightAsString,
                latitudeAsString,
                longitudeAsString,
                regionIdAsString,
                wmoCountryCodeAsString,
                wmoStationIdAsString,
                _id,
            };
        }
    }
}
