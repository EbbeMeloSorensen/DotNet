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

        private static async Task<bool> FetchData(
            string host,
            string database,
            string user,
            string password,
            ProgressCallback progressCallback = null)
        {
            var connectionOK = false;

            await Task.Run(async () =>
            {
                var dataProvider = new DataProvider(null);

                connectionOK = await dataProvider.CheckConnection(host, database, user, password);

                var currentActivity = "Making mohitos";
                var count = 0;
                var total = 317;

                while (count < total)
                {
                    count++;

                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }
            });

            return connectionOK;
        }
    }
}