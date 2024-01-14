using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using System.Collections.Generic;

namespace WIGOS.ViewModel
{
    public class ObservingFacilityDataExtract
    {
        public ObservingFacility ObservingFacility { get; set; }

        public List<GeospatialLocation> GeospatialLocations { get; set; }
    }
}