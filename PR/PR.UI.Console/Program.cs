using CommandLine;
using StructureMap;
using PR.Domain.Entities;
using PR.UI.Console.Verbs;

namespace PR.UI.Console
{
    class Program
    {
        public static async Task CreatePerson(Create options)
        {
            System.Console.Write("Creating Person...\nProgress: ");

            var person = new Person()
            {
                FirstName = options.FirstName
            };

            await GetApplication().CreatePerson(person, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");

            //System.Console.WriteLine("Counting Person records...");
            //System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllPeople().Count}");

            //await ExportData(application);
            //await CreatePerson(new Person
            //{
            //    FirstName = "Sofus"
            //}, application);
            //await ListPeople(application);
        }

        public static async Task ListPeople(List options)
        {
            await GetApplication().ListPeople((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
        }

        public static async Task CountPeople(Count options)
        {
            System.Console.WriteLine("Coming soon: CountPeople");
            await Task.Delay(200);
        }

        public static async Task ExportPeople(Export options)
        {
            System.Console.Write("Exporting data...\nProgress: ");
            var dateTime = DateTime.Now;
            await GetApplication().ExportData((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task UpdatePerson(Update options)
        {
            System.Console.WriteLine("Coming soon: UpdatePerson");
            await Task.Delay(200);
        }

        public static async Task DeletePerson(Delete options)
        {
            System.Console.WriteLine("Coming soon: DeletePerson");
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
            //args = "create --user john --password secret --firstname Heidi".Split();
            //args = "count --user john --password secret".Split();
            args = "list --user john --password secret".Split();
            //args = "export --user john --password secret".Split();
            //args = "update --user john --password secret --id 67".Split();
            //args = "delete --user john --password secret --id 67".Split();

            await Parser.Default.ParseArguments<
                    Create,
                    Count,
                    List,
                    Export,
                    Update, 
                    Delete,
                    Breakfast>(args)
                .MapResult(
                    (Create options) => CreatePerson(options),
                    (Count options) => CountPeople(options),
                    (List options) => ListPeople(options),
                    (Export options) => ExportPeople(options),
                    (Update options) => UpdatePerson(options),
                    (Delete options) => DeletePerson(options),
                    (Breakfast options) => MakeBreakfast(options),
                    errs => Task.FromResult(0));
        }

        // Helper
        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.Initialize();
            return application;
        }
    }
}