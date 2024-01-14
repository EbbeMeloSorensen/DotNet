using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using GalaSoft.MvvmLight;

namespace WIGOS.ViewModel
{
    public class ObservingFacilityListItemViewModel : ViewModelBase
    {
        private ObservingFacility _observingFacility;

        public ObservingFacility ObservingFacility
        {
            get { return _observingFacility; }
            set
            {
                _observingFacility = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayText => _observingFacility.Name;
    }
}