using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D.ScrollFree;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;

namespace C2IEDM.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Application.Application _application;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private readonly ILogger _logger;
    private readonly List<DateTime> _databaseWriteTimes;
    private readonly ObservableObject<DateTime?> _timeOfInterest;
    private Brush _timeStampBrush = new SolidColorBrush(Colors.DarkSlateBlue);

    public ObservingFacilityListViewModel ObservingFacilityListViewModel { get; }

    public ObservingFacilitiesDetailsViewModel ObservingFacilitiesDetailsViewModel { get; }

    public TimeSeriesViewModel TimeSeriesViewModel { get; }

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

        _timeOfInterest = new ObservableObject<DateTime?>
        {
            Object = null
            //Object = new DateTime(2023, 9, 17, 13, 0, 0, DateTimeKind.Utc) // -- Kun bræk
            //Object = new DateTime(2023, 9, 17, 11, 0, 0, DateTimeKind.Utc) // -- 
        };

        ObservingFacilityListViewModel = new ObservingFacilityListViewModel(
            unitOfWorkFactory, 
            applicationDialogService,
            _timeOfInterest);

        ObservingFacilitiesDetailsViewModel = new ObservingFacilitiesDetailsViewModel(
            unitOfWorkFactory,
            ObservingFacilityListViewModel.SelectedObservingFacilities);

        var timeSpan = TimeSpan.FromHours(1);
        var utcNow = DateTime.UtcNow;
        var timeAtOrigo = utcNow.Date;
        var tFocus = utcNow - timeSpan / 2 + TimeSpan.FromMinutes(1);
        var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);

        TimeSeriesViewModel = new TimeSeriesViewModel(
            new Point(xFocus, 0),
            new Size(timeSpan.TotalDays, 3),
            true,
            25,
            60,
            timeAtOrigo);

        TimeSeriesViewModel.GeometryEditorViewModel.YAxisLocked = true;
        TimeSeriesViewModel.ShowHorizontalGridLines = false;

        ObservingFacilitiesDetailsViewModel.ObservingFacilitiesUpdated += ObservingFacilityDetailsViewModel_ObservingFacilitiesUpdated;

        ObservingFacilityListViewModel.SelectedObservingFacilities.PropertyChanged += HandleObservingFacilitySelectionChanged;

        using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
        {
            var observingFacilities = unitOfWork.ObservingFacilities.GetAll();
            var timeStampsOfInterest =  observingFacilities.Select(_ => _.Created).ToList();
            timeStampsOfInterest.AddRange(observingFacilities.Select(_ => _.Superseded));
            _databaseWriteTimes = timeStampsOfInterest.Distinct().ToList();
        }

        TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
        {
            RefreshTimeSeriesView();
        };

        TimeSeriesViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
        {
            _timeOfInterest.Object = TimeSeriesViewModel.TimeAtMousePosition.Object;
        };
    }

    private void ObservingFacilityDetailsViewModel_ObservingFacilitiesUpdated(
        object? sender, 
        Application.ObservingFacilitiesEventArgs e)
    {
        ObservingFacilityListViewModel.UpdateObservingFacilities(e.ObservingFacilities);
    }

    private void HandleObservingFacilitySelectionChanged(
        object sender,
        PropertyChangedEventArgs e)
    {
        DeleteSelectedObservingFacilitiesCommand.RaiseCanExecuteChanged();
        //ExportSelectionToGraphmlCommand.RaiseCanExecuteChanged();
    }

    private void CreateObservingFacility(
        object owner)
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

        _databaseWriteTimes.Add(DateTime.UtcNow);
        RefreshTimeSeriesView();
    }

    private bool CanCreateObservingFacility(
        object owner)
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

            _databaseWriteTimes.Add(DateTime.UtcNow);
            RefreshTimeSeriesView();
        }
    }

    private bool CanDeleteSelectedObservingFacilities()
    {
        return ObservingFacilityListViewModel.SelectedObservingFacilities.Objects != null &&
               ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any();
    }

    private void RefreshTimeSeriesView()
    {
        var x0 = TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X;
        var x1 = TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X + TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowSize.Width;
        var y0 = TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y;

        var y1 = TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y +
             TimeSeriesViewModel.GeometryEditorViewModel.WorldWindowSize.Height * TimeSeriesViewModel.Y2 /
             TimeSeriesViewModel.GeometryEditorViewModel.ViewPortSize.Height;

        TimeSeriesViewModel.GeometryEditorViewModel.ClearLines();

        var lineThickness = 2.0 / TimeSeriesViewModel.GeometryEditorViewModel.Scaling.Width;

        var lineViewModels = _databaseWriteTimes
            .Select(_ => (_ - TimeSeriesViewModel.TimeAtOrigo).TotalDays)
            .Where(_ => _ > x0 && _ < x1)
            .Select(_ => new LineViewModel(new PointD(_, y0), new PointD(_, y1), lineThickness, _timeStampBrush))
            .ToList();

        lineViewModels.ForEach(_ => TimeSeriesViewModel.GeometryEditorViewModel.LineViewModels.Add(_));
    }
}