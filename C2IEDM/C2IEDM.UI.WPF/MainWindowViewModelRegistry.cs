using StructureMap;

namespace C2IEDM.UI.WPF;

public class MainWindowViewModelRegistry : Registry
{
    public MainWindowViewModelRegistry()
    {
        Scan(_ =>
        {
            _.WithDefaultConventions();
            _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Craft.Logging"));
            _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Craft.UIElements"));
            _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("C2IEDM"));
            _.LookForRegistries();
        });
    }
}