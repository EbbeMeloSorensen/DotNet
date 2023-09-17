using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;

namespace C2IEDM.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Application.Application _application;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private readonly ILogger _logger;

    public ObservingFacilityListViewModel ObservingFacilityListViewModel { get; private set; }

    public LogViewModel LogViewModel { get; }

    private RelayCommand<object> _createObservingFacilityCommand;
    private RelayCommand _deleteSelectedObservingFacilitiesCommand;

    public RelayCommand<object> CreateObservingFacilityCommand
    {
        get { return _createObservingFacilityCommand ?? (_createObservingFacilityCommand = new RelayCommand<object>(CreateObservingFacility, CanCreateObservingFacility)); }
    }

    public RelayCommand DeleteSelectedObservingFacilitiesCommand
    {
        get { return _deleteSelectedObservingFacilitiesCommand ?? (_deleteSelectedObservingFacilitiesCommand = new RelayCommand(DeleteSelectedObservingFacilities, CanDeleteSelectedObservingFacilities)); }
    }

    public MainWindowViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService,
        ILogger logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _applicationDialogService = applicationDialogService;

        _application = new Application.Application(
            unitOfWorkFactory,
            logger);

        LogViewModel = new LogViewModel();
        _logger = new ViewModelLogger(logger, LogViewModel);

        ObservingFacilityListViewModel = new ObservingFacilityListViewModel(unitOfWorkFactory, applicationDialogService);

        ObservingFacilityListViewModel.SelectedObservingFacilities.PropertyChanged += HandleObservingFacilitySelectionChanged;
    }

    private void HandleObservingFacilitySelectionChanged(
        object sender,
        PropertyChangedEventArgs e)
    {
        DeleteSelectedObservingFacilitiesCommand.RaiseCanExecuteChanged();
        //ExportSelectionToGraphmlCommand.RaiseCanExecuteChanged();
    }

    private void CreateObservingFacility(object owner)
    {
        var dialogViewModel = new CreateObservingFacilityDialogViewModel();

        if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
        {
            return;
        }

        var dateEstablished = dialogViewModel.DateEstablished.HasValue
            ? new DateTime(
                dialogViewModel.DateEstablished.Value.Year,
                dialogViewModel.DateEstablished.Value.Month,
                dialogViewModel.DateEstablished.Value.Day,
                0, 0, 0, DateTimeKind.Utc)
            : new DateTime?();

        var dateClosed = dialogViewModel.DateClosed.HasValue
            ? new DateTime(
                dialogViewModel.DateClosed.Value.Year,
                dialogViewModel.DateClosed.Value.Month,
                dialogViewModel.DateClosed.Value.Day,
                0, 0, 0, DateTimeKind.Utc)
            : new DateTime?();

        var observingFacility = new ObservingFacility(
            Guid.NewGuid(),
            DateTime.UtcNow)
        {
            Name = dialogViewModel.Name,
            DateEstablished = dateEstablished,
            DateClosed = dateClosed,
            Created = DateTime.UtcNow
        };

        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            unitOfWork.ObservingFacilities.Add(observingFacility);
            unitOfWork.Complete();
        }

        ObservingFacilityListViewModel.AddObservingFacility(observingFacility);
    }

    private bool CanCreateObservingFacility(object owner)
    {
        return true;
    }

    private void DeleteSelectedObservingFacilities()
    {
        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            var ids = ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Select(p => p.Id).ToList();

            var observingFacilitiesForDeletion = unitOfWork.ObservingFacilities
                //.GetPeopleIncludingAssociations(p => ids.Contains(p.Id))
                .Find(p => ids.Contains(p.Id))
                .ToList();

            //var personAssociationsForDeletion = peopleForDeletion
            //    .SelectMany(p => p.ObjectPeople)
            //    .Concat(peopleForDeletion.SelectMany(p => p.SubjectPeople))
            //    .ToList();

            var now = DateTime.UtcNow;

            observingFacilitiesForDeletion.ForEach(_ => _.Superseded = now);

            //unitOfWork.PersonAssociations.RemoveRange(personAssociationsForDeletion);
            unitOfWork.ObservingFacilities.UpdateRange(observingFacilitiesForDeletion);
            unitOfWork.Complete();

            ObservingFacilityListViewModel.RemoveObservingFacilities(observingFacilitiesForDeletion);
        }
    }

    private bool CanDeleteSelectedObservingFacilities()
    {
        return ObservingFacilityListViewModel.SelectedObservingFacilities.Objects != null &&
               ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any();
    }
}