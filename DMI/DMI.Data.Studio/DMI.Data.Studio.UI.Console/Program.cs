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
                    Lunch,
                    Extract>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Extract options) => Extract(options),
                    errs => Task.FromResult(0));
        }

        private static async Task MakeLunch(Lunch options)
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

        private static async Task Extract(Extract options)
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

        private static Application.Application GetApplication()
        {
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.Initialize();
            return application;
        }
    }
}