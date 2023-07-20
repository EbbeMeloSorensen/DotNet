using System;
using CommandLine;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.SimpleDataMigrator.Verbs;

namespace DMI.ObsDB.SimpleDataMigrator
{
    // Limit på 1000 stationer og data fra 1980 til 2000 giver en database på 477928 KB dvs knap en halv GB
    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<
                    Breakfast,
                    Migrate>(args)
                .MapResult(
                    (Breakfast options) => MakeBreakfast(options),
                    (Migrate options) => Migrate(options),
                    errs => Task.FromResult(0));
        }

        private static async Task MakeBreakfast(Breakfast options)
        {
            Console.Write("Making breakfast...\nProgress: ");
            await GetApplication().MakeBreakfast((progress, nameOfSubtask) =>
            {
                Console.SetCursorPosition(10, Console.CursorTop);
                Console.Write($"{progress:F2} %");
                return false;
            });
            Console.WriteLine("\nDone");
        }

        private static async Task Migrate(Migrate options)
        {
            Console.Write("Migrating...\nProgress: 0.00 %");

            await GetApplication().Migrate(
                options.StationLimit,
                options.FirstYear,
                options.LastYear,
                (progress, nameOfSubtask) =>
            {
                Console.SetCursorPosition(10, Console.CursorTop);
                Console.Write($"{progress:F2} %");
                return false;
            });

            Console.WriteLine("\nDone");
        }

        private static Application.Application GetApplication()
        {
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            //application.Initialize(_host, _port, _database, _schema, _user, _password);
            //application.Initialize();
            return application;
        }
    }
}