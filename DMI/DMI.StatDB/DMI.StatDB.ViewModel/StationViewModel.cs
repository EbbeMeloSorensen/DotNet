using GalaSoft.MvvmLight;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.ViewModel
{
    public class StationViewModel : ViewModelBase
    {
        private Station _station;

        public Station Station
        {
            get { return _station; }
            set
            {
                _station = value;
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
