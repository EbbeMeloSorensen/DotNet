using Craft.Math;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Geometry2D
{
    public abstract class ShapeViewModel : ViewModelBase
    {
        private Point2D _point;
        private double _width;
        private double _height;

        public Point2D Point
        {
            get => _point;
            set
            {
                _point = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }
    }                                     
}
