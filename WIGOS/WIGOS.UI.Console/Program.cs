using CommandLine;
using StructureMap;
using WIGOS.UI.Console.Verbs;

namespace WIGOS.UI.Console
{
    internal class Program
    {
        static async Task Main(
            string[] args)
        {
            await Parser.Default.ParseArguments<
                    List,
                    Breakfast>(args)
                .MapResult(
                    (List options) => ListLocations(options),
                    (Breakfast options) => MakeBreakfast(options),
                    errs => Task.FromResult(0));
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

        public static async Task ListLocations(
            List options)
        {
            switch (options.Entity)
            {
                case "Location":
                    {
                        await GetApplication().ListLocations((progress, nameOfSubtask) =>
                        {
                            System.Console.SetCursorPosition(10, System.Console.CursorTop);
                            System.Console.Write($"{progress:F2} %");
                            return false;
                        });

                        break;
                    }
                case "ObservingFacility":
                    {
                        await GetApplication().ListObservingFacilities((progress, nameOfSubtask) =>
                        {
                            System.Console.SetCursorPosition(10, System.Console.CursorTop);
                            System.Console.Write($"{progress:F2} %");
                            return false;
                        });

                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("The entity parameter passed to the program was invalid");
                    }
            }
        }

        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            return application;
        }
    }
}