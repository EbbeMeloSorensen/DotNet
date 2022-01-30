using DD.Application;
using StructureMap;

namespace DD.UI.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DD command line User interface");

            System.Console.WriteLine("Creature Types in repository:");
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.UIDataProvider.GetAllCreatureTypes()
                .ToList()
                .ForEach(c => System.Console.WriteLine($"  {c.Name}"));

            application.Engine.Scene = SceneGenerator.GenerateScene(2);
            System.Console.WriteLine($"Setting up scene: \"{application.Engine.Scene.Name}\"");
            System.Console.WriteLine("Starting battle..");

            await Task.Run(async () =>
            {
                await application.ActOutBattle();
            });

            System.Console.WriteLine($"Battle decided. {application.Engine.Creatures.Count} creatures survived");
            System.Console.WriteLine("Done");
        }
    }
}