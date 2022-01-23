using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Math;

namespace Craft.ViewModels.Geometry2D
{
    public class PointViewModel : ViewModelBase
    {
        private Point2D _point;
        private double _diameter;
        private Brush _brush;

        public Point2D Point
        {
            get => _point;
            set
            {
                _point = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get => _diameter;
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public PointViewModel(
            Point2D point,
            double diameter,
            Brush brush)
        {
            Point = point;
            Diameter = diameter;
            Brush = brush;
        }
    }
}
