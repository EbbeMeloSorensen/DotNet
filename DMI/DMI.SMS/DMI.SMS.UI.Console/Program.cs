using CommandLine;
using DMI.SMS.Domain.Entities;
using DMI.SMS.UI.Console.Verbs;
using StructureMap;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        // Disse bruger vi i PR.UI.Console, men ikke umiddelbart her
        //private static string _host;
        //private static string _port;
        //private static string _database;
        //private static string _schema;
        //private static string _user;
        //private static string _password;

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DMI.SMS command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.Initialize();

            System.Console.WriteLine("Counting StationInformation records...");
            System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllStationInformations().Count}");

            // Works
            //System.Console.WriteLine("Exporting data...");
            //application.UIDataProvider.ExportData(".//Kylling.xml");
            //System.Console.WriteLine("Done...");

            // Works
            //await MakeBreakfast(application);
            //await ExportData(application);
            //await ExtractMeteorologicalStations(application);
            //await ExtractOceanographicalStations(application);
            //await GenerateSQLScriptForTurningElevationAngles(application);

            await Parser.Default.ParseArguments<
                    Lunch>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    errs => Task.FromResult(0));
        }

        public static async Task MakeLunch(Lunch options)
        {
            System.Console.Write("Making lunch...\nProgress: ");
            await GetApplication().MakeBreakfast((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task MakeBreakfast(
            Application.Application application)
        {
            System.Console.Write("Making breakfast...\nProgress: ");
            await application.MakeBreakfast((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task ExportData(
            Application.Application application)
        {
            System.Console.Write("Exporting data...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.ExportData((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task GenerateSQLScriptForTurningElevationAngles(
            Application.Application application)
        {
            System.Console.Write("Generating elevation angles script...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.GenerateSQLScriptForTurningElevationAngles((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task ExtractMeteorologicalStations(
            Application.Application application)
        {
            System.Console.Write("Extracting meteorological stations...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.ExtractMeteorologicalStations(dateTime, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task ExtractOceanographicalStations(
            Application.Application application)
        {
            System.Console.Write("Extracting oceanographical stations...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.ExtractOceanographicalStations(dateTime, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        // Helper
        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            //application.Initialize(_host, _port, _database, _schema, _user, _password);
            application.Initialize();
            return application;
        }
    }
}