using GalaSoft.MvvmLight;
using PR.ViewModel.GIS.Domain;

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