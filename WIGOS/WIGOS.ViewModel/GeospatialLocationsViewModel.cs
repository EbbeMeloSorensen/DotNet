using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Persistence;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Point = WIGOS.Domain.Entities.WIGOS.GeospatialLocations.Point;

namespace WIGOS.ViewModel
{
    public class GeospatialLocationsViewModel : ViewModelBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;
        private readonly ObservableObject<DateTime?> _databaseTimeOfInterest;

        private ObjectCollection<ObservingFacility> _observingFacilities;
        private ObservingFacility _selectedObservingFacility;
        private ObservableCollection<GeospatialLocationListItemViewModel> _geospatialLocationListItemViewModels;
        private RelayCommand<object> _selectionChangedCommand;
        private RelayCommand<object> _deleteSelectedGeospatialLocationsCommand;
        private RelayCommand<object> _createGeospatialLocationCommand;
        private RelayCommand<object> _updateSelectedGeospatialLocationCommand;

        public ObjectCollection<GeospatialLocation> SelectedGeospatialLocations { get; private set; }

        public RelayCommand<object> SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>(SelectionChanged)); }
        }

        public ObservableCollection<GeospatialLocationListItemViewModel> GeospatialLocationListItemViewModels
        {
            get { return _geospatialLocationListItemViewModels; }
            set
            {
                _geospatialLocationListItemViewModels = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> DeleteSelectedGeospatialLocationsCommand
        {
            get
            {
                return _deleteSelectedGeospatialLocationsCommand ?? (
                    _deleteSelectedGeospatialLocationsCommand = new RelayCommand<object>(DeleteSelectedGeospatialLocations, CanDeleteSelectedGeospatialLocations));
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

        public event EventHandler<CommandInvokedEventArgs> NewGeospatialLocationCalledByUser;
        public event EventHandler<DatabaseWriteOperationOccuredEventArgs> GeospatialLocationsUpdatedOrDeleted;

        public GeospatialLocationsViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService,
            ObservableObject<DateTime?> databaseTimeOfInterest,
            ObjectCollection<ObservingFacility> observingFacilities)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _observingFacilities = observingFacilities;
            _databaseTimeOfInterest = databaseTimeOfInterest;

            SelectedGeospatialLocations = new ObjectCollection<GeospatialLocation>
            {
                Objects = new List<GeospatialLocation>()
            };

            _observingFacilities.PropertyChanged += Initialize;

            _databaseTimeOfInterest.PropertyChanged += (s, e) =>
            {
                CreateGeospatialLocationCommand.RaiseCanExecuteChanged();
                UpdateSelectedGeospatialLocationCommand.RaiseCanExecuteChanged();
                DeleteSelectedGeospatialLocationsCommand.RaiseCanExecuteChanged();
            };
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
                var geospatialLocationPredicates = new List<Expression<Func<GeospatialLocation, bool>>>
                {
                    Application.Helpers.GeospatialLocationFilterAsExpression(_databaseTimeOfInterest.Object)
                };

                var observingFacility =
                    unitOfWork.ObservingFacilities.GetIncludingGeospatialLocations(
                        _selectedObservingFacility.Id,
                        geospatialLocationPredicates);

                GeospatialLocationListItemViewModels = new ObservableCollection<GeospatialLocationListItemViewModel>(
                    observingFacility.Item2
                        .Select(_ => new GeospatialLocationListItemViewModel(_))
                        .OrderBy(_ => _.From));
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
            return !_databaseTimeOfInterest.Object.HasValue;
        }

        private void DeleteSelectedGeospatialLocations(
            object owner)
        {
            var nSelectedGeospatialLocations = SelectedGeospatialLocations.Objects.Count();

            var message = nSelectedGeospatialLocations == 1
                ? "Delete location?"
                : $"Delete {nSelectedGeospatialLocations} locations?";

            var dialogViewModel = new MessageBoxDialogViewModel(message, true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
            {
                return;
            }

            var objectIds = SelectedGeospatialLocations.Objects
                .Select(_ => _.ObjectId)
                .ToList();

            var geospatialLocationsRemaining = GeospatialLocationListItemViewModels
                .Select(_ => _.GeospatialLocation)
                .Where(_ => !objectIds.Contains(_.ObjectId))
                .ToList();

            var dateEstablishedAfter = geospatialLocationsRemaining.Min(_ => _.From);
            var dateClosedAfter = geospatialLocationsRemaining.Max(_ => _.To);

            var now = DateTime.UtcNow;

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var geospatialLocationsForDeletion = unitOfWork.GeospatialLocations
                    .Find(_ => _.Superseded == DateTime.MaxValue && objectIds.Contains(_.ObjectId))
                    .ToList();

                geospatialLocationsForDeletion.ForEach(_ => _.Superseded = now);

                if (dateEstablishedAfter > _selectedObservingFacility.DateEstablished ||
                    dateClosedAfter < _selectedObservingFacility.DateClosed)
                {
                    // Update active period of observing facility
                    var observingFacilityFromRepo = unitOfWork.ObservingFacilities.Get(_selectedObservingFacility.Id);

                    observingFacilityFromRepo.Superseded = now;
                    unitOfWork.ObservingFacilities.Update(observingFacilityFromRepo);

                    var newObservingFacility = new ObservingFacility(Guid.NewGuid(), now)
                    {
                        ObjectId = observingFacilityFromRepo.ObjectId,
                        Name = observingFacilityFromRepo.Name,
                        DateEstablished = dateEstablishedAfter > observingFacilityFromRepo.DateEstablished
                            ? dateEstablishedAfter
                            : observingFacilityFromRepo.DateEstablished,
                        DateClosed = dateClosedAfter < observingFacilityFromRepo.DateClosed
                            ? dateClosedAfter
                            : observingFacilityFromRepo.DateClosed
                    };

                    unitOfWork.ObservingFacilities.Add(newObservingFacility);
                }

                unitOfWork.GeospatialLocations.UpdateRange(geospatialLocationsForDeletion);
                unitOfWork.Complete();
            }

            OnGeospatialLocationsUpdatedOrDeleted(now);
        }

        private bool CanDeleteSelectedGeospatialLocations(
            object owber)
        {
            return !_databaseTimeOfInterest.Object.HasValue &&
                   //SelectedGeospatialLocations.Objects != null &&
                   SelectedGeospatialLocations.Objects.Any() &&
                   SelectedGeospatialLocations.Objects.Count() != _geospatialLocationListItemViewModels.Count;
        }

        private void UpdateSelectedGeospatialLocation(
            object owner)
        {
            var point = SelectedGeospatialLocations.Objects.Single() as Point;

            var dialogViewModel = new DefineGeospatialLocationDialogViewModel(
                DefineGeospatialLocationMode.Update,
                point.Coordinate1,
                point.Coordinate2,
                point.From,
                point.To == DateTime.MaxValue ? null : point.To);

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

            var latitude = double.Parse(dialogViewModel.Latitude, CultureInfo.InvariantCulture);
            var longitude = double.Parse(dialogViewModel.Longitude, CultureInfo.InvariantCulture);

            var now = DateTime.UtcNow;

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var geospatialLocation = unitOfWork.GeospatialLocations.Get(point.Id);
                geospatialLocation.Superseded = now;

                unitOfWork.GeospatialLocations.Update(geospatialLocation);

                var newPoint = new Point(Guid.NewGuid(), now)
                {
                    ObjectId = geospatialLocation.ObjectId,
                    AbstractEnvironmentalMonitoringFacilityId = _selectedObservingFacility.Id,
                    AbstractEnvironmentalMonitoringFacilityObjectId = _selectedObservingFacility.ObjectId,
                    From = from,
                    To = to,
                    Coordinate1 = latitude,
                    Coordinate2 = longitude,
                    CoordinateSystem = "WGS_84",
                };

                unitOfWork.GeospatialLocations.Add(newPoint);

                // Determine if we should change the DateEstablished/DateClosed range of the observing facility
                var geospatialLocationPredicates = new List<Expression<Func<GeospatialLocation, bool>>>
                {
                    _ => _.Superseded == DateTime.MaxValue,
                    _ => _.ObjectId != geospatialLocation.ObjectId
                };

                var otherGeospatialLocations =
                    unitOfWork.ObservingFacilities.GetIncludingGeospatialLocations(
                        _selectedObservingFacility.Id,
                        geospatialLocationPredicates).Item2;

                var minFromDate = otherGeospatialLocations
                    .Select(_ => _.From)
                    .Append(newPoint.From)
                    .Min();

                var maxToDate = otherGeospatialLocations
                    .Select(_ => _.To)
                    .Append(newPoint.To)
                    .Max();

                if (minFromDate != _selectedObservingFacility.DateEstablished ||
                    maxToDate != _selectedObservingFacility.DateClosed)
                {
                    var observingFacilityFromRepo = unitOfWork.ObservingFacilities.Get(_selectedObservingFacility.Id);
                    observingFacilityFromRepo.Superseded = now;

                    unitOfWork.ObservingFacilities.Update(observingFacilityFromRepo);

                    var newObservingFacility = new ObservingFacility(Guid.NewGuid(), now)
                    {
                        Name = observingFacilityFromRepo.Name,
                        ObjectId = observingFacilityFromRepo.ObjectId,
                        DateEstablished = minFromDate,
                        DateClosed = maxToDate
                    };

                    unitOfWork.ObservingFacilities.Add(newObservingFacility);
                }

                unitOfWork.Complete();
            }

            OnGeospatialLocationsUpdatedOrDeleted(now);
        }

        private bool CanUpdateSelectedGeospatialLocation(
            object owner)
        {
            return !_databaseTimeOfInterest.Object.HasValue &&
                   //SelectedGeospatialLocations.Objects != null &&
                   SelectedGeospatialLocations.Objects.Count() == 1;
        }

        private void SelectionChanged(
            object obj)
        {
            var temp = (IList)obj;

            var selectedGeospatialLocationListItemViewModels =
                temp.Cast<GeospatialLocationListItemViewModel>();

            SelectedGeospatialLocations.Objects = selectedGeospatialLocationListItemViewModels.Select(_ => _.GeospatialLocation);

            DeleteSelectedGeospatialLocationsCommand.RaiseCanExecuteChanged();
            UpdateSelectedGeospatialLocationCommand.RaiseCanExecuteChanged();
        }

        private void OnNewGeospatialLocationCalledByUser(
            object owner)
        {
            var handler = NewGeospatialLocationCalledByUser;

            if (handler != null)
            {
                handler(this, new CommandInvokedEventArgs(owner));
            }
        }

        private void OnGeospatialLocationsUpdatedOrDeleted(
            DateTime dateTime)
        {
            var handler = GeospatialLocationsUpdatedOrDeleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new DatabaseWriteOperationOccuredEventArgs(dateTime));
            }
        }
    }
}
