using System.Collections.Generic;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;

namespace PR.ViewModel.GIS
{
    public class ObservingFacilityDataExtract
    {
        public ObservingFacility ObservingFacility { get; set; }

        public List<GeospatialLocation> GeospatialLocations { get; set; }
    }
}