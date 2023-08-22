using StructureMap;

namespace C2IEDM.UI.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var application = GetApplication();

            await application.ListPeople();
        }

        private static Application.Application GetApplication()
        {
            var container = Container.For<InstanceScanner>();
            var application = container.GetInstance<Application.Application>();
            return application;
        }
    }
}