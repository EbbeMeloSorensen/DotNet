﻿using System;
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

namespace C2IEDM.ViewModel
{
    public class GeospatialLocationsViewModel : ViewModelBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;

        private ObjectCollection<ObservingFacility> _observingFacilities;
        private ObservingFacility _activeObservingFacility;
        private ObservableCollection<GeospatialLocationListItemViewModel> _geospatialLocationListItemViewModels;
        private RelayCommand _deleteSelectedGeospatialLocationsCommand;
        private RelayCommand<object> _createGeospatialLocationCommand;

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
            };

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

                GeospatialLocationListItemViewModels = new ObservableCollection<GeospatialLocationListItemViewModel>(
                    observingFacility.Item2.Select(_ => new GeospatialLocationListItemViewModel{ GeospatialLocation = _}));
            }
        }

        private void CreateGeospatialLocation(
            object owner)
        {
            var dialogViewModel = new DefineGeospatialLocationDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var now = DateTime.UtcNow;

                var point = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), now);
                point.From = dialogViewModel.From;
                point.Coordinate1 = dialogViewModel.Latitude;
                point.Coordinate2 = dialogViewModel.Longitude;
                point.CoordinateSystem = "WGS_84";
                point.AbstractEnvironmentalMonitoringFacilityId = _activeObservingFacility.Id;
                point.AbstractEnvironmentalMonitoringFacilityObjectId = _activeObservingFacility.ObjectId;

                unitOfWork.Points_Wigos.Add(point);

                unitOfWork.Complete();
            }

            Populate();
        }

        private bool CanCreateGeospatialLocation(object owner)
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
    }
}