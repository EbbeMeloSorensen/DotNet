using GalaSoft.MvvmLight;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Persistence;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using C2IEDM.Domain.Entities;
using System.Windows;
using System;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

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

    public RelayCommand<object> CreateObservingFacilityCommand
    {
        get { return _createObservingFacilityCommand ?? (_createObservingFacilityCommand = new RelayCommand<object>(CreateObservingFacility, CanCreateObservingFacility)); }
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
        //DeleteSelectedPeopleCommand.RaiseCanExecuteChanged();
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
}