using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DMI.FD.Domain.IO
{
    public static class Helpers
    {
        public static IList<Parameter> ReadParametersFromJsonFile(
            string fileName)
        {
            List<Parameter> parameterList;

            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                parameterList = JsonConvert.DeserializeObject<List<Parameter>>(json);
            }

            return parameterList;
        }

        public static Dictionary<string, Dictionary<string, int>> ReadStationTableFromFile(
            this FileInfo stationTableFile)
        {
            var linesFromFile = File.ReadAllLines(stationTableFile.FullName);
            var parameters = linesFromFile.First().Split(',').Skip(1).Select(s => s.Trim()).ToList();

            return linesFromFile
                .Skip(1)
                .Select(l => l.Split(','))
                .Select(m => m.Select(x => x.Trim()).ToArray())
                .Select(l => new
                {
                    StationKey = l.First().ExtractStationId(),
                    Value = l.Skip(1).Select((w, i) => new { ParamKey = parameters[i], Value = w == "na" ? -1 : int.Parse(w) })
                })
                .ToDictionary(
                    x => x.StationKey,
                    x => x.Value.ToDictionary(y => y.ParamKey, y => y.Value));
        }

        public static void Aggregate(
            this Dictionary<string, Dictionary<string, int>> aggregatedResult,
            Dictionary<string, Dictionary<string, int>> partialResult)
        {
            foreach (var kvp1 in partialResult)
            {
                if (!aggregatedResult.ContainsKey(kvp1.Key))
                {
                    aggregatedResult[kvp1.Key] = new Dictionary<string, int>();
                }

                foreach (var kvp2 in kvp1.Value)
                {
                    if (!aggregatedResult[kvp1.Key].ContainsKey(kvp2.Key))
                    {
                        aggregatedResult[kvp1.Key][kvp2.Key] = 0;
                    }

                    aggregatedResult[kvp1.Key][kvp2.Key] += kvp2.Value;
                }
            }
        }

        private static string ExtractStationId(
            this string text)
        {
            return text.Trim().Split(' ').First();
        }
    }
}
