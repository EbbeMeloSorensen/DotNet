using StructureMap;

namespace DMI.StatDB.UI.WPF
{
    public class MainWindowViewModelRegistry : Registry
    {
        public MainWindowViewModelRegistry()
        {
            Scan(_ =>
            {
                _.WithDefaultConventions();
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Craft.UIElements"));
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("DMI.StatDB"));
                _.LookForRegistries();
            });
        }
    }
}
