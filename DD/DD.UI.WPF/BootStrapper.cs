using StructureMap;
using DD.ViewModel;

namespace DD.UI.WPF
{
    public class BootStrapper
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return Container.For<MainWindowViewModelRegistry>().GetInstance<MainWindowViewModel>();
            }
        }
    }
}
