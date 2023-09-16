using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using GalaSoft.MvvmLight;

namespace C2IEDM.ViewModel;

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