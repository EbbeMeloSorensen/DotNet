using StructureMap;
//using System.Configuration;

namespace PR.UI.Console
{
    internal class InstanceScanner : Registry
    {
        public InstanceScanner()
        {
            // Kan ikke umiddelbartfå det til at virke... det lader det ellers til at gøre for WPF-applikationen
            //var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var settings = configFile.AppSettings.Settings;
            //var repositoryPluginAssembly = settings["RepositoryPluginAssembly"]?.Value;

            Scan(_ =>
            {
                //_.TheCallingAssembly(); // No the implementation of the interface is not in this assembly
                _.WithDefaultConventions(); // Default Convention is that the interface has the same name as the implementation except for the I prefix
                _.AssembliesAndExecutablesFromApplicationBaseDirectory(); // The implementation of the interface is in another assembly in the base directory
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("PR.Persistence"));
            });
        }
    }
}
