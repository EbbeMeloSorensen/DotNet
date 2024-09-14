using System.Diagnostics.CodeAnalysis;
using CommandLine;
using DMI.SMS.UI.Console.Verbs;

namespace DMI.SMS.UI.Console
{
    class Program
    {
        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: System.String; size: 85MB")]
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("DMI.SMS.UI.Console");

            // Override arguments
            //args = new string[3] {"export", "-f", "test.json"};

            await Parser.Default.ParseArguments<
                    Lunch,
                    Export,
                    Import>(args)
                .MapResult(
                    (Lunch options) => MakeLunch(options),
                    (Export options) => Export(options),
                    (Import options) => Import(options),
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

        private static async Task Import(Import options)
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

        // Helper
        private static Application.Application GetApplication()
        {
            var container = StructureMap.Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            return application;
        }
    }
}