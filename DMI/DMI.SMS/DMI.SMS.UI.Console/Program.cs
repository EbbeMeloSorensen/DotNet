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
            // Override arguments
            //args = new [] {"export", "-f", "test.json"};
            //args = new [] { "createStationInformation", "-i", "7913", "-n", "Bamse" };
            //args = new [] { "createStationInformation", "-i", "7914", "-n", "Kylling" };
            //args = new [] {"listStationInformations"};
            //args = new [] { "createSensorLocation", "-i", "7913" };
            //args = new [] { "createSensorLocation", "-i", "7914" };
            //args = new [] {"listSensorLocations"};
            //args = new [] { "createElevationAngles", "--sl", "c381610b-e488-4f3c-8cee-751ae0cb0b5b", "-n", "1", "--ne", "3" };
            //args = new [] {"listElevationAngles"};
            //args = new [] { "createServiceVisitReport", "--si", "40074ef7-77c6-4702-8009-9faeab7058d1" };

            await Parser.Default.ParseArguments<
                    Lunch,
                    Export,
                    Import,
                    Verbs.StationInformation.Create,
                    Verbs.StationInformation.List,
                    Verbs.SensorLocation.Create,
                    Verbs.SensorLocation.List,
                    Verbs.ElevationAngles.Create,
                    Verbs.ElevationAngles.List,
                    Verbs.ServiceVisitReport.Create,
                    Verbs.ContactPerson.List>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Export options) => Export(options),
                    (Import options) => Import(options),
                    (Verbs.StationInformation.Create options) => CreateStationInformation(options),
                    (Verbs.StationInformation.List options) => ListStationInformations(options),
                    (Verbs.SensorLocation.Create options) => CreateSensorLocation(options),
                    (Verbs.SensorLocation.List options) => ListSensorLocations(options),
                    (Verbs.ElevationAngles.Create options) => CreateElevationAngles(options),
                    (Verbs.ElevationAngles.List options) => ListElevationAngles(options),
                    (Verbs.ServiceVisitReport.Create options) => CreateServiceVisitReport(options),
                    (Verbs.ContactPerson.List options) => ListContactPersons(options),
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

        private static async Task CreateElevationAngles(
            Verbs.ElevationAngles.Create options)
        {
            System.Console.Write("Creating ElevationAngles...\nProgress: ");

            var elevationAngles = new ElevationAngles
            {
                GdbFromDate = DateTime.UtcNow,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59),
                ParentGuid = options.ParentGuid,
                DateFrom = new DateTime(2024, 9, 22, 1, 1, 1, DateTimeKind.Utc),
                Angle_N = options.Angle_N,
                Angle_NE = options.Angle_NE,
                Angle_E = 30,
                Angle_SE = 40,
                Angle_S = 50,
                Angle_SW = 60,
                Angle_W = 70,
                Angle_NW = 80,
                AngleIndex = 52,
                AngleComment = "No comment"
            };

            await GetApplication().CreateElevationAngles(elevationAngles, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task ListElevationAngles(
            Verbs.ElevationAngles.List options)
        {
            System.Console.Write("List elevation angles...\nProgress: ");

            await GetApplication().ListElevationAngles((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task CreateServiceVisitReport(
            Verbs.ServiceVisitReport.Create options)
        {
            System.Console.Write("Creating Service Visit Report...\nProgress: ");

            var serviceVisitReport = new ServiceVisitReport
            {
                GdbFromDate = DateTime.UtcNow,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59),
                ParentGuid = options.ParentGuid
            };

            await GetApplication().CreateServiceVisitReport(serviceVisitReport, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task ListContactPersons(
            Verbs.ContactPerson.List options)
        {
            System.Console.Write("List contact persons...\nProgress: ");

            await GetApplication().ListContactPersons((progress, nameOfSubtask) =>
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