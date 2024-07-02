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

            await Parser.Default.ParseArguments<
                    Breakfast,
                    Import,
                    Export>(args)
                .MapResult(
                    (Breakfast options) => MakeBreakfast(),
                    (Import options) => Import(options),
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

        private static async Task Import(
            Import options)
        {
            System.Console.Write("Importing data...\nProgress: ");
            await GetApplication().ImportData(options.FileName, (progress, nameOfSubtask) =>
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