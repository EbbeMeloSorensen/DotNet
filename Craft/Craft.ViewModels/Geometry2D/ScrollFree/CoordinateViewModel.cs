using Craft.Utils;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateViewModel : ViewModelBase
    {
        private double _coordinate;

        public double Coordinate
        {
            get => _coordinate;
            set
            {
                _coordinate = value;
                RaisePropertyChanged();
            } 
        }
    }
}
