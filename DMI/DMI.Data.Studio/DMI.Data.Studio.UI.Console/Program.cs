﻿using CommandLine;
using DMI.Data.Studio.UI.Console.Verbs;

namespace DMI.Data.Studio.UI.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("DMI.Data.Studio.UI.Console");

            await Parser.Default.ParseArguments<
                    Die,
                    Lunch,
                    Extract,
                    Intervals,
                    Stations>(args)
                .MapResult(
                    (Die options) => RollADie(options),
                    (Lunch options) => MakeLunch(options),
                    (Extract options) => Extract(options),
                    (Intervals options) => ExtractObservationIntervals(options),
                    (Stations options) => ExtractStations(options),
                    errs => Task.FromResult(0));
        }

        private static async Task RollADie(
            Die options)
        {
            System.Console.Write("Rolling a die...\nProgress: ");
            var result = await GetApplication().RollADie((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
            System.Console.WriteLine($"\nResult: {result}");
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

        private static async Task Extract(
            Extract options)
        {
            var dateTime = new DateTime(2021, 5, 1);

            switch (options.Category)
            {
                case "m":
                    {
                        System.Console.Write("Extracting meteorological stations...\nProgress: ");
                        await GetApplication().ExtractMeteorologicalStations(dateTime, (progress, nameOfSubtask) =>
                        {
                            System.Console.SetCursorPosition(10, System.Console.CursorTop);
                            System.Console.Write($"{progress:F2} %");
                            return false;
                        });

                        break;
                    }
                case "o":
                    {
                        System.Console.Write("Extracting oceanographical stations...\nProgress: ");
                        await GetApplication().ExtractOceanographicalStations(dateTime, (progress, nameOfSubtask) =>
                        {
                            System.Console.SetCursorPosition(10, System.Console.CursorTop);
                            System.Console.Write($"{progress:F2} %");
                            return false;
                        });

                        break;
                    }
                default:
                    {
                        System.Console.Write("Invalid argument. Please choose m or o");
                        break;
                    }
            }
        }

        private static async Task ExtractObservationIntervals(
            Intervals options)
        {
            var stationIDs = options.StatID.Split(',').ToList();

            foreach (var stationID in stationIDs)
            {
                System.Console.Write($"Extracting observation intervals for station {stationID}...\nProgress: ");

                var result = await GetApplication().ExtractObservationIntervals(
                    stationID,
                    options.Parameter,
                    int.Parse(options.Tolerance),
                    int.Parse(options.FirstYear),
                    int.Parse(options.LastYear),
                    false,
                    (progress, nameOfSubtask) =>
                {
                    System.Console.SetCursorPosition(10, System.Console.CursorTop);
                    System.Console.Write($"{progress:F2} %");
                    return false;
                });

                System.Console.WriteLine("\nDone");
                System.Console.WriteLine($"\nOperational intervals: {result.Count}");
            }
        }

        private static async Task ExtractStations(
            Stations options)
        {
            System.Console.Write("Extracting stations...\nProgress: ");

            var result = await GetApplication().ExtractStations(
                (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });


            System.Console.WriteLine($"\n{result.Count} Stations:");

            foreach (var stationID in result)
            {
                System.Console.WriteLine($"  {stationID}");
            }
        }

        private static Application.Application GetApplication()
        {
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            return application;
        }
    }
}