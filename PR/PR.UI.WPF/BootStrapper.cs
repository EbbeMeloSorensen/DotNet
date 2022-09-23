using System;
using System.Configuration;
using PR.ViewModel;
using StructureMap;

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
                    Console.WriteLine("Error reading app settings");
                    throw;
                }
            }
        }
    }
}
