
using System;

namespace DMI.SMS.ViewModel
{
    internal static class StringExtensions
    {
        public static string AsNanoqStationId(
            this string stationId)
        {
            if (stationId.Length == 4)
            {
                stationId = stationId.PadLeft(5, '0');
            }

            if (stationId.Substring(0, 2) == "04")
            {
                return stationId + "00";
            }

            if (stationId.Substring(0, 2) == "05")
            {
                return stationId + "20";
            }

            if (stationId.Substring(0, 2) == "06")
            {
                if (stationId == "06051" ||
                    stationId == "06116")
                {
                    return stationId + "01";
                }

                return stationId + "00";
            }

            if (stationId.Substring(0, 1) == "2" ||
                stationId.Substring(0, 1) == "3")
            {
                return stationId + "50";
            }

            throw new ArgumentException("Apparently not a valid station id");
        }
    }
}
