using StructureMap;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fun with a DD command line User interface");

            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<DD.Application.Application>();
            application.UIDataProvider.GetAllCreatureTypes().ToList().ForEach(c => Console.WriteLine(c.Name));
        }
    }
}