using System;
using System.Globalization;
using GalaSoft.MvvmLight;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using Craft.Utils;

namespace C2IEDM.ViewModel
{
    public class GeospatialLocationListItemViewModel : ViewModelBase
    {
        private GeospatialLocation _geospatialLocation;

        public GeospatialLocation GeospatialLocation
        {
            get => _geospatialLocation;
            set
            {
                _geospatialLocation = value;
                RaisePropertyChanged();
            }
        }

        public string Latitude { get; }
        public string Longitude { get; }
        public string From { get; }
        public string To { get; }

        public GeospatialLocationListItemViewModel(
            GeospatialLocation geospatialLocation)
        {
            GeospatialLocation = geospatialLocation;
            Latitude = (geospatialLocation as Point).Coordinate1.ToString(CultureInfo.InvariantCulture);
            Longitude = (geospatialLocation as Point).Coordinate2.ToString(CultureInfo.InvariantCulture);
            From = geospatialLocation.From.AsDateString();
            To = geospatialLocation.To == DateTime.MaxValue ? "" : geospatialLocation.To.AsDateString();
        }
    }
}
