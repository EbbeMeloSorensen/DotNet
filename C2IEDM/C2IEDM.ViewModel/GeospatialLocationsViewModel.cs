using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Persistence;
using Point = C2IEDM.Domain.Entities.WIGOS.GeospatialLocations.Point;
using C2IEDM.Domain.Entities.Geometry.CoordinateSystems;

namespace C2IEDM.ViewModel
{
    public class GeospatialLocationsViewModel : ViewModelBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;

        private ObjectCollection<ObservingFacility> _observingFacilities;
        private ObservingFacility _selectedObservingFacility;
        private ObservableCollection<GeospatialLocationListItemViewModel> _geospatialLocationListItemViewModels;
        private RelayCommand _deleteSelectedGeospatialLocationsCommand;
        private RelayCommand<object> _createGeospatialLocationCommand;
        private RelayCommand<object> _updateSelectedGeospatialLocationCommand;

        public event EventHandler<CommandInvokedEventArgs> NewGeospatialLocationCalledByUser;

        public ObjectCollection<GeospatialLocation> SelectedGeospatialLocations { get; private set; }

        public ObservableCollection<GeospatialLocationListItemViewModel> GeospatialLocationListItemViewModels
        {
            get { return _geospatialLocationListItemViewModels; }
            set
            {
                _geospatialLocationListItemViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<GeospatialLocationListItemViewModel> SelectedGeospatialLocationListItemViewModels { get; set; }

        public RelayCommand DeleteSelectedGeospatialLocationsCommand
        {
            get
            {
                return _deleteSelectedGeospatialLocationsCommand ?? (
                    _deleteSelectedGeospatialLocationsCommand = new RelayCommand(DeleteSelectedGeospatialLocations, CanDeleteSelectedGeospatialLocations));
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

        public RelayCommand<object> UpdateSelectedGeospatialLocationCommand
        {
            get
            {
                return _updateSelectedGeospatialLocationCommand ?? (
                    _updateSelectedGeospatialLocationCommand = new RelayCommand<object>(UpdateSelectedGeospatialLocation, CanUpdateSelectedGeospatialLocation));
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

            SelectedGeospatialLocations = new ObjectCollection<GeospatialLocation>
            {
                Objects = new List<GeospatialLocation>()
            };

            SelectedGeospatialLocationListItemViewModels =
                new ObservableCollection<GeospatialLocationListItemViewModel>();

            SelectedGeospatialLocationListItemViewModels.CollectionChanged += (s, e) =>
            {
                SelectedGeospatialLocations.Objects = SelectedGeospatialLocationListItemViewModels.Select(_ => _.GeospatialLocation);
                DeleteSelectedGeospatialLocationsCommand.RaiseCanExecuteChanged();
                UpdateSelectedGeospatialLocationCommand.RaiseCanExecuteChanged();
            };

            _observingFacilities.PropertyChanged += Initialize;
        }

        private void Initialize(object sender, PropertyChangedEventArgs e)
        {
            var temp = sender as ObjectCollection<ObservingFacility>;

            if (temp != null && temp.Objects != null && temp.Objects.Count() == 1)
            {
                _selectedObservingFacility = temp.Objects.Single();
                Populate();
                //IsVisible = true;
            }
            else
            {
                _selectedObservingFacility = null;
                //IsVisible = false;
            }
        }

        public void Populate()
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacility =
                    unitOfWork.ObservingFacilities.GetIncludingGeospatialLocations(_selectedObservingFacility.Id);

                GeospatialLocationListItemViewModels = new ObservableCollection<GeospatialLocationListItemViewModel>(
                    observingFacility.Item2.Select(_ => new GeospatialLocationListItemViewModel{ GeospatialLocation = _}));
            }
        }

        private void CreateGeospatialLocation(
            object owner)
        {
            OnNewGeospatialLocationCalledByUser(owner);
        }

        private bool CanCreateGeospatialLocation(
            object owner)
        {
            return true;
        }

        private void DeleteSelectedGeospatialLocations()
        {
            throw new NotImplementedException();
        }

        private bool CanDeleteSelectedGeospatialLocations()
        {
            return SelectedGeospatialLocations.Objects != null &&
                   SelectedGeospatialLocations.Objects.Any();
        }

        private void UpdateSelectedGeospatialLocation(
            object owner)
        {
            var dialogViewModel = new DefineGeospatialLocationDialogViewModel();

            var point = SelectedGeospatialLocations.Objects.Single() as Point;
            dialogViewModel.Latitude = point.Coordinate1;
            dialogViewModel.Longitude = point.Coordinate2;
            dialogViewModel.From = point.From;
            dialogViewModel.To = point.To;

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var from = new DateTime(
                dialogViewModel.From.Year,
                dialogViewModel.From.Month,
                dialogViewModel.From.Day,
                dialogViewModel.From.Hour,
                dialogViewModel.From.Minute,
                dialogViewModel.From.Second,
                DateTimeKind.Utc);

            var to = dialogViewModel.To.HasValue
                ? dialogViewModel.To == DateTime.MaxValue
                    ? DateTime.MaxValue
                    : new DateTime(
                        dialogViewModel.To.Value.Year,
                        dialogViewModel.To.Value.Month,
                        dialogViewModel.To.Value.Day,
                        dialogViewModel.To.Value.Hour,
                        dialogViewModel.To.Value.Minute,
                        dialogViewModel.To.Value.Second,
                        DateTimeKind.Utc)
                : DateTime.MaxValue;

            var latitude = dialogViewModel.Latitude;
            var longitude = dialogViewModel.Longitude;

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var geospatialLocation = unitOfWork.GeospatialLocations.Get(point.Id);
                var now = DateTime.UtcNow;
                geospatialLocation.Superseded = now;

                unitOfWork.GeospatialLocations.Update(geospatialLocation);

                var newGeospatialLocation = new Point(Guid.NewGuid(), now)
                {
                    From = from,
                    To = to,
                    Coordinate1 = latitude,
                    Coordinate2 = longitude,
                    CoordinateSystem = "WGS_84"
                };

                unitOfWork.GeospatialLocations.Add(newGeospatialLocation);
                unitOfWork.Complete();
            }

            Populate();
        }

        private bool CanUpdateSelectedGeospatialLocation(
            object owner)
        {
            return SelectedGeospatialLocations.Objects != null &&
                   SelectedGeospatialLocations.Objects.Count() == 1;
        }

        private void OnNewGeospatialLocationCalledByUser(
            object owner)
        {
            var handler = NewGeospatialLocationCalledByUser;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new CommandInvokedEventArgs(owner) );
            }
        }
    }
}
