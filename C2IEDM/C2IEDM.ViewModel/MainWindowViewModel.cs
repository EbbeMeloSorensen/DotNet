using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.DataStructures.IO;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D.ScrollFree;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Persistence;

namespace C2IEDM.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private ILogger _logger;
    private readonly Application.Application _application;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private readonly List<DateTime> _databaseWriteTimes;
    private readonly ObservableObject<DateTime?> _timeOfInterest;
    private readonly Brush _timeStampBrush = new SolidColorBrush(Colors.DarkSlateBlue);
    private readonly Brush _timeOfInterestBrush = new SolidColorBrush(Colors.OrangeRed);
    private readonly ObservableObject<bool> _displayRetrospectionControls;
    private readonly ObservableObject<bool> _displayHistoricalTimeControls;
    private readonly ObservableObject<bool> _displayDatabaseTimeControls;
    private bool _displayMessageInMap;
    private int _selectedTabIndexForRetrospectionTimeLines;
    private Window _owner;

    public bool DisplayRetrospectionControls
    {
        get => _displayRetrospectionControls.Object;
        set
        {
            _displayRetrospectionControls.Object = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayHistoricalTimeControls
    {
        get => _displayHistoricalTimeControls.Object;
        set
        {
            _displayHistoricalTimeControls.Object = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayDatabaseTimeControls
    {
        get => _displayDatabaseTimeControls.Object;
        set
        {
            _displayDatabaseTimeControls.Object = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayMessageInMap 
    {
        get => _displayMessageInMap;
        set
        {
            _displayMessageInMap = value;
            RaisePropertyChanged();
        }
    }

    public int SelectedTabIndexForRetrospectionTimeLines
    {
        get => _selectedTabIndexForRetrospectionTimeLines;
        set
        {
            _selectedTabIndexForRetrospectionTimeLines = value;
            RaisePropertyChanged();
        }
    }

    public LogViewModel LogViewModel { get; private set; }
    public ObservingFacilityListViewModel ObservingFacilityListViewModel { get; private set; }
    public ObservingFacilitiesDetailsViewModel ObservingFacilitiesDetailsViewModel { get; private set; }
    public GeometryEditorViewModel MapViewModel { get; private set; }
    public TimeSeriesViewModel DatabaseWriteTimesViewModel { get; private set;  }
    public TimeSeriesViewModel HistoricalTimeViewModel { get; private set; }

    private RelayCommand<object> _createObservingFacilityCommand;
    private RelayCommand _deleteSelectedObservingFacilitiesCommand;
    private RelayCommand _escapeCommand;

    public RelayCommand<object> CreateObservingFacilityCommand
    {
        get { return _createObservingFacilityCommand ?? (_createObservingFacilityCommand = new RelayCommand<object>(CreateObservingFacility, CanCreateObservingFacility)); }
    }

    public RelayCommand DeleteSelectedObservingFacilitiesCommand
    {
        get { return _deleteSelectedObservingFacilitiesCommand ?? (_deleteSelectedObservingFacilitiesCommand = 
                new RelayCommand(DeleteSelectedObservingFacilities, CanDeleteSelectedObservingFacilities)); }
    }

    public RelayCommand EscapeCommand
    {
        get { return _escapeCommand ?? (_escapeCommand = new RelayCommand(Escape)); }
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

        _timeOfInterest = new ObservableObject<DateTime?>
        {
            Object = null
        };

        _displayRetrospectionControls = new ObservableObject<bool>
        {
            Object = false
        };

        _displayHistoricalTimeControls = new ObservableObject<bool>
        {
            Object = false
        };

        _displayDatabaseTimeControls = new ObservableObject<bool>
        {
            Object = false
        };

        _displayHistoricalTimeControls.PropertyChanged += (s, e) =>
            UpdateRetrospectionControls();

        _displayDatabaseTimeControls.PropertyChanged += (s, e) =>
            UpdateRetrospectionControls();

        InitializeLogViewModel(logger);
        InitializeObservingFacilityListViewModel(unitOfWorkFactory, applicationDialogService);
        InitializeObservingFacilitiesDetailsViewModel(unitOfWorkFactory);
        InitializeMapViewModel();
        InitializeDatabaseWriteTimesViewModel();
        InitializeHistoricalTimeViewModel();

        DrawMapOfDenmark();

        if (false)
        {
            try
            {
                using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
                {
                    var observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                    var timeStampsOfInterest = observingFacilities.Select(_ => _.Created).ToList();
                    timeStampsOfInterest.AddRange(observingFacilities.Select(_ => _.Superseded));
                    _databaseWriteTimes = timeStampsOfInterest.Distinct().ToList();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Just swallow it for now - write it to the log later
                _databaseWriteTimes = new List<DateTime>();
            }
        }
        else
        {
            _databaseWriteTimes = new List<DateTime>();
        }
    }

    private void CreateObservingFacility(
        object owner)
    {
        _owner = owner as Window;
        DisplayMessageInMap = true;
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
            // Todo: Remember to also delete orphaned children

            var objectIds = ObservingFacilityListViewModel.SelectedObservingFacilities.Objects
                .Select(_ => _.ObjectId).ToList();

            var observingFacilitiesForDeletion = unitOfWork.ObservingFacilities
                .Find(_ => _.Superseded == DateTime.MaxValue && objectIds.Contains(_.ObjectId))
                .ToList();

            var now = DateTime.UtcNow;

            observingFacilitiesForDeletion.ForEach(_ => _.Superseded = now);

            unitOfWork.ObservingFacilities.UpdateRange(observingFacilitiesForDeletion);
            unitOfWork.Complete();

            ObservingFacilityListViewModel.RemoveObservingFacilities(observingFacilitiesForDeletion);

            _databaseWriteTimes.Add(now);
            RefreshTimeSeriesView();
        }
    }

    private bool CanDeleteSelectedObservingFacilities()
    {
        return ObservingFacilityListViewModel.SelectedObservingFacilities.Objects != null &&
               ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any();
    }

    private void Escape()
    {
        DisplayMessageInMap = false;
    }

    private void InitializeLogViewModel(ILogger logger)
    {
        LogViewModel = new LogViewModel();
        _logger = new ViewModelLogger(logger, LogViewModel);
    }

    private void InitializeObservingFacilityListViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService)
    {
        ObservingFacilityListViewModel = new ObservingFacilityListViewModel(
            unitOfWorkFactory,
            applicationDialogService,
            _timeOfInterest,
            _displayHistoricalTimeControls,
            _displayDatabaseTimeControls);

        ObservingFacilityListViewModel.SelectedObservingFacilities.PropertyChanged += (s, e) =>
        {
            DeleteSelectedObservingFacilitiesCommand.RaiseCanExecuteChanged();
        };
    }

    private void InitializeObservingFacilitiesDetailsViewModel(
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        ObservingFacilitiesDetailsViewModel = new ObservingFacilitiesDetailsViewModel(
            unitOfWorkFactory,
            ObservingFacilityListViewModel.SelectedObservingFacilities);

        ObservingFacilitiesDetailsViewModel.ObservingFacilitiesUpdated += (s, e) =>
        {
            ObservingFacilityListViewModel.UpdateObservingFacilities(e.ObservingFacilities);
            _databaseWriteTimes.Add(e.ObservingFacilities.First().Created);
            RefreshTimeSeriesView();
        };
    }

    private void InitializeMapViewModel()
    {
        //var worldWindowBoundingBoxNorthWest = new Point(10.25, 57.95);
        //var worldWindowBoundingBoxSouthEast = new Point(13.75, 54.45);
        var worldWindowBoundingBoxNorthWest = new Point(8, 57.95);
        var worldWindowBoundingBoxSouthEast = new Point(16, 54.45);

        var worldWindowFocus = new Point(
            (worldWindowBoundingBoxNorthWest.X + worldWindowBoundingBoxSouthEast.X) / 2,
            (worldWindowBoundingBoxNorthWest.Y + worldWindowBoundingBoxSouthEast.Y) / 2);

        var worldWindowSize = new Size(
            Math.Abs(worldWindowBoundingBoxNorthWest.X - worldWindowBoundingBoxSouthEast.X),
            Math.Abs(worldWindowBoundingBoxNorthWest.Y - worldWindowBoundingBoxSouthEast.Y));

        MapViewModel = new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize, false);

        MapViewModel.MouseClickOccured += (s, e) =>
        {
            if (!DisplayMessageInMap || !MapViewModel.MousePositionWorld.Object.HasValue)
            {
                return;
            }

            DisplayMessageInMap = false;

            var dialogViewModel = new CreateObservingFacilityDialogViewModel(MapViewModel.MousePositionWorld.Object.Value);

            if (_applicationDialogService.ShowDialog(dialogViewModel, _owner) != DialogResult.OK)
            {
                return;
            }

            var dateEstablished = new DateTime(
                dialogViewModel.DateEstablished.Year,
                dialogViewModel.DateEstablished.Month,
                dialogViewModel.DateEstablished.Day,
                0, 0, 0, DateTimeKind.Utc);

            var dateClosed = dialogViewModel.DateClosed.HasValue
                ? new DateTime(
                    dialogViewModel.DateClosed.Value.Year,
                    dialogViewModel.DateClosed.Value.Month,
                    dialogViewModel.DateClosed.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : new DateTime?();

            var from = new DateTime(
                dialogViewModel.From.Year,
                dialogViewModel.From.Month,
                dialogViewModel.From.Day,
                0, 0, 0, DateTimeKind.Utc);

            var to = dialogViewModel.To.HasValue
                ? new DateTime(
                    dialogViewModel.To.Value.Year,
                    dialogViewModel.To.Value.Month,
                    dialogViewModel.To.Value.Day,
                    0, 0, 0, DateTimeKind.Utc)
                : new DateTime?();

            var latitude = dialogViewModel.Latitude;

            var longitude = dialogViewModel.Longitude;

            var now = DateTime.UtcNow;

            var observingFacility = new ObservingFacility(
                Guid.NewGuid(),
                now)
            {
                Name = dialogViewModel.Name,
                DateEstablished = dateEstablished,
                DateClosed = dateClosed
            };

            var point = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), now)
            {
                AbstractEnvironmentalMonitoringFacility = observingFacility,
                AbstractEnvironmentalMonitoringFacilityObjectId = observingFacility.ObjectId,
                From = from,
                To = to,
                Coordinate1 = latitude,
                Coordinate2 = longitude,
                CoordinateSystem = "WGS_84"
            };

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.ObservingFacilities.Add(observingFacility);
                unitOfWork.Points_Wigos.Add(point);
                unitOfWork.Complete();
            }

            ObservingFacilityListViewModel.AddObservingFacility(observingFacility);

            _databaseWriteTimes.Add(now);
            RefreshTimeSeriesView();
        };
    }

    private void InitializeHistoricalTimeViewModel()
    {
        var timeSpan = TimeSpan.FromDays(40);
        var utcNow = DateTime.UtcNow;
        var timeAtOrigo = utcNow.Date;
        var tFocus = utcNow - timeSpan / 2 + TimeSpan.FromMinutes(1);
        var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);

        HistoricalTimeViewModel = new TimeSeriesViewModel(
            new Point(xFocus, 0),
            new Size(timeSpan.TotalDays, 3),
            true,
            0,
            40,
            timeAtOrigo);

        HistoricalTimeViewModel.GeometryEditorViewModel.YAxisLocked = true;
        HistoricalTimeViewModel.ShowHorizontalGridLines = false;
        HistoricalTimeViewModel.ShowHorizontalAxis = false;

        HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
        {
        };

        HistoricalTimeViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
        {
        };
    }

    private void InitializeDatabaseWriteTimesViewModel()
    {
        var timeSpan = TimeSpan.FromHours(1);
        var utcNow = DateTime.UtcNow;
        var timeAtOrigo = utcNow.Date;
        var tFocus = utcNow - timeSpan / 2 + TimeSpan.FromMinutes(1);
        var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);

        DatabaseWriteTimesViewModel = new TimeSeriesViewModel(
            new Point(xFocus, 0),
            new Size(timeSpan.TotalDays, 3),
            true,
            0,
            40,
            timeAtOrigo);

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.YAxisLocked = true;
        DatabaseWriteTimesViewModel.ShowHorizontalGridLines = false;
        DatabaseWriteTimesViewModel.ShowHorizontalAxis = false;

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
        {
            RefreshTimeSeriesView();
        };

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
        {
            _timeOfInterest.Object = DatabaseWriteTimesViewModel.TimeAtMousePosition.Object;
            RefreshTimeSeriesView();
        };
    }

    private void RefreshTimeSeriesView()
    {
        var x0 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X;
        var x1 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X + DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Width;
        var y0 = -DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y - DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Height;
        var y1 = -DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y;

        var y2 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y +
             DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Height * DatabaseWriteTimesViewModel.Y2 /
             DatabaseWriteTimesViewModel.GeometryEditorViewModel.ViewPortSize.Height;

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.ClearLines();

        var lineThickness = 2.0 / DatabaseWriteTimesViewModel.GeometryEditorViewModel.Scaling.Width;

        var lineViewModels = _databaseWriteTimes
            .Select(_ => (_ - DatabaseWriteTimesViewModel.TimeAtOrigo).TotalDays)
            .Where(_ => _ > x0 && _ < x1)
            .Select(_ => new LineViewModel(new PointD(_, y0), new PointD(_, y2), lineThickness, _timeStampBrush))
            .ToList();

        lineViewModels.ForEach(_ => DatabaseWriteTimesViewModel.GeometryEditorViewModel.LineViewModels.Add(_));

        if (_timeOfInterest.Object.HasValue)
        {
            var xTimeOfInterest = (_timeOfInterest.Object.Value - DatabaseWriteTimesViewModel.TimeAtOrigo).TotalDays;

            if (xTimeOfInterest > x0 && xTimeOfInterest < x1)
            {
                DatabaseWriteTimesViewModel.GeometryEditorViewModel.LineViewModels.Add(
                    new LineViewModel(new PointD(xTimeOfInterest, y0), new PointD(xTimeOfInterest, y2), lineThickness, _timeOfInterestBrush));

            }
        }
    }

    private void DrawMapOfDenmark()
    {
        // Load GML file of Denmark
        //var fileName = @".\Data\Denmark.gml";
        var fileName = @".\Data\DenmarkAndGreenland.gml";
        DataIOHandler.ExtractGeometricPrimitivesFromGMLFile(fileName, out var polygons);

        // Add the regions of Denmark to the map as polygons
        var lineThickness = 0.005;
        var brush = new SolidColorBrush(new Color { R = 150, G = 255, B = 150, A = 127 });

        foreach (var polygon in polygons)
        {
            MapViewModel.AddPolygon(polygon.Select(p => new PointD(p[1], p[0])), lineThickness, brush);
        }
    }

    private void UpdateRetrospectionControls()
    {
        if (_displayHistoricalTimeControls.Object == false &&
            SelectedTabIndexForRetrospectionTimeLines == 0)
        {
            SelectedTabIndexForRetrospectionTimeLines = 1;
        }
        else if (_displayDatabaseTimeControls.Object == false &&
                 SelectedTabIndexForRetrospectionTimeLines == 1)
        {
            SelectedTabIndexForRetrospectionTimeLines = 0;
        }

        DisplayRetrospectionControls = 
            _displayHistoricalTimeControls.Object ||
            _displayDatabaseTimeControls.Object;
    }
}