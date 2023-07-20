using StructureMap;

namespace DMI.ObsDB.SimpleDataMigrator
{
    internal class InstanceScanner : Registry
    {
        public InstanceScanner()
        {
            Scan(_ =>
            {
                // No the implementation of the interface is not in this assembly
                //_.TheCallingAssembly();

                // The implementation of the interface is in another assembly in the base directory
                _.AssembliesFromApplicationBaseDirectory(
                    d => d.FullName.StartsWith("DMI.ObsDB.Persistence.EntityFrameworkCore"));

                _.AssembliesFromApplicationBaseDirectory(
                    d => d.FullName.StartsWith("Craft.Logging"));

                // Default Convention is that the interface has the same name as the implementation except for the I prefix
                _.WithDefaultConventions();
            });
        }
    }
}
