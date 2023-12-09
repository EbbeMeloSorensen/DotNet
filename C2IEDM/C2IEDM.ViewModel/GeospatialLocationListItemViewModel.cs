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
            get { return _geospatialLocation; }
            set
            {
                _geospatialLocation = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayText
        {
            get
            {
                var point = _geospatialLocation as Point;

                return $"({point.Coordinate1}, {point.Coordinate2}) from {point.From.AsDateString()}";
            }
        }
    }
}
