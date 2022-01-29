using GalaSoft.MvvmLight;

namespace DD.ViewModel
{
    public class HighlightedSquareViewModel : ViewModelBase
    {
        private double _left;
        private double _top;
        private double _diameter;

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

        public double Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public HighlightedSquareViewModel(
            int positionX,
            int positionY)
        {
            Diameter = BoardViewModel.SquareLength - 3;
            Left = positionX * BoardViewModel.SquareLength;
            Top = positionY * BoardViewModel.SquareLength;
        }
    }
}
