using System.Collections.Generic;

namespace DMI.DAL.ObsDB
{
    public static class Helpers
    {
        public static void Aggregate(this Dictionary<string, int> aggregatedResult, Dictionary<string, int> partialResult)
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
    }
}
