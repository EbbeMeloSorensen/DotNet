using System.Collections;
using CommandLine;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PR.Domain.Entities;

namespace PR.UI.Console
{
    class Program
    {
        [Verb("create", HelpText = "Create a new Person.")]
        public sealed class CreateOptions
        {
            [Option('f', "firstname", Required = true, HelpText = "First Name")]
            public string FirstName { get; set; }
        }

        [Verb("update", HelpText = "Update an existing person.")]
        public sealed class UpdateOptions
        {
            [Option('i', "id", Required = true, HelpText = "Person ID")]
            public string ID{ get; set; }

            [Option('f', "firstname", Required = false, HelpText = "First Name")]
            public string FirstName { get; set; }
        }

        [Verb("delete", HelpText = "Delete an existing person.")]
        public sealed class DeleteOptions
        {
            [Option('i', "id", Required = true, HelpText = "Person ID")]
            public string ID { get; set; }
        }

        public sealed class Options1
        {
            [Option('u', "username", Required = true, HelpText = "Your username")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "Your password")]
            public string Password { get; set; }
        }

        // Works fine but is not async
        //static int Main(string[] args)
        //{
        //    Parser.Default.ParseArguments<CreateOptions, UpdateOptions, DeleteOptions>(args)
        //        .WithParsed<CreateOptions>(options => { System.Console.WriteLine("Create called"); })
        //        .WithParsed<UpdateOptions>(options => { System.Console.WriteLine("Update called"); })
        //        .WithParsed<DeleteOptions>(options => { System.Console.WriteLine("Delete called"); })
        //        .WithNotParsed(errors => { System.Console.WriteLine("Invalid arguments"); });

        //    return 0;
        //}

        /* Alternativ måde at gøre det på ifølge manualen - det virker også
        static int Main(string[] args) =>
            Parser.Default.ParseArguments<CreateOptions, UpdateOptions, DeleteOptions>(args)
                .MapResult(
                    (CreateOptions options) => RunAddAndReturnExitCode(options),
                    (UpdateOptions options) => RunCommitAndReturnExitCode(options),
                    (DeleteOptions options) => RunCloneAndReturnExitCode(options),
                    errors => 1);

        private static int RunCloneAndReturnExitCode(DeleteOptions options)
        {
            System.Console.WriteLine("Satan");
            return 0;
        }

        private static int RunCommitAndReturnExitCode(object options)
        {
            System.Console.WriteLine("Helvede");
            return 0;
        }

        private static int RunAddAndReturnExitCode(object options)
        {
            System.Console.WriteLine("Fanden");
            return 0;
        }
        */

        /* Virker også, men hvordan opererer man med flere verbs
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CreateOptions>(args)
                .WithParsedAsync(RunAsync1);

            //await Parser.Default.ParseArguments<UpdateOptions>(args)
            //    .WithParsedAsync(RunAsync2);

            System.Console.WriteLine($"Exit code= {Environment.ExitCode}");
        }

        static async Task RunAsync1(CreateOptions options)
        {
            System.Console.WriteLine("Before Task in RunAsync1");
            await Task.Delay(20); //simulate async method 
            System.Console.WriteLine("After Task in RunAsync1");
        }

        static async Task RunAsync2(UpdateOptions options)
        {
            System.Console.WriteLine("Before Task in RunAsync2");
            await Task.Delay(20); //simulate async method 
            System.Console.WriteLine("After Task in RunAsync2");
        }
        */

        /* Det her virker, men hvordan igen . hvordan fanden opererer vi med flere verbs
        public static async Task Main(string[] args)
        {
            var retValue = await Parser.Default.ParseArguments<CreateOptions>(args)
                .MapResult(RunAndReturnExitCodeAsync1, _ => Task.FromResult(1));

            System.Console.WriteLine($"retValue= {retValue}");
        }
        static async Task<int> RunAndReturnExitCodeAsync1(CreateOptions options)
        {
            System.Console.WriteLine("Before Task (Create)");
            await Task.Delay(20); //simulate async method
            System.Console.WriteLine("After Task (Create)");
            return 0;
        }
        */

        /*
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<CreateOptions>(args)
                .MapResult(
                    options => RunAndReturnExitCode(options),
                    _ => 1);
        }

        static int RunAndReturnExitCode(CreateOptions options)
        {
            //options.Dump();
            return 0;
        }
        */

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options1>(args)
                .WithParsedAsync(async opts1 =>
                {
                    System.Console.WriteLine(opts1.Username);
                    DoSomeWork(opts1);

                    var container = Container.For<InstanceScanner>();
                    var application = container.GetInstance<Application.Application>();
                    application.Initialize();

                    await MakeBreakfast(application);

                    System.Console.WriteLine("Counting Person records...");
                    System.Console.WriteLine($"Station Count: {application.UIDataProvider.GetAllPeople().Count}");

                    //await ExportData(application);
                    //await CreatePerson(new Person
                    //{
                    //    FirstName = "Sofus"
                    //}, application);
                    //await ListPeople(application);
                });
        }

        private static void DoSomeWork(Options1 opts)
        {
            System.Console.WriteLine("Command Line parameters provided were valid :)");
        }

        private static void HandleParseError(IEnumerable errs)
        {
            System.Console.WriteLine("Command Line parameters provided were not valid!");
        }

        private static async Task MakeBreakfast(
            Application.Application application)
        {
            System.Console.Write("Making breakfast...\nProgress: ");
            await application.MakeBreakfast((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task MakeLunch()
        {
            System.Console.Write("Making lunch...");
            await Task.Delay(1000);
            System.Console.WriteLine("\nDone");
        }

        private static async Task CreatePerson(
            Person person,
            Application.Application application)
        {
            System.Console.Write("Creating Person...\nProgress: ");

            await application.CreatePerson(person, (progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }

        private static async Task ListPeople(
            Application.Application application)
        {
            await application.ListPeople((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
        }

        private static async Task ExportData(
            Application.Application application)
        {
            System.Console.Write("Exporting data...\nProgress: ");
            var dateTime = DateTime.Now;
            await application.ExportData((progress, nameOfSubtask) =>
            {
                System.Console.SetCursorPosition(10, System.Console.CursorTop);
                System.Console.Write($"{progress:F2} %");
                return false;
            });
            System.Console.WriteLine("\nDone");
        }
    }
}