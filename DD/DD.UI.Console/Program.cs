using StructureMap;
using DD.Application;

namespace DD.UI.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Console.WriteLine("Fun with a DD command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            application.UIDataProvider.GetAllCreatureTypes()
                .ToList()
                .ForEach(c => System.Console.WriteLine(c.Name));

            var scene = SceneGenerator.GenerateScene(2);
            System.Console.WriteLine("Done");
        }
    }
}