using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Chronology
{
    public class TimeIntervalBarViewModel : ViewModelBase
    {
        private string _label;
        private double _leftOfLabel;
        private Brush _labelBrush;
        private double _leftOfBar;
        private double _top;
        private double _width;
        private double _height;
        private Brush _brush; // Todo: rename to barbrush

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged();
            }
        }

        public double LeftOfLabel
        {
            get { return _leftOfLabel; }
            set
            {
                _leftOfLabel = value;
                RaisePropertyChanged();
            }
        }

        public Brush LabelBrush
        {
            get { return _labelBrush; }
            set
            {
                _labelBrush = value;
                RaisePropertyChanged();
            }
        }

        public double LeftOfBar
        {
            get { return _leftOfBar; }
            set
            {
                _leftOfBar = value;
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
