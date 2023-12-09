using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities;
using System.Linq;

namespace C2IEDM.ViewModel
{
    public class GeospatialLocationsViewModel : ViewModelBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;

        private ObjectCollection<ObservingFacility> _observingFacilities;
        private ObservingFacility _activeObservingFacility;
        private ObservableCollection<GeospatialLocationViewModel> _geospatialLocationViewModels;

        public ObservableCollection<GeospatialLocationViewModel> GeospatialLocationViewModels
        {
            get { return _geospatialLocationViewModels; }
            set
            {
                _geospatialLocationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public GeospatialLocationsViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService,
            ObjectCollection<ObservingFacility> observingFacilities)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _observingFacilities = observingFacilities;

            _observingFacilities.PropertyChanged += Initialize;
        }

        private void Initialize(object sender, PropertyChangedEventArgs e)
        {
            var temp = sender as ObjectCollection<ObservingFacility>;

            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                _activeObservingFacility = temp.Objects.Single();
                Populate();
                //IsVisible = true;
            }
            else
            {
                _activeObservingFacility = null;
                //IsVisible = false;
            }
        }

        private void Populate()
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacility =
                    unitOfWork.ObservingFacilities.GetIncludingGeospatialLocations(_activeObservingFacility.Id);

                GeospatialLocationViewModels = new ObservableCollection<GeospatialLocationViewModel>(
                    observingFacility.Item2.Select(_ => new GeospatialLocationViewModel{ GeospatialLocation = _}));
            }
        }
    }
}
