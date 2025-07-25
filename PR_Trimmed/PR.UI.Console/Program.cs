﻿using System.Configuration;
using StructureMap;
using CommandLine;
using Craft.Utils;
using PR.Domain.Entities.PR;
using PR.Persistence;
using PR.Persistence.Versioned;
using PR.UI.Console.Verbs;
using Craft.Logging;

namespace PR.UI.Console
{
    class Program
    {
        public static async Task CreatePerson(
            Create options)
        {
            System.Console.Write("Creating Person...\nProgress: ");

            options.StartTime.TryParsingAsDateTime(out var startTime);
            options.EndTime.TryParsingAsDateTime(out var endTime);

            var person = new Person()
            {
                FirstName = options.FirstName,
                Start = startTime ?? DateTime.UtcNow.Date,
                End = endTime ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };

            var businessRuleViolations = await GetApplication().CreateNewPerson(person, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            if (businessRuleViolations.Any())
            {
                System.Console.WriteLine("\nErrors:");

                foreach (var kvp in businessRuleViolations)
                {
                    System.Console.WriteLine($"  {kvp.Value}");
                }

                //errors.ForEach(_ => System.Console.WriteLine($"  {_.Substring(_.IndexOf(":") + 2)}"));
            }
            else
            {
                System.Console.WriteLine("\nDone");
            }
        }

        public static async Task ListPeople(
            List options)
        {
            options.HistoricalTime.TryParsingAsDateTime(out var historicalTime);
            options.DatabaseTime.TryParsingAsDateTime(out var databaseTime);

            try
            {
                await GetApplication().ListPeople(
                    historicalTime,
                    databaseTime,
                    options.ExcludeCurrentObjects,
                    options.IncludeHistoricalObjects,
                    options.WriteToFile,
                    (progress, nameOfSubtask) =>
                    {
                        System.Console.SetCursorPosition(10, System.Console.CursorTop);
                        System.Console.Write($"{progress:F2} %");
                        return false;
                    });
            }
            catch (HttpRequestException exception)
            {
                System.Console.Write($"\nError occured: \"{exception.Message}\"\n");
            }
        }

        public static async Task GetPersonDetails(
            Details options)
        {
            System.Console.Write("Getting person details...\nProgress: ");

            var id = new Guid(options.ID);
            options.DatabaseTime.TryParsingAsDateTime(out var databaseTime);

            try
            {
                await GetApplication().GetPersonDetails(
                    id, 
                    databaseTime,
                    options.WriteToFile,
                    (progress, nameOfSubtask) =>
                {
                    System.Console.SetCursorPosition(10, System.Console.CursorTop);
                    System.Console.Write($"{progress:F2} %");
                    return false;
                });

                System.Console.WriteLine("\nDone");

            }
            catch (Exception e)
            {
                System.Console.WriteLine($"\nError getting person details: {e.Message}");
            }
        }

        public static async Task CountPeople(
            Count options)
        {
            System.Console.WriteLine("Coming soon: CountPeople");
            await Task.Delay(200);
        }

        public static async Task ExportPeople(
            Export options)
        {
            System.Console.Write("Exporting data...\nProgress: ");
            var dateTime = DateTime.Now;
            await GetApplication().ExportData(options.FileName, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task ImportPeople(
            Import options)
        {
            System.Console.Write("Importing data...\nProgress: ");
            await GetApplication().ImportData(
                options.FileName, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task UpdatePerson(
            Update options)
        {
            System.Console.Write("Updating Person...\nProgress: ");

            var person = new Person()
            {
                ID = new Guid(options.ID),
                FirstName = options.FirstName
            };

            await GetApplication().UpdatePerson(person, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task DeletePerson(
            Delete options)
        {
            System.Console.Write("Deleting...\nProgress: ");

            var id = new Guid(options.ID);

            await GetApplication().DeletePerson(id, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task MakeBreakfast(
            Breakfast options)
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

        static async Task Main(
            string[] args)
        {
            //args = "breakfast".Split();
            //args = "create --host localhost --user postgres --password L1on8Zebra --firstname Egon".Split();
            //args = "count --user john --password secret".Split();
            //args = "list --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "import --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "export --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "update --user john --password secret --id 67".Split();
            //args = "delete --user john --password secret --id 67".Split();
            //args = "list -h localhost -d PR -u postgres -p L1on8Zebra".Split();
            //args = "import --filename Contacts.json --legacy true --host localhost --user postgres --password L1on8Zebra".Split();

            await Parser.Default.ParseArguments<
                    Create,
                    Count,
                    List,
                    Details,
                    Export,
                    Import,
                    Update, 
                    Delete,
                    Breakfast>(args)
                .MapResult(
                    (Create options) => CreatePerson(options),
                    (Count options) => CountPeople(options),
                    (List options) => ListPeople(options),
                    (Details options) => GetPersonDetails(options),
                    (Export options) => ExportPeople(options),
                    (Import options) => ImportPeople(options),
                    (Update options) => UpdatePerson(options),
                    (Delete options) => DeletePerson(options),
                    (Breakfast options) => MakeBreakfast(options),
                    errs => Task.FromResult(0));
        }

        // Helpers
        private static Application.Application GetApplication()
        {
            // Denne blok bør du nok lave generel, så det er den samme, der bruges i WPF-applikationen

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var versioning = settings["Versioning"]?.Value;
            var reseeding = settings["Reseeding"]?.Value;

            if (versioning == "enabled")
            {
                // Den skal ikke wrappes, hvis det er en af dem, der repræsenterer et API
                if (application.UnitOfWorkFactory is not IUnitOfWorkFactoryVersioned)
                {
                    // Wrap the UnitOfWorkFactory, so we get versioning and history
                    application.UnitOfWorkFactory =
                        new UnitOfWorkFactoryFacade(application.UnitOfWorkFactory);
                }
            }
            else if (versioning != "disabled")
            {
                throw new ConfigurationException(
                    "Invalid value for versioning in config file (must be \"enabled\" or \"disabled\")");
            }

            application.UnitOfWorkFactory.Initialize(versioning == "enabled");

            if (reseeding == "enabled")
            {
                application.UnitOfWorkFactory.Reseed();
            }
            else if (reseeding != "disabled")
            {
                throw new ConfigurationException(
                    "Invalid value for reseeding in config file (must be \"enabled\" or \"disabled\")");
            }

            return application;
        }
    }
}