using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;
using Craft.DataStructures.IO.graphml.x;

namespace C2IEDM.ViewModel;

public class ObservingFacilityListViewModel : ViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    //private IList<ObservingFacility> _observingFacilities;
    private ObservableCollection<ObservingFacilityListItemViewModel> _observingFacilityListItemViewModels;
    private Sorting _sorting;

    private RelayCommand<object> _findObservingFacilitiesCommand;

    public ObjectCollection<ObservingFacility> ObservingFacilities { get; }
    public ObjectCollection<ObservingFacility> SelectedObservingFacilities { get; }

    public ObservableCollection<ObservingFacilityListItemViewModel> ObservingFacilityListItemViewModels
    {
        get { return _observingFacilityListItemViewModels; }
        set
        {
            _observingFacilityListItemViewModels = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<ObservingFacilityListItemViewModel> SelectedObservingFacilityListItemViewModels { get; set; }

    public ObservingFacilityFilterViewModel ObservingFacilityFilterViewModel { get; }

    public Sorting Sorting
    {
        get { return _sorting; }
        set
        {
            _sorting = value;
            RaisePropertyChanged();
            UpdateSorting();
            UpdateObservingFacilityListItemViewModels();
        }
    }

    public RelayCommand<object> FindObservingFacilitiesCommand
    {
        get
        {
            return _findObservingFacilitiesCommand ?? (_findObservingFacilitiesCommand = new RelayCommand<object>(FindObservingFacilities));
        }
    }

    public ObservingFacilityListViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService,
        ObservableObject<DateTime?> timeOfInterest,
        ObservableObject<bool> displayHistoricalTimeControls,
        ObservableObject<bool> displayDatabaseTimeControls)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _applicationDialogService = applicationDialogService;
        _sorting = Sorting.Name;

        ObservingFacilityFilterViewModel = new ObservingFacilityFilterViewModel(
            timeOfInterest, 
            displayHistoricalTimeControls,
            displayDatabaseTimeControls);

        ObservingFacilities = new ObjectCollection<ObservingFacility>
        {
            Objects = new List<ObservingFacility>()
        };

        SelectedObservingFacilities = new ObjectCollection<ObservingFacility>
        {
            Objects = new List<ObservingFacility>()
        };

        SelectedObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>();

        SelectedObservingFacilityListItemViewModels.CollectionChanged += (s, e) =>
        {
            SelectedObservingFacilities.Objects = SelectedObservingFacilityListItemViewModels.Select(_ => _.ObservingFacility);
        };
    }

    public void AddObservingFacility(
        ObservingFacility observingFacility)
    {
        var observingFacilities = ObservingFacilities.Objects.ToList();
        observingFacilities.Add(observingFacility);
        ObservingFacilities.Objects = observingFacilities;
        UpdateObservingFacilityListItemViewModels();
        SelectedObservingFacilityListItemViewModels.Clear();

        foreach (var observingFacilityListItemViewModel in ObservingFacilityListItemViewModels)
        {
            if (observingFacilityListItemViewModel.ObservingFacility.Id != observingFacility.Id) continue;

            SelectedObservingFacilityListItemViewModels.Add(observingFacilityListItemViewModel);
            break;
        }
    }

    public void UpdateObservingFacilities(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        var objectIdsOfUpdatedObservingFacilities = observingFacilities.Select(_ => _.ObjectId).ToList();

        foreach (var observingFacility in ObservingFacilities.Objects)
        {
            if (objectIdsOfUpdatedObservingFacilities.Contains(observingFacility.ObjectId))
            {
                var temp = observingFacilities.Single(_ => _.ObjectId == observingFacility.ObjectId);

                observingFacility.Name = temp.Name;
                observingFacility.DateEstablished = temp.DateEstablished;
                observingFacility.DateClosed = temp.DateClosed;
            }
        }

        ObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>(
            ObservingFacilities.Objects.Select(_ => new ObservingFacilityListItemViewModel { ObservingFacility = _ }));

        SelectedObservingFacilityListItemViewModels.Clear();

        foreach (var observingFacilityListItemViewModel in ObservingFacilityListItemViewModels)
        {
            if (objectIdsOfUpdatedObservingFacilities.Contains(observingFacilityListItemViewModel.ObservingFacility.ObjectId))
            {
                SelectedObservingFacilityListItemViewModels.Add(observingFacilityListItemViewModel);
            }
        }
    }

    public void RemoveObservingFacilities(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        var objectIdsOfDeletedObservingFacilities = observingFacilities.Select(_ => _.ObjectId);

        ObservingFacilities.Objects = ObservingFacilities.Objects
            .Where(_ => !objectIdsOfDeletedObservingFacilities.Contains(_.ObjectId))
            .ToList();

        SelectedObservingFacilityListItemViewModels.Clear();
        UpdateObservingFacilityListItemViewModels();
    }

    private void RetrieveObservingFacilitiesMatchingFilterFromRepository()
    {
        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            ObservingFacilities.Objects = 
                unitOfWork.ObservingFacilities.Find(ObservingFacilityFilterViewModel.FilterAsExpression()).ToList();
        }
    }

    private int CountObservingFacilitiesMatchingFilterFromRepository()
    {
        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            return unitOfWork.ObservingFacilities.Count(ObservingFacilityFilterViewModel.FilterAsExpression());
        }
    }

    private void UpdateSorting()
    {
    }

    private void UpdateObservingFacilityListItemViewModels()
    {
        UpdateSorting();

        ObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>(
            ObservingFacilities.Objects.Select(_ => new ObservingFacilityListItemViewModel() { ObservingFacility = _ }));
    }

    private void FindObservingFacilities(object owner)
    {
        var limit = 10;
        var count = CountObservingFacilitiesMatchingFilterFromRepository();

        if (count == 0)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("No observing facilities the search criteria", false);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        if (count > limit)
        {
            var dialogViewModel = new MessageBoxDialogViewModel($"{count} observing facilities match the search criteria.\nDo you want to retrieve them all from the repository?", true);
            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
            {
                return;
            }
        }

        RetrieveObservingFacilitiesMatchingFilterFromRepository();
        UpdateObservingFacilityListItemViewModels();
    }
}