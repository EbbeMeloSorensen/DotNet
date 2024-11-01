using GalaSoft.MvvmLight;
using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace PR.ViewModel.GIS
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