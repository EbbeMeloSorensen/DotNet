using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using DMI.SMS.UI.Console.Verbs;
using Container = StructureMap.Container;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        // Disse bruger vi i PR.UI.Console, men ikke umiddelbart her
        //private static string _host;
        //private static string _port;
        //private static string _database;
        //private static string _schema;
        //private static string _user;
        //private static string _password;

        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.String; size: 85MB")]
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("DMI.SMS.UI.Console");

            // Works
            //await ExtractMeteorologicalStations(application);
            //await ExtractOceanographicalStations(application);
            //await GenerateSQLScriptForTurningElevationAngles(application);

            //args = new string[3] {"export", "-f", "test.json"};


            await Parser.Default.ParseArguments<
                    Lunch,
                    Export,
                    Script,
                    Extract>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Export options) => Export(options),
                    (Script options) => Script(options),
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

        private static async Task Export(Export options)
        {
            System.Console.Write("Exporting...\nProgress: ");

            await GetApplication().ExportData(options.FileName, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });

            System.Console.WriteLine("\nDone");
        }

        private static async Task Script(Script options)
        {
            System.Console.Write("Generating SQL script...\nProgress: ");
            await GetApplication().GenerateSQLScriptForAddingWigosIDs((progress, nameOfSubtask) =>
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

        // Helper
        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            //application.Initialize(_host, _port, _database, _schema, _user, _password);
            application.Initialize();
            return application;
        }
    }
}