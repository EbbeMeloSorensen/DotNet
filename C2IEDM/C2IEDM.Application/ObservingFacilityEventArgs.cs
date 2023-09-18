using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.Application;

public class ObservingFacilityEventArgs : EventArgs
{
    public readonly ObservingFacility ObservingFacility;

    public ObservingFacilityEventArgs(
        ObservingFacility observingFacility)
    {
        ObservingFacility = observingFacility;
    }
}