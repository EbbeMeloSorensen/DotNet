using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Simulator.Laboratory.ViewModel.ShapeViewModels
{
    // Denne skal knytte sig til en Cyclic Bodystate
    public class WalkerViewModel : RectangleViewModel
    {
        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }
    }
}
