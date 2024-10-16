using CommandLine;
using StructureMap;
using PR.Domain.Entities;
using PR.Persistence.Versioned;
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
            await GetApplication().ExportData(options.FileName, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        public static async Task ImportPeople(Import options)
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
                    Export,
                    Import,
                    Update, 
                    Delete,
                    Breakfast>(args)
                .MapResult(
                    (Create options) => CreatePerson(options),
                    (Count options) => CountPeople(options),
                    (List options) => ListPeople(options),
                    (Export options) => ExportPeople(options),
                    (Import options) => ImportPeople(options),
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

            var versioned = false;
            application.UnitOfWorkFactory.Initialize(versioned);
            application.UnitOfWorkFactory.Reseed();

            if (versioned)
            {
                application.UnitOfWorkFactory = new UnitOfWorkFactoryFacade(application.UnitOfWorkFactory);
            }

            return application;
        }
    }
}