using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace WIGOS.Application
{
    public class ObservingFacilitiesEventArgs : EventArgs
    {
        public readonly IEnumerable<ObservingFacility> ObservingFacilities;

        public ObservingFacilitiesEventArgs(
            IEnumerable<ObservingFacility> observingFacilities)
        {
            ObservingFacilities = observingFacilities;
        }
    }
}