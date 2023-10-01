using System.Collections.Generic;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.ViewModel;

public class ObservingFacilityDataExtract
{
    public ObservingFacility ObservingFacility { get; set; }

    public List<GeospatialLocation> GeospatialLocations { get; set; }
}