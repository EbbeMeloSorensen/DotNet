using StructureMap;

namespace HelloWorld
{
    public class InstanceScanner : Registry
    {
        public InstanceScanner()
        {
            Scan(_ =>
            {
                //_.TheCallingAssembly(); // No the implementation of the interface is not in this assembly
                _.AssembliesAndExecutablesFromApplicationBaseDirectory(); // The implementation of the interface is in another assembly in the base directory
                _.WithDefaultConventions(); // Default Convention is that the interface has the same name as the implementation except for the I prefix
            });
        }
    }

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