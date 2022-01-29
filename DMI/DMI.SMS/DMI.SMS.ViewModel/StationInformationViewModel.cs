using System.Windows.Media;
using GalaSoft.MvvmLight;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.ViewModel
{
    public class StationInformationViewModel : ViewModelBase
    {
        private StationInformation _stationInformation;
        //private bool _isSelected;
        private Brush _brush;
        private Brush _backgroundBrush;
        private bool _warning1; // Warning that the record itself violates a number of business rules
        private bool _warning2; // Warning that the group of records with same object id violate a business rule
        private bool _warning3; // Warning that the group of records with the same station id violate a business rule

        public StationInformation StationInformation
        {
            get { return _stationInformation; }
            set
            {
                _stationInformation = value;
                RaisePropertyChanged();
            }
        }

        //public bool IsSelected
        //{
        //    get { return _isSelected; }
        //    set
        //    {
        //        _isSelected = value;
        //        RaisePropertyChanged();
        //    }
        //}

        public string StationName
        {
            get
            {
                return $"{_stationInformation.StationName}";
            }
        }

        public string StationID_DMI
        {
            get
            {
                return $"{_stationInformation.StationIDDMI}";
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

        public Brush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set
            {
                _backgroundBrush = value;
                RaisePropertyChanged();
            }
        }

        public bool Warning1
        {
            get { return _warning1; }
            set
            {
                _warning1 = value;
                RaisePropertyChanged();
            }
        }

        public bool Warning2
        {
            get { return _warning2; }
            set
            {
                _warning2 = value;
                RaisePropertyChanged();
            }
        }

        public bool Warning3
        {
            get { return _warning3; }
            set
            {
                _warning3 = value;
                RaisePropertyChanged();
            }
        }
    }
}
