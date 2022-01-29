using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace DMI.SMS.ViewModel
{
    public class TimeIntervalBarViewModel : ViewModelBase
    {
        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private Brush _brush;

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
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
    }
}
