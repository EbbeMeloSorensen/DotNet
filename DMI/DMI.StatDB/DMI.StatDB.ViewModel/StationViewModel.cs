using GalaSoft.MvvmLight;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.ViewModel
{
    public class StationViewModel : ViewModelBase
    {
        private Station _station;
        private bool _isSelected;

        public Station Station
        {
            get { return _station; }
            set
            {
                _station = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayText
        {
            get
            {
                return $"{_station.StatID}";
            }
        }
    }
}
