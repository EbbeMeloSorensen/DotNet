using GalaSoft.MvvmLight;
using Craft.Algorithms.GuiTest2.Tab1;
using Craft.Algorithms.GuiTest2.Tab2;
using Craft.Algorithms.GuiTest2.Tab3;
using Craft.Algorithms.GuiTest2.Tab4;
using Craft.Algorithms.GuiTest2.Tab5;
using Craft.Algorithms.GuiTest2.Tab6;
using Craft.Algorithms.GuiTest2.Tab7;

namespace Craft.Algorithms.GuiTest2
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Tab1ViewModel Tab1ViewModel { get; }
        public Tab2ViewModel Tab2ViewModel { get; }
        public Tab3ViewModel Tab3ViewModel { get; }
        public Tab4ViewModel Tab4ViewModel { get; }
        public Tab5ViewModel Tab5ViewModel { get; }
        public Tab6ViewModel Tab6ViewModel { get; }
        public Tab7ViewModel Tab7ViewModel { get; }

        public MainWindowViewModel()
        {
            Tab1ViewModel = new Tab1ViewModel();
            Tab2ViewModel = new Tab2ViewModel(1200, 900);
            Tab3ViewModel = new Tab3ViewModel(1200, 900);
            Tab4ViewModel = new Tab4ViewModel(1200, 900);
            Tab5ViewModel = new Tab5ViewModel();
            Tab6ViewModel = new Tab6ViewModel();
            Tab7ViewModel = new Tab7ViewModel();
        }
    }
}
