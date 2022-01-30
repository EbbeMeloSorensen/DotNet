using StructureMap;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DMI.SMS command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();

            System.Console.WriteLine("Counting StationInformation records...");
            System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllStationInformations().Count}");

            System.Console.WriteLine("Exporting data...");
            application.UIDataProvider.ExportData(@"C:\Temp\Bamse.xml");
            System.Console.WriteLine("Done...");
        }
    }
}