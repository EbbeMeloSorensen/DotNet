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

namespace C2IEDM.ViewModel;

public class ObservingFacilityListViewModel : ViewModelBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private IList<ObservingFacility> _observingFacilities;
    private Sorting _sorting;

    public FindObservingFacilitiesViewModel FindObservingFacilitiesViewModel { get; }
    private ObservableCollection<ObservingFacilityListItemViewModel> _observingFacilityListItemViewModels;

    private RelayCommand<object> _findObservingFacilitiesCommand;

    public ObservableCollection<ObservingFacilityListItemViewModel> ObservingFacilityListItemViewModels
    {
        get { return _observingFacilityListItemViewModels; }
        set
        {
            _observingFacilityListItemViewModels = value;
            RaisePropertyChanged();
        }
    }

    public ObjectCollection<ObservingFacility> SelectedObservingFacilities { get; }

    public ObservableCollection<ObservingFacilityListItemViewModel> SelectedObservingFacilityListItemViewModels { get; set; }

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
        ObservableObject<DateTime?> timeOfInterest)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _applicationDialogService = applicationDialogService;
        _sorting = Sorting.Name;

        FindObservingFacilitiesViewModel = new FindObservingFacilitiesViewModel(timeOfInterest);

        _observingFacilities = new List<ObservingFacility>();

        SelectedObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>();
        SelectedObservingFacilities = new ObjectCollection<ObservingFacility>();

        SelectedObservingFacilityListItemViewModels.CollectionChanged += (s, e) =>
        {
            SelectedObservingFacilities.Objects = SelectedObservingFacilityListItemViewModels.Select(_ => _.ObservingFacility);
        };
    }

    public void AddObservingFacility(
        ObservingFacility observingFacility)
    {
        _observingFacilities.Add(observingFacility);
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
        var idsOfUpdatedObservingFacilities = observingFacilities.Select(_ => _.Id).ToList();

        foreach (var observingFacility in _observingFacilities)
        {
            if (idsOfUpdatedObservingFacilities.Contains(observingFacility.Id))
            {
                var temp = observingFacilities.Single(_ => _.Id == observingFacility.Id);

                observingFacility.Name = temp.Name;
                observingFacility.DateEstablished = temp.DateEstablished;
                observingFacility.DateClosed = temp.DateClosed;
            }
        }

        ObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>(_observingFacilities.Select(
            _ => new ObservingFacilityListItemViewModel { ObservingFacility = _ }));

        SelectedObservingFacilityListItemViewModels.Clear();

        foreach (var observingFacilityListItemViewModel in ObservingFacilityListItemViewModels)
        {
            if (idsOfUpdatedObservingFacilities.Contains(observingFacilityListItemViewModel.ObservingFacility.Id))
            {
                SelectedObservingFacilityListItemViewModels.Add(observingFacilityListItemViewModel);
            }
        }
    }

    public void RemoveObservingFacilities(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        var idsOfDeletedObservingFacilities = observingFacilities.Select(_ => _.Id);

        _observingFacilities = _observingFacilities
            .Where(_ => !idsOfDeletedObservingFacilities.Contains(_.Id))
            .ToList();

        SelectedObservingFacilityListItemViewModels.Clear();
        UpdateObservingFacilityListItemViewModels();
    }

    private void RetrieveObservingFacilitiesMatchingFilterFromRepository()
    {
        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            _observingFacilities = unitOfWork.ObservingFacilities.Find(FindObservingFacilitiesViewModel.FilterAsExpression()).ToList();
        }
    }

    private int CountObservingFacilitiesMatchingFilterFromRepository()
    {
        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            return unitOfWork.ObservingFacilities.Count(FindObservingFacilitiesViewModel.FilterAsExpression());
        }
    }

    private void UpdateSorting()
    {
    }

    private void UpdateObservingFacilityListItemViewModels()
    {
        UpdateSorting();

        ObservingFacilityListItemViewModels = new ObservableCollection<ObservingFacilityListItemViewModel>(_observingFacilities.Select(
            _ => new ObservingFacilityListItemViewModel() { ObservingFacility = _ }));
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