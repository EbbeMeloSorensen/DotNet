﻿using CommandLine;
using StructureMap;
using Glossary.Domain.Entities;
using Glossary.UI.Console.Verbs;

namespace Glossary.UI.Console
{
    class Program
    {
        private static string _host;
        private static string _port;
        private static string _database;
        private static string _schema;
        private static string _user;
        private static string _password;

        public static async Task CreateRecord(Create options)
        {
            System.Console.Write("Creating Record...\nProgress: ");

            var record = new Record()
            {
                Term = options.Term
            };

            _host = "localhost";
            _port = "5432";
            _database = "Glossary";
            _schema = "public";
            _user = options.User;
            _password = options.Password;

            await GetApplication().CreateRecord(record, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task ListRecords(List options)
        {
            _host = options.Host;
            _port = options.Port;
            _database = options.Database;
            _schema = options.Schema;
            _user = options.User;
            _password = options.Password;

            await GetApplication().ListRecords((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
        }

        public static async Task CountRecords(Count options)
        {
            System.Console.WriteLine("Coming soon: CountRecords");
            await Task.Delay(200);
        }

        public static async Task ExportRecords(Export options)
        {
            _host = options.Host;
            _port = options.Port;
            _database = options.Database;
            _schema = options.Schema;
            _user = options.User;
            _password = options.Password;

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

        public static async Task ImportRecords(Import options)
        {
            _host = options.Host;
            _port = options.Port;
            _database = options.Database;
            _schema = options.Schema;
            _user = options.User;
            _password = options.Password;

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

        public static async Task UpdateRecord(Update options)
        {
            System.Console.WriteLine("Coming soon: UpdateRecord");
            await Task.Delay(200);
        }

        public static async Task DeleteRecord(Delete options)
        {
            System.Console.WriteLine("Coming soon: DeleteRecord");
            await Task.Delay(200);
        }

        public static async Task MakeBreakfast(Breakfast options)
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

        static async Task Main(string[] args)
        {
            //args = "breakfast".Split();
            //args = "create --host localhost --user postgres --password L1on8Zebra --term Egon".Split();
            //args = "count --user john --password secret".Split();
            //args = "list --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "import --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "export --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "update --user john --password secret --id 67".Split();
            //args = "delete --user john --password secret --id 67".Split();
            //args = "list -h localhost -d PR -u postgres -p L1on8Zebra".Split();
            //args = "import --filename Contacts.json --host localhost --user postgres --password L1on8Zebra".Split();

            await Parser.Default.ParseArguments<
                    Create,
                    Count,
                    List,
                    Export,
                    Import,
                    Update, 
                    Delete,
                    Breakfast>(args)
                .MapResult(
                    (Create options) => CreateRecord(options),
                    (Count options) => CountRecords(options),
                    (List options) => ListRecords(options),
                    (Export options) => ExportRecords(options),
                    (Import options) => ImportRecords(options),
                    (Update options) => UpdateRecord(options),
                    (Delete options) => DeleteRecord(options),
                    (Breakfast options) => MakeBreakfast(options),
                    errs => Task.FromResult(0));
        }

        // Helper
        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.Initialize(_host, _port, _database, _schema, _user, _password);
            return application;
        }
    }
}