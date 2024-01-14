using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace WIGOS.Application
{
    public class ObservingFacilityEventArgs : EventArgs
    {
        public readonly ObservingFacility ObservingFacility;

        public ObservingFacilityEventArgs(
            ObservingFacility observingFacility)
        {
            ObservingFacility = observingFacility;
        }
    }
}