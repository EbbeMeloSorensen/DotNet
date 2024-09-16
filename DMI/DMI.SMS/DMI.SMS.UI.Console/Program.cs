using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using DMI.SMS.Domain.Entities;
using DMI.SMS.UI.Console.Verbs;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.String; size: 85MB")]
        static async Task Main(
            string[] args)
        {
            System.Console.WriteLine("DMI.SMS.UI.Console");

            // Override arguments
            //args = new [] {"export", "-f", "test.json"};
            args = new [] {"listStationInformations"};
            //args = new [] { "createStationInformation", "-i", "7913", "-n", "Bamse" };
            //args = new [] { "createStationInformation", "-i", "7914", "-n", "Kylling" };
            //args = new [] { "createSensorLocation", "-i", "7913" };
            //args = new [] {"listSensorLocations"};

            await Parser.Default.ParseArguments<
                    Lunch,
                    Export,
                    Import,
                    Verbs.StationInformation.List,
                    Verbs.StationInformation.Create,
                    Verbs.SensorLocation.List,
                    Verbs.SensorLocation.Create>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Export options) => Export(options),
                    (Import options) => Import(options),
                    (Verbs.StationInformation.List options) => ListStationInformations(options),
                    (Verbs.StationInformation.Create options) => CreateStationInformation(options),
                    (Verbs.SensorLocation.List options) => ListSensorLocations(options),
                    (Verbs.SensorLocation.Create options) => CreateSensorLocation(options),
                    errs => Task.FromResult(0));
        }

        private static async Task MakeLunch(
            Lunch options)
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

        private static async Task Export(
            Export options)
        {
            System.Console.Write("Exporting...\nProgress: ");

            await GetApplication().ExportData(
                options.FileName,
                options.ExcludeSupercededRows,
                (progress, nameOfSubtask) =>
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
            System.Console.Write("Import...\nProgress: ");

            await GetApplication().ImportData(options.FileName, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task ListStationInformations(
            Verbs.StationInformation.List options)
        {
            System.Console.Write("List...\nProgress: ");

            await GetApplication().ListStationInformations((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task CreateStationInformation(
            Verbs.StationInformation.Create options)
        {
            System.Console.Write("Creating Station Information...\nProgress: ");

            var stationInformation = new StationInformation
            {
                StationIDDMI = int.Parse(options.StationId),
                StationName = options.StationName
            };

            await GetApplication().CreateStationInformation(stationInformation, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task ListSensorLocations(
            Verbs.SensorLocation.List options)
        {
            System.Console.Write("List...\nProgress: ");

            await GetApplication().ListSensorLocations((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task CreateSensorLocation(
            Verbs.SensorLocation.Create options)
        {
            System.Console.Write("Creating Sensor Location...\nProgress: ");

            var sensorLocation = new SensorLocation
            {
                StationidDMI = 7913
            };

            await GetApplication().CreateSensorLocation(sensorLocation, (progress, nameOfSubtask) =>
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
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            return application;
        }
    }
}