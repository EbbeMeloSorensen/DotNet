using System.Configuration;
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
