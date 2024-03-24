using StructureMap;
using Games.Pig.ViewModel;

namespace Games.Pig.UI.WPF
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
