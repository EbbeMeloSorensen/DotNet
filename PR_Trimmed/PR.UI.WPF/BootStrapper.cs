using System.Configuration;
using PR.Persistence.Versioned;
using StructureMap;
using PR.ViewModel;

namespace PR.UI.WPF
{
    public class BootStrapper
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                try
                {
                    var mainWindowViewModel = Container.For<MainWindowViewModelRegistry>().GetInstance<MainWindowViewModel>();

                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;
                    var versioning = settings["Versioning"]?.Value;

                    if (string.IsNullOrEmpty(versioning)) return mainWindowViewModel;

                    if (versioning == "enabled")
                    {
                        // Wrap the UnitOfWorkFactory, so we get versioning
                        mainWindowViewModel.UnitOfWorkFactory =
                            new UnitOfWorkFactoryFacade(mainWindowViewModel.UnitOfWorkFactory);
                    }
                    else if (versioning != "disabled")
                    {
                        throw new ConfigurationException(
                            "Invalid value for versioning in config file (must be \"enabled\" or \"disabled\")");
                    }

                    return mainWindowViewModel;
                }
                catch (ConfigurationErrorsException)
                {
                    System.Console.WriteLine("Error reading app settings");
                    throw;
                }
            }
        }
    }
}
