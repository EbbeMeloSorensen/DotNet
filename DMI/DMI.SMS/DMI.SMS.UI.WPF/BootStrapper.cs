using System;
using System.Configuration;
using StructureMap;
using DMI.SMS.ViewModel;

namespace DMI.SMS.UI.WPF
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
