using StructureMap;

namespace DMI.StatDB.UI.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DMI.StatDB command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.Initialize();

            System.Console.WriteLine("Counting Station records...");
            System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllStations().Count}");

            await MakeBreakfast(application);
        }

        private static async Task MakeBreakfast(
            Application.Application application)
        {
            System.Console.Write("Making breakfast...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.MakeBreakfast((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }
    }
}