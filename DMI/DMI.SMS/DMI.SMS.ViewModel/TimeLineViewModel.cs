using GalaSoft.MvvmLight;

namespace DMI.SMS.ViewModel
{
    public class TimeLineViewModel : ViewModelBase
    {
        private double _x;
        private double _height;
        private string _header;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
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

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }
    }
}