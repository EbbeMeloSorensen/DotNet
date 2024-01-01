
using System;

namespace DMI.SMS.ViewModel
{
    public static class StringExtensions
    {
        public static string AsNanoqStationId(
            this string stationId,
            bool stripPrecedingNulls = false)
        {
            string result = null;

            if (stationId.Length == 4)
            {
                stationId = stationId.PadLeft(5, '0');
            }

            if (stationId.Substring(0, 2) == "04")
            {
                result = stationId + "00";
            }

            if (stationId.Substring(0, 2) == "05")
            {
                result = stationId + "20";
            }

            if (stationId.Substring(0, 2) == "06")
            {
                if (stationId == "06051" ||
                    stationId == "06116")
                {
                    result = stationId + "01";
                }
                else
                {
                    result = stationId + "00";
                }
            }

            if (stationId.Substring(0, 1) == "2" ||
                stationId.Substring(0, 1) == "3")
            {
                result = stationId + "50";
            }

            if (result == null)
            {
                throw new ArgumentException("Apparently not a valid station id");
            }

            if (stripPrecedingNulls)
            {
                while (result[0] == '0')
                {
                    result = result.Substring(1);
                }
            }

            return result;
        }
    }
}
