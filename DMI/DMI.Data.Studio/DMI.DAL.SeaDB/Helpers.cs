using System;
using System.Collections.Generic;
using System.Linq;

namespace DMI.DAL.SeaDB
{
    public static class Helpers
    {
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

        public static void Aggregate(
            this Dictionary<string, int> aggregatedResult,
            Dictionary<string, int> partialResult)
        {
            foreach (var kvp in partialResult)
            {
                if (!aggregatedResult.ContainsKey(kvp.Key))
                {
                    aggregatedResult[kvp.Key] = 0;
                }

                aggregatedResult[kvp.Key] += kvp.Value;
            }
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

    }
}
