using System.Linq;
using CommandLine;
using Craft.Logging;
using DMI.DAL.ObsDB.UI.Console.Verbs;

namespace DMI.DAL.ObsDB.UI.Console
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("DMI.DAL.ObsDB.UI.Console");

            await Parser.Default.ParseArguments<
                    Lunch,
                    Fetch>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Fetch options) => FetchData(options),
                    errs => Task.FromResult(0));
        }

        private static async Task MakeLunch(Lunch options)
        {
            System.Console.Write("Making lunch...\nProgress: ");
            await DoSomethingSilly((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task FetchData(Fetch options)
        {
            System.Console.Write("Fetching Data...\nProgress: ");
            var result = await FetchData(
                options.Host, 
                options.Database, 
                options.User, 
                options.Password,
                options.StationID,
                options.Parameter,
                (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");

            System.Console.WriteLine($"\nResult: {result}");
        }

        private static async Task DoSomethingSilly(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                //Logger?.WriteLine(LogMessageCategory.Information, "Making breakfast..");

                var result = 0.0;
                var currentActivity = "Baking bread";
                var count = 0;
                var total = 317;

                //Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Pouring Milk";

                        if (count == 160)
                        {
                            //Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Frying eggs";

                        if (count == 80)
                        {
                            //Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }

                    for (var j = 0; j < 499999999 / 100; j++)
                    {
                        result += 1.0;
                    }

                    count++;

                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }

                //Logger?.WriteLine(LogMessageCategory.Information, "Completed breakfast");
            });
        }

        private static async Task<int> FetchData(
            string host,
            string database,
            string user,
            string password,
            string stationId_forget,
            string parameter,
            ProgressCallback progressCallback = null)
        {
            var observationCount = -1;

            await Task.Run(async () =>
            {
                var dataProvider = new DataProvider(null);
                dataProvider.Initialize(new[] { parameter });

                var stationIds = new List<string>{
                    "06011",
                    "06030",
                    "06041",
                    "06060",
                    "06071",
                    "06081",
                    "06110",
                    "06180"
                };

                var lastYear = 2023;
                var years = Enumerable.Range(1953, lastYear - 1953 + 1);

                foreach(var stationId in stationIds)
                {
                    foreach (var year in years)
                    {
                        var directoryInfo = new DirectoryInfo(Path.Combine($"{stationId}/{parameter}"));

                        if (!directoryInfo.Exists)
                        {
                            directoryInfo.Create();
                        }

                        var fileName = Path.Combine($"{stationId}/{parameter}", $"{stationId}_{parameter}_{year}.txt");
                        var file = new FileInfo(fileName);

                        if (file.Exists)
                        {
                            continue;
                        }

                        System.Console.WriteLine($"{stationId} - {year}");
                        using (var streamWriter = new StreamWriter(file.FullName))
                        {
                            Dictionary<string, List<Tuple<DateTime, float>>> observations = null;
                            var failedAttempts = 0;

                            while (failedAttempts < 10)
                            {
                                try
                                {
                                    observations = dataProvider.RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
                                        host,
                                        database,
                                        user,
                                        password,
                                        "temp_wind_radiation",
                                        stationId,
                                        year);

                                    break;
                                }
                                catch (Exception)
                                {
                                    failedAttempts++;
                                    Thread.Sleep(2000);
                                }
                            }

                            if (failedAttempts == 10)
                            {
                                throw new InvalidDataException();
                            }

                            var currentDayOfYear = 0;

                            if (observations == null || observations.First().Value.Count == 0)
                            {
                                streamWriter.WriteLine($"No observations");
                                continue;
                            }

                            foreach (var kvp in observations)
                            {
                                foreach (var observation in kvp.Value)
                                {
                                    var time = observation.Item1;
                                    var value = observation.Item2;

                                    if (currentDayOfYear != time.DayOfYear)
                                    {
                                        currentDayOfYear = time.DayOfYear;
                                        streamWriter.WriteLine($"{time.Year}-{time.Month}-{time.Day}");
                                    }

                                    var hour = time.Hour.ToString().PadLeft(2, '0');
                                    var minute = time.Minute.ToString().PadLeft(2, '0');
                                    var second = time.Second.ToString().PadLeft(2, '0');

                                    streamWriter.WriteLine($" {hour}:{minute}:{second} {value}");
                                }
                            }

                            streamWriter.Close();
                        }
                    }
                }
            });

            return observationCount;
        }
    }
}