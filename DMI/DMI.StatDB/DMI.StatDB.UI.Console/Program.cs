using CommandLine;
using DMI.StatDB.UI.Console.Verbs;
using StructureMap;
using System;

namespace DMI.StatDB.UI.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("DMI.StatDB.UI.Console");

            //System.Console.WriteLine("Counting Station records...");
            //System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllStations().Count}");

            //await MakeBreakfast(application);
            //await ExportData(application);

            await Parser.Default.ParseArguments<
                    Breakfast,
                    Export>(args)
                .MapResult(
                    (Breakfast options) => MakeBreakfast(),
                    (Export options) => Export(),
                    errs => Task.FromResult(0));

        }

        private static async Task MakeBreakfast()
        {
            System.Console.Write("Making breakfast...\nProgress: ");
            await GetApplication().MakeBreakfast((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task Export()
        {
            System.Console.Write("Exporting data...\nProgress: ");
            await GetApplication().ExportData((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static Application.Application GetApplication()
        {
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            //application.Initialize(_host, _port, _database, _schema, _user, _password);
            application.Initialize();
            return application;
        }
    }
}