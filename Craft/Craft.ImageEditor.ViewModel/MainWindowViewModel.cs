using GalaSoft.MvvmLight;

namespace Craft.ImageEditor.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public MainWindowViewModel(
            ImageEditorViewModel imageEditorViewModel)
        {
            ImageEditorViewModel = imageEditorViewModel;

            ImageEditorViewModel.ImageWidth = 1200;
            ImageEditorViewModel.ImageHeight = 900;
        }
    }
}
