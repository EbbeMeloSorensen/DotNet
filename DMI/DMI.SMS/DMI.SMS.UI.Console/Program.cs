using StructureMap;
//using DMI.SMS.Persistence.Npgsql;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DMI.SMS command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();

            //var host = "172.25.7.23";
            //var port = 5432;
            //var schema = "sde";
            //var database = "sms_prod";
            //var user = "ebs";
            //var password = "Vm6PAkPh";
            //ConnectionStringProvider.Initialize(host, port, database, schema, user, password);

            System.Console.WriteLine("Counting StationInformation records...");
            System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllStationInformations().Count}");

            // Works
            //System.Console.WriteLine("Exporting data...");
            //application.UIDataProvider.ExportData(".//Kylling.xml");
            //System.Console.WriteLine("Done...");

            await MakeBreakfast(application);
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
    }
}