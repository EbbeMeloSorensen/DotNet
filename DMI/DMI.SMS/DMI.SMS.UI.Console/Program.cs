using StructureMap;

namespace DMI.SMS.UI.Console
{
    class Program
    {
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
            //await ExtractMeteorologicalStations(application);

            await ExportData(application);
        }

        private static async Task MakeBreakfast(
            Application.Application application)
        {
            System.Console.Write("Making breakfast...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.MakeBreakfast(dateTime, (progress, nameOfSubtask) =>
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
    }
}