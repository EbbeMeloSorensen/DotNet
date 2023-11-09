using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Simulator.Laboratory.ViewModel.ShapeViewModels
{
    public class TaggedEllipseViewModel : EllipseViewModel
    {
        private string _tag;

        public string Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                RaisePropertyChanged();
            }
        }
    }
}
