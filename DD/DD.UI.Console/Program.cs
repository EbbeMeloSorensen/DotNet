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
            application.Engine = new SimpleEngine(null);
            application.Engine.Randomize();
            application.Engine.Scene = SceneGenerator.GenerateScene(2);

            System.Console.WriteLine($"Setting up scene: \"{application.Engine.Scene.Name}\"");
            System.Console.WriteLine("Starting battle..");

            await Task.Run(async () =>
            {
                await application.ActOutBattle();
            });

            var message = $"Battle decided. {application.Engine.Creatures.Count}";
            message += $" {application.Engine.Creatures.First().CreatureType.Name}";
            if (application.Engine.Creatures.Count > 1)
            {
                message += "s";
            }
            message += " survived";

            System.Console.WriteLine(message);
            System.Console.WriteLine("Done");
        }
    }
}