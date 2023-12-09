using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;
using System.Windows;
using System;

namespace C2IEDM.ViewModel
{
    public class GeospatialLocationsViewModel : ViewModelBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;

        private ObjectCollection<ObservingFacility> _observingFacilities;
        private ObservingFacility _activeObservingFacility;
        private ObservableCollection<GeospatialLocationViewModel> _geospatialLocationViewModels;
        private RelayCommand<object> _createGeospatialLocationCommand;

        public ObservableCollection<GeospatialLocationViewModel> GeospatialLocationViewModels
        {
            get { return _geospatialLocationViewModels; }
            set
            {
                _geospatialLocationViewModels = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> CreateGeospatialLocationCommand
        {
            get
            {
                return _createGeospatialLocationCommand ?? (
                    _createGeospatialLocationCommand = new RelayCommand<object>(CreateGeospatialLocation, CanCreateGeospatialLocation));
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

        private void CreateGeospatialLocation(object owner)
        {
            var dialogViewModel = new DefineGeospatialLocationDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            throw new NotImplementedException();

            /*
            if (_activePerson != null)
            {
                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.PersonAssociations.Add(new PersonAssociation
                    {
                        SubjectPersonId = dialogViewModel.SubjectPerson.Id,
                        ObjectPersonId = dialogViewModel.ObjectPerson.Id,
                        Description = dialogViewModel.Description,
                        Created = DateTime.UtcNow
                    });

                    unitOfWork.Complete();
                }

                Populate();
            }
            */
        }

        private bool CanCreateGeospatialLocation(object owner)
        {
            return true;
        }
    }
}
