using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.Application;

public class ObservingFacilitiesEventArgs : EventArgs
{
    public readonly IEnumerable<ObservingFacility> ObservingFacilities;

    public ObservingFacilitiesEventArgs(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        ObservingFacilities = observingFacilities;
    }
}