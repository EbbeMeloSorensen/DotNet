using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using C2IEDM.Domain.Entities;

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
        IDialogService applicationDialogService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _applicationDialogService = applicationDialogService;
        _sorting = Sorting.Name;

        FindObservingFacilitiesViewModel = new FindObservingFacilitiesViewModel();

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
            var dialogViewModel = new MessageBoxDialogViewModel("No person matches the search criteria", false);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        if (count > limit)
        {
            var dialogViewModel = new MessageBoxDialogViewModel($"{count} people match the search criteria.\nDo you want to retrieve them all from the repository?", true);
            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
            {
                return;
            }
        }

        RetrieveObservingFacilitiesMatchingFilterFromRepository();
        UpdateObservingFacilityListItemViewModels();
    }
}