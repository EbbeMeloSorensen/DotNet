using Craft.ViewModels.Graph;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab5
{
    public class Tab5ViewModel : ViewModelBase
    {
        public GraphViewModel GraphViewModel { get; set; }

        public Tab5ViewModel()
        {
            GraphViewModel = new GraphViewModel();
        }
    }
}
