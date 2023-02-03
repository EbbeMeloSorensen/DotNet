using System;
using System.Linq;
using System.Collections.Generic;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Domain
{
    public static class Helpers
    {
        private static Dictionary<int, string> _countryCodeMap = new Dictionary<int, string>
        {
            { 0, "DNK" },
            { 1, "GRL" },
            { 2, "FRO" },
        };

        private static Dictionary<Country, int> _countryMap = new Dictionary<Country, int>
        {
            { Country.Denmark, 0 },
            { Country.Greenland, 1 },
            { Country.FaroeIslands, 2 }
        };

        private static Dictionary<Status, int> _statusMap = new Dictionary<Status, int>
        {
            { Status.Inactive, 0 },
            { Status.Active, 1 },
        };

        private static Dictionary<StationType, int> _stationTypeMap = new Dictionary<StationType, int>
        {
            {StationType.Synop, 0},
            {StationType.Strømstation, 1},
            {StationType.SVK_gprs, 2},
            {StationType.Vandstandsstation, 3},
            {StationType.GIWS, 4},
            {StationType.Pluvio, 5},
            {StationType.SHIP_AWS, 6},
            {StationType.Temp_ship, 7},
            {StationType.Lynpejlestation, 8},
            {StationType.Radar, 9},
            {StationType.Radiosonde, 10},
            {StationType.Historisk_stationstype, 11},
            {StationType.Manuel_nedbør, 12},
            {StationType.Bølgestation, 13},
            {StationType.Snestation, 14}
        };

        public static int ConvertToStationTypeCode(
            this StationType stationType)
        {
            return _stationTypeMap[stationType];
        }

        public static int ConvertToCountryCode(
            this Country country)
        {
            return _countryMap[country];
        }

        public static int ConvertToStatusCode(
            this Status status)
        {
            return _statusMap[status];
        }

        public static string AsDateTimeString(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            return result;
        }

        public static string AsDateString(
            this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}";
        }
    }
}