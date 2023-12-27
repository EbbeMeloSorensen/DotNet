using System;
using System.Linq;
using System.Collections.Generic;
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
using System.Globalization;

namespace C2IEDM.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private enum MapOperation
    {
        None,
        CreateObservingFacility,
        CreateGeospatialLocation
    }

    private ILogger _logger;
    private readonly Application.Application _application;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private readonly List<DateTime> _databaseWriteTimes;
    private readonly List<DateTime> _historicalChangeTimes;
    private readonly ObservableObject<DateTime?> _historicalTimeOfInterest;
    private readonly ObservableObject<DateTime?> _databaseTimeOfInterest;
    private readonly ObservableObject<bool> _autoRefresh;
    private readonly ObservableObject<bool> _displayNameFilter;
    private readonly ObservableObject<bool> _displayStatusFilter;
    private readonly ObservableObject<bool> _showActiveStations;
    private readonly ObservableObject<bool> _showClosedStations;
    private readonly ObservableObject<bool> _displayRetrospectionControls;
    private readonly ObservableObject<bool> _displayHistoricalTimeControls;
    private readonly ObservableObject<bool> _displayDatabaseTimeControls;
    private readonly Brush _mapBrushSeaCurrent = new SolidColorBrush(new Color { R = 200, G = 200, B = 255, A = 255 });
    private readonly Brush _mapBrushSeaHistoric = new SolidColorBrush(new Color { R = 239, G = 228, B = 176, A = 255 });
    private readonly Brush _mapBrushLandCurrent = new SolidColorBrush(new Color { R = 100, G = 200, B = 100, A = 255 });
    private readonly Brush _mapBrushLandHistoric = new SolidColorBrush(new Color { R = 185, G = 122, B = 87, A = 255 });
    private readonly Brush _timeStampBrush = new SolidColorBrush(Colors.DarkSlateBlue);
    private readonly Brush _activeObservingFacilityBrush = new SolidColorBrush(Colors.Black);
    private readonly Brush _closedObservingFacilityBrush = new SolidColorBrush(Colors.DarkRed);
    private readonly Brush _controlBackgroundBrushCurrent = new SolidColorBrush(Colors.WhiteSmoke);
    private readonly Brush _controlBackgroundBrushHistoric = new SolidColorBrush(Colors.BurlyWood);
    private Brush _controlBackgroundBrush;
    private string _messageInMap;
    private string _statusBarText;
    private bool _displayMessageInMap;
    private bool _displayLog;
    private int _selectedTabIndexForRetrospectionTimeLines;
    private Window _owner;
    private MapOperation _mapOperation;

    private RelayCommand<object> _createObservingFacilityCommand;
    private RelayCommand<object> _deleteSelectedObservingFacilitiesCommand;
    private RelayCommand<object> _clearRepositoryCommand;
    private RelayCommand _escapeCommand;

    public bool AutoRefresh
    {
        get => _autoRefresh.Object;
        set
        {
            _autoRefresh.Object = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayNameFilter
    {
        get => _displayNameFilter.Object;
        set
        {
            _displayNameFilter.Object = value;
            RaisePropertyChanged();
        }
    }

    public bool DisplayStatusFilter
    {
        get => _displayStatusFilter.Object;
        set
        {
            _displayStatusFilter.Object = value;
            RaisePropertyChanged();
        }
    }

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

    public string StatusBarText
    {
        get => _statusBarText;
        set
        {
            _statusBarText = value;
            RaisePropertyChanged();
        }
    }

    public string MessageInMap
    {
        get => _messageInMap;
        set
        {
            _messageInMap = value;
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

    public bool DisplayLog
    {
        get => _displayLog;
        set
        {
            _displayLog = value;
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

    public Brush ControlBackgroundBrush
    {
        get => _controlBackgroundBrush;
        set
        {
            _controlBackgroundBrush = value;
            RaisePropertyChanged();
        }
    }

    public LogViewModel LogViewModel { get; private set; }
    public ObservingFacilityListViewModel ObservingFacilityListViewModel { get; private set; }
    public ObservingFacilitiesDetailsViewModel ObservingFacilitiesDetailsViewModel { get; private set; }
    public GeometryEditorViewModel MapViewModel { get; private set; }
    public TimeSeriesViewModel DatabaseWriteTimesViewModel { get; private set;  }
    public TimeSeriesViewModel HistoricalTimeViewModel { get; private set; }

    public RelayCommand<object> CreateObservingFacilityCommand
    {
        get { return _createObservingFacilityCommand ?? (_createObservingFacilityCommand = new RelayCommand<object>(CreateObservingFacility, CanCreateObservingFacility)); }
    }

    public RelayCommand<object> DeleteSelectedObservingFacilitiesCommand
    {
        get { return _deleteSelectedObservingFacilitiesCommand ?? (_deleteSelectedObservingFacilitiesCommand = 
                new RelayCommand<object>(DeleteSelectedObservingFacilities, CanDeleteSelectedObservingFacilities)); }
    }

    public RelayCommand<object> ClearRepositoryCommand
    {
        get
        {
            return _clearRepositoryCommand ?? (_clearRepositoryCommand =
                new RelayCommand<object>(ClearRepository, CanClearRepository));
        }
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
        _logger = logger;

        _application = new Application.Application(
            unitOfWorkFactory,
            _logger);

        _historicalTimeOfInterest = new ObservableObject<DateTime?>
        {
            Object = null
        };

        _databaseTimeOfInterest = new ObservableObject<DateTime?>
        {
            Object = null
        };

        DisplayLog = false;
        //DisplayLog = true; // Set to true when diagnosing application behaviour

        _historicalTimeOfInterest.PropertyChanged += (s, e) =>
        {
            if (_historicalTimeOfInterest.Object.HasValue)
            {
                // Highlight the position of the time of interest
                HistoricalTimeViewModel.StaticXValue =
                    (_historicalTimeOfInterest.Object.Value - HistoricalTimeViewModel.TimeAtOrigo) / TimeSpan.FromDays(1);
            }
            else
            {
                HistoricalTimeViewModel.StaticXValue = null;

                // Vi vil gerne se situationen, som den gør sig gældende nu. Derfor opererer vi også essentielt med seneste version af databasen
                _databaseTimeOfInterest.Object = null;
            }

            UpdateMapColoring();
            RefreshHistoricalTimeSeriesView(false);
            UpdateControlBackground();
            UpdateStatusBar();
        };

        _databaseTimeOfInterest.PropertyChanged += (s, e) =>
        {
            UpdateMapColoring();
            RefreshDatabaseTimeSeriesView();
            UpdateControlBackground();
            UpdateStatusBar();
            UpdateCommands();
        };

        _autoRefresh = new ObservableObject<bool>
        {
            Object = true
        };

        _displayNameFilter = new ObservableObject<bool>
        {
            Object = false
        };

        _displayStatusFilter = new ObservableObject<bool>
        {
            Object = true
        };

        _showActiveStations = new ObservableObject<bool>
        {
            Object = true
        };

        _showClosedStations = new ObservableObject<bool>
        {
            Object = false
        };

        _displayRetrospectionControls = new ObservableObject<bool>
        {
            Object = false
        };

        _displayHistoricalTimeControls = new ObservableObject<bool>
        {
            //Object = false
            Object = true
        };

        _displayDatabaseTimeControls = new ObservableObject<bool>
        {
            //Object = false
            Object = true
        };

        _displayHistoricalTimeControls.PropertyChanged += (s, e) =>
            UpdateRetrospectionControls();

        _displayDatabaseTimeControls.PropertyChanged += (s, e) =>
            UpdateRetrospectionControls();

        _showActiveStations.PropertyChanged += (s, e) =>
        {
            if (_autoRefresh.Object)
            {
                ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
            }
        };

        _showClosedStations.PropertyChanged += (s, e) =>
        {
            if (_autoRefresh.Object)
            {
                ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
            }
        };

        InitializeLogViewModel(_logger);
        InitializeObservingFacilityListViewModel(_logger, _unitOfWorkFactory, _applicationDialogService);
        InitializeObservingFacilitiesDetailsViewModel(_unitOfWorkFactory, _applicationDialogService);
        InitializeMapViewModel();
        InitializeDatabaseWriteTimesViewModel();
        InitializeHistoricalTimeViewModel();

        DrawMapOfDenmark();

        if (true)
        {
            try
            {
                using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
                {
                    _databaseWriteTimes = new List<DateTime>();

                    var observingFacilities = unitOfWork.ObservingFacilities.GetAll().ToList();
                    var timeStampsForObservingFacilities = observingFacilities.Select(_ => _.Created).ToList();
                    timeStampsForObservingFacilities.AddRange(observingFacilities.Select(_ => _.Superseded));
                    timeStampsForObservingFacilities = timeStampsForObservingFacilities.Where(_ => _ < DateTime.MaxValue).ToList();
                    _databaseWriteTimes.AddRange(timeStampsForObservingFacilities.Distinct());

                    var geospatialLocations = unitOfWork.GeospatialLocations.GetAll().ToList();
                    var timeStampsForGeospatialLocations = geospatialLocations.Select(_ => _.Created).ToList();
                    timeStampsForGeospatialLocations.AddRange(geospatialLocations.Select(_ => _.Superseded));
                    timeStampsForGeospatialLocations = timeStampsForGeospatialLocations.Where(_ => _ < DateTime.MaxValue).ToList();
                    _databaseWriteTimes.AddRange(timeStampsForGeospatialLocations.Distinct());

                    _databaseWriteTimes = _databaseWriteTimes.Distinct().ToList();

                    geospatialLocations = unitOfWork.GeospatialLocations
                        .Find(_ => _.Superseded == DateTime.MaxValue)
                        .ToList();

                    var historicalChangeTimeStamps = geospatialLocations.Select(_ => _.From).ToList();
                    historicalChangeTimeStamps.AddRange(geospatialLocations.Select(_ => _.To));

                    _historicalChangeTimes = historicalChangeTimeStamps
                        .Where(_ => _ < DateTime.MaxValue)
                        .Distinct()
                        .ToList();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Just swallow it for now - write it to the log later
                _databaseWriteTimes = new List<DateTime>();
                _historicalChangeTimes = new List<DateTime>();
            }
        }
        else
        {
            _databaseWriteTimes = new List<DateTime>();
            _historicalChangeTimes = new List<DateTime>();
        }

        UpdateRetrospectionControls();
        UpdateControlBackground();
        UpdateStatusBar();

        if (_autoRefresh.Object)
        {
            _logger.WriteLine(LogMessageCategory.Information, "Emulating click on Find button (1)");
            ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
        }

        _historicalTimeOfInterest.PropertyChanged += (s, e) =>
        {
            if (!_historicalTimeOfInterest.Object.HasValue)
            {
                HistoricalTimeViewModel.StaticXValue = null;
                DatabaseWriteTimesViewModel.LockWorldWindowOnDynamicXValue = true;
                DatabaseWriteTimesViewModel.StaticXValue = null;
            }
        };

        SelectedTabIndexForRetrospectionTimeLines = 1;
    }

    private void CreateObservingFacility(
        object owner)
    {
        _owner = owner as Window;
        _mapOperation = MapOperation.CreateObservingFacility;
        MessageInMap = "Click the map to place new observing facility";
        DisplayMessageInMap = true;
    }

    private void CreateGeospatialLocation(
        object owner)
    {
        _owner = owner as Window;
        _mapOperation = MapOperation.CreateGeospatialLocation;
        MessageInMap = "Click the map to indicate new position of observing facility";
        DisplayMessageInMap = true;
    }

    private bool CanCreateObservingFacility(
        object owner)
    {
        return !_databaseTimeOfInterest.Object.HasValue;
    }

    private void DeleteSelectedObservingFacilities(
        object owner)
    {
        var nSelectedObservingFacilities = ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Count();

        var message = nSelectedObservingFacilities == 1
            ? "Delete Observing Facility?"
            : $"Delete {nSelectedObservingFacilities} Observing Facilities?";

        var dialogViewModel = new MessageBoxDialogViewModel(message, true);

        if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) == DialogResult.Cancel)
        {
            return;
        }

        var objectIds = ObservingFacilityListViewModel.SelectedObservingFacilities.Objects
            .Select(_ => _.ObjectId)
            .ToList();

        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            // Todo: Remember to also delete orphaned children

            var observingFacilitiesForDeletion = unitOfWork.ObservingFacilities
                .Find(_ => _.Superseded == DateTime.MaxValue && objectIds.Contains(_.ObjectId))
                .ToList();

            var now = DateTime.UtcNow;

            observingFacilitiesForDeletion.ForEach(_ => _.Superseded = now);

            unitOfWork.ObservingFacilities.UpdateRange(observingFacilitiesForDeletion);
            unitOfWork.Complete();

            ObservingFacilityListViewModel.RemoveObservingFacilities(observingFacilitiesForDeletion);

            _databaseWriteTimes.Add(now);
            RefreshDatabaseTimeSeriesView();
            RefreshHistoricalTimeSeriesView(true);
        }
    }

    private bool CanDeleteSelectedObservingFacilities(
        object owner)
    {
        return !_databaseTimeOfInterest.Object.HasValue &&
               ObservingFacilityListViewModel.SelectedObservingFacilities.Objects != null &&
               ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any();
    }

    private void ClearRepository(
        object owner)
    {
        var dialogViewModel1 = new MessageBoxDialogViewModel("Clear repository?", true);

        if (_applicationDialogService.ShowDialog(dialogViewModel1, owner as Window) == DialogResult.Cancel)
        {
            return;
        }

        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            unitOfWork.GeospatialLocations.Clear();
            unitOfWork.AbstractEnvironmentalMonitoringFacilities.Clear();
            unitOfWork.Complete();
        }

        _historicalChangeTimes.Clear();
        RefreshHistoricalTimeSeriesView(false);

        _databaseWriteTimes.Clear();
        RefreshDatabaseTimeSeriesView();

        _logger.WriteLine(LogMessageCategory.Information, "Emulating click on Find button (2)");
        ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);

        var dialogViewModel2 = new MessageBoxDialogViewModel("Repository was cleared", false);

        _applicationDialogService.ShowDialog(dialogViewModel2, owner as Window);
    }

    private bool CanClearRepository(
        object owner)
    {
        return true;
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
        ILogger logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService)
    {
        ObservingFacilityListViewModel = new ObservingFacilityListViewModel(
            logger,
            unitOfWorkFactory,
            applicationDialogService,
            _historicalTimeOfInterest,
            _databaseTimeOfInterest,
            _autoRefresh,
            _displayNameFilter,
            _displayStatusFilter,
            _showActiveStations,
            _showClosedStations,
            _displayHistoricalTimeControls,
            _displayDatabaseTimeControls);

        ObservingFacilityListViewModel.SelectedObservingFacilities.PropertyChanged += (s, e) =>
        {
            DeleteSelectedObservingFacilitiesCommand.RaiseCanExecuteChanged();
        };

        ObservingFacilityListViewModel.ObservingFacilityDataExtracts.PropertyChanged += (s, e) =>
        {
            _logger?.WriteLine(LogMessageCategory.Information, "Updating Map points");
            UpdateMapPoints();
        };
    }

    private void InitializeObservingFacilitiesDetailsViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService)
    {
        ObservingFacilitiesDetailsViewModel = new ObservingFacilitiesDetailsViewModel(
            unitOfWorkFactory,
            applicationDialogService,
            _databaseTimeOfInterest,
            ObservingFacilityListViewModel.SelectedObservingFacilities);

        ObservingFacilitiesDetailsViewModel.ObservingFacilitiesUpdated += (s, e) =>
        {
            ObservingFacilityListViewModel.UpdateObservingFacilities(e.ObservingFacilities);
            _databaseWriteTimes.Add(e.ObservingFacilities.First().Created);
            RefreshDatabaseTimeSeriesView();
        };

        ObservingFacilitiesDetailsViewModel.GeospatialLocationsViewModel.NewGeospatialLocationCalledByUser += (s, e) =>
        {
            CreateGeospatialLocation(e.Owner);
        };

        ObservingFacilitiesDetailsViewModel.GeospatialLocationsViewModel.GeospatialLocationsUpdatedOrDeleted += (s, e) =>
        {
            // Todo: consider placing this in a general helper method
            _databaseWriteTimes.Add(e.DateTime);
            ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
            if (ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any())
            {
                ObservingFacilitiesDetailsViewModel.GeospatialLocationsViewModel.Populate();
            }
            UpdateMapPoints();
            RefreshDatabaseTimeSeriesView();
            RefreshHistoricalTimeSeriesView(true);
        };
    }

    private void InitializeMapViewModel()
    {
        var worldWindowBoundingBoxNorthWest = new Point(6.5, 57.95);
        var worldWindowBoundingBoxSouthEast = new Point(15.5, 54.45);

        var worldWindowFocus = new Point(
            (worldWindowBoundingBoxNorthWest.X + worldWindowBoundingBoxSouthEast.X) / 2,
            (worldWindowBoundingBoxNorthWest.Y + worldWindowBoundingBoxSouthEast.Y) / 2);

        var worldWindowSize = new Size(
            Math.Abs(worldWindowBoundingBoxNorthWest.X - worldWindowBoundingBoxSouthEast.X),
            Math.Abs(worldWindowBoundingBoxNorthWest.Y - worldWindowBoundingBoxSouthEast.Y));

        MapViewModel = new GeometryEditorViewModel(-1)
        {
            AspectRatioLocked = true
        };

        MapViewModel.InitializeWorldWindow(worldWindowFocus, worldWindowSize, false);

        MapViewModel.MouseClickOccured += (s, e) =>
        {
            if (!DisplayMessageInMap || !MapViewModel.MousePositionWorld.Object.HasValue)
            {
                return;
            }

            DisplayMessageInMap = false;

            switch (_mapOperation)
            {
                case MapOperation.CreateObservingFacility:
                {
                    CreateNewObservingFacility();
                    break;
                }
                case MapOperation.CreateGeospatialLocation:
                {
                    CreateNewGeospatialLocation();
                    break;
                }
            }
        };

        UpdateMapColoring();
    }

    private void InitializeHistoricalTimeViewModel()
    {
        var timeSpan = TimeSpan.FromDays(40);
        var utcNow = DateTime.UtcNow;
        var timeAtOrigo = utcNow.Date;
        var tFocus = utcNow - timeSpan / 2 + TimeSpan.FromDays(2);
        var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);

        HistoricalTimeViewModel = new TimeSeriesViewModel(
            new Point(xFocus, 0),
            new Size(timeSpan.TotalDays, 3),
            true,
            0,
            40,
            1,
            timeAtOrigo,
            _logger)
        {
            LockWorldWindowOnDynamicXValue = true,
            ShowHorizontalGridLines = false,
            ShowVerticalGridLines = false,
            ShowHorizontalAxis = true,
            ShowVerticalAxis = false,
            ShowXAxisLabels = true,
            ShowYAxisLabels = false,
            ShowPanningButtons = true,
            Fraction = 0.95,
            LabelForDynamicXValue = "Now"
        };

        HistoricalTimeViewModel.GeometryEditorViewModel.YAxisLocked = true;

        HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
        {
            // Når brugeren dragger, træder vi ud af det mode, hvor World Window løbende opdateres
            HistoricalTimeViewModel.LockWorldWindowOnDynamicXValue = false;
        };

        HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
        {
            RefreshHistoricalTimeSeriesView(false);
        };

        HistoricalTimeViewModel.GeometryEditorViewModel.UpdateModelCallBack = () =>
        {
            // Update the x value of interest (set it to current time)
            var nowAsScalar = (DateTime.UtcNow - HistoricalTimeViewModel.TimeAtOrigo).TotalDays;
            HistoricalTimeViewModel.DynamicXValue = nowAsScalar;
        };

        HistoricalTimeViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
        {
            if (_databaseTimeOfInterest.Object.HasValue &&
                HistoricalTimeViewModel.TimeAtMousePosition.Object.Value > _databaseTimeOfInterest.Object)
            {
                var message = "Historical time of interest cannot be later than database time of interest";
                var dialogViewModel = new MessageBoxDialogViewModel(message, false);

                _applicationDialogService.ShowDialog(dialogViewModel, _owner);

                return;
            }

            if (HistoricalTimeViewModel.TimeAtMousePosition.Object > DateTime.UtcNow)
            {
                return;
            }

            _historicalTimeOfInterest.Object = HistoricalTimeViewModel.TimeAtMousePosition.Object;

            // Highlight the position of the time of interest
            //HistoricalTimeViewModel.StaticXValue =
            //    (_historicalTimeOfInterest.Object.Value - HistoricalTimeViewModel.TimeAtOrigo) / TimeSpan.FromDays(1);
        };
    }

    private void InitializeDatabaseWriteTimesViewModel()
    {
        var timeSpan = TimeSpan.FromMinutes(5);
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
            1,
            timeAtOrigo,
            _logger)
        {
            LockWorldWindowOnDynamicXValue = true,
            ShowHorizontalGridLines = false,
            ShowVerticalGridLines = false,
            ShowHorizontalAxis = true,
            ShowVerticalAxis = false,
            ShowXAxisLabels = true,
            ShowYAxisLabels = false,
            ShowPanningButtons = true,
            Fraction = 0.95,
            LabelForDynamicXValue = "Now"
        };

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.YAxisLocked = true;

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
        {
            // Når brugeren dragger, træder vi ud af det mode, hvor World Window løbende opdateres
            DatabaseWriteTimesViewModel.LockWorldWindowOnDynamicXValue = false;
        };

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
        {
            RefreshDatabaseTimeSeriesView();
        };

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.UpdateModelCallBack = () =>
        {
            // Update the x value of interest (set it to current time)
            // Dette udvirker, at World Window følger med current time
            var nowAsScalar = (DateTime.UtcNow - DatabaseWriteTimesViewModel.TimeAtOrigo).TotalDays;
            DatabaseWriteTimesViewModel.DynamicXValue = nowAsScalar;
        };

        DatabaseWriteTimesViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
        {
            if (DatabaseWriteTimesViewModel.TimeAtMousePosition.Object > DateTime.UtcNow)
            {
                return;
            }

            _databaseTimeOfInterest.Object = DatabaseWriteTimesViewModel.TimeAtMousePosition.Object;

            // Highlight the position of the time of interest
            DatabaseWriteTimesViewModel.StaticXValue = 
                (_databaseTimeOfInterest.Object.Value - DatabaseWriteTimesViewModel.TimeAtOrigo) / TimeSpan.FromDays(1);

            // Der skal altid gælde, at historisk tid er ældre end eller lig med databasetid
            if (!_historicalTimeOfInterest.Object.HasValue ||
                _historicalTimeOfInterest.Object.Value > _databaseTimeOfInterest.Object.Value)
            {
                _historicalTimeOfInterest.Object = _databaseTimeOfInterest.Object.Value;
            }
        };

        DatabaseWriteTimesViewModel.PanLeftClicked += (s, e) =>
        {
            DatabaseWriteTimesViewModel.LockWorldWindowOnDynamicXValue = false;

            // Identify the database write time that is to the left of the current focus

            var currentTimeInFocus = DatabaseWriteTimesViewModel.TimeAtOrigo +
                TimeSpan.FromDays(DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus.X);

            // Af hensyn til afrundingsfejl, så vi sikrer, at den faktisk stepper tilbage i tid
            currentTimeInFocus -= TimeSpan.FromMilliseconds(10);

            var earlierDatabaseWriteTimes = _databaseWriteTimes.Where(_ => _ < currentTimeInFocus);

            if (!earlierDatabaseWriteTimes.Any())
            {
                return;
            }

            var newTimeInFocus = earlierDatabaseWriteTimes.Max();
            var xValueInFocus = (newTimeInFocus - DatabaseWriteTimesViewModel.TimeAtOrigo) / TimeSpan.FromDays(1);

            DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus = new Point(
                xValueInFocus,
                DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus.Y);
        };

        DatabaseWriteTimesViewModel.PanRightClicked += (s, e) =>
        {
            // Identify the database write time that is to the right of the current focus

            var currentTimeInFocus = DatabaseWriteTimesViewModel.TimeAtOrigo +
                                     TimeSpan.FromDays(DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus.X);

            // Af hensyn til afrundingsfejl, så vi sikrer, at den faktisk stepper frem i tid
            currentTimeInFocus += TimeSpan.FromMilliseconds(10);

            var laterDatabaseWriteTimes = _databaseWriteTimes.Where(_ => _ > currentTimeInFocus);

            if (!laterDatabaseWriteTimes.Any())
            {
                DatabaseWriteTimesViewModel.LockWorldWindowOnDynamicXValue = true;
                return;
            }

            var newTimeInFocus = laterDatabaseWriteTimes.Min();
            var xValueInFocus = (newTimeInFocus - DatabaseWriteTimesViewModel.TimeAtOrigo) / TimeSpan.FromDays(1);

            DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus = new Point(
                xValueInFocus,
                DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowFocus.Y);
        };
    }

    private void RefreshHistoricalTimeSeriesView(
        bool recalculate)
    {
        // Called:
        //   - During upstart (ok)
        //   - When a major world window update occurs (such as after a drag) (ok)
        //   - When the user changes the historical time of interest by clicking in the view (ok)

        //   - When a new observing facility is created
        //   - When selected observing facilities are deleted
        //   - When the user resets the historical time of interest by clicking the Now button

        // Calculate position of world window
        var x0 = HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X;
        var x1 = HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X + HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowSize.Width;
        var y0 = -HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y - HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowSize.Height;
        var y1 = -HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y;

        // Calculate y coordinate of the principal axis (so we can make the lines stop there)
        var y2 = HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y +
                 HistoricalTimeViewModel.GeometryEditorViewModel.WorldWindowSize.Height * HistoricalTimeViewModel.GeometryEditorViewModel.MarginBottomOffset /
                 HistoricalTimeViewModel.GeometryEditorViewModel.ViewPortSize.Height;

        // Clear lines
        HistoricalTimeViewModel.GeometryEditorViewModel.ClearLines();

        var lineThickness = 1.5;

        var lineViewModels = _historicalChangeTimes
            .Select(_ => (_ - HistoricalTimeViewModel.TimeAtOrigo).TotalDays)
            .Where(_ => _ > x0 && _ < x1)
            .Select(_ => new LineViewModel(new PointD(_, y0), new PointD(_, y2), lineThickness, _timeStampBrush))
            .ToList();

        lineViewModels.ForEach(_ => HistoricalTimeViewModel.GeometryEditorViewModel.LineViewModels.Add(_));

        if (!_historicalTimeOfInterest.Object.HasValue)
        {
            HistoricalTimeViewModel.StaticXValue = null;
        }
    }

    private void RefreshDatabaseTimeSeriesView()
    {
        // Called:
        //   - During upstart
        //   - When a new observing facility is created
        //   - When selected observing facilities are updated
        //   - When selected observing facilities are deleted
        //   - When a major world window update occurs (such as after a drag)
        //   - When the user changes the database time of interest by clicking in the view
        //   - When the user resets the database time of interest by clicking the Latest button

        // Calculate position of world window
        var x0 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X;
        var x1 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X + DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Width;
        var y0 = -DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y - DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Height;
        var y1 = -DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y;

        // Calculate y coordinate of the principal axis (so we can make the lines stop there)
        var y2 = DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.Y +
             DatabaseWriteTimesViewModel.GeometryEditorViewModel.WorldWindowSize.Height * DatabaseWriteTimesViewModel.GeometryEditorViewModel.MarginBottomOffset /
             DatabaseWriteTimesViewModel.GeometryEditorViewModel.ViewPortSize.Height;

        // Clear lines
        DatabaseWriteTimesViewModel.GeometryEditorViewModel.ClearLines();

        var lineThickness = 1.5;

        var lineViewModels = _databaseWriteTimes
            .Select(_ => (_ - DatabaseWriteTimesViewModel.TimeAtOrigo).TotalDays)
            .Where(_ => _ > x0 && _ < x1)
            .Select(_ => new LineViewModel(new PointD(_, y0), new PointD(_, y2), lineThickness, _timeStampBrush))
            .ToList();

        lineViewModels.ForEach(_ => DatabaseWriteTimesViewModel.GeometryEditorViewModel.LineViewModels.Add(_));

        if (!_databaseTimeOfInterest.Object.HasValue)
        {
            DatabaseWriteTimesViewModel.StaticXValue = null;
        }
    }

    private void DrawMapOfDenmark()
    {
        // Load GML file of Denmark
        var fileName = @".\Data\Denmark.gml";
        //var fileName = @".\Data\DenmarkAndGreenland.gml";
        DataIOHandler.ExtractGeometricPrimitivesFromGMLFile(fileName, out var polygons);

        // Add the regions of Denmark to the map as polygons
        var lineThickness = 0.005;

        foreach (var polygon in polygons)
        {
            MapViewModel.AddPolygon(polygon
                .Select(p => new PointD(p[1], p[0])), lineThickness, _mapBrushLandCurrent);
        }
    }

    private void UpdateMapPoints()
    {
        MapViewModel.PointViewModels.Clear();

        foreach (var observingFacilityDataExtract in ObservingFacilityListViewModel.ObservingFacilityDataExtracts.Objects)
        {
            var relevantGeospatialLocations = _historicalTimeOfInterest.Object.HasValue
                ? observingFacilityDataExtract.GeospatialLocations
                    .Where(_ => _.From < _historicalTimeOfInterest.Object.Value)
                : observingFacilityDataExtract.GeospatialLocations;

            var latestGeospatialLocationAmongRelevantOnes = relevantGeospatialLocations
                .OrderByDescending(_ => _.From)
                .First() as Domain.Entities.WIGOS.GeospatialLocations.Point;

            var brush = _activeObservingFacilityBrush;

            if (_historicalTimeOfInterest.Object.HasValue)
            {
                if (_historicalTimeOfInterest.Object.Value > latestGeospatialLocationAmongRelevantOnes.To)
                {
                    brush = _closedObservingFacilityBrush;
                }
            }
            else if (latestGeospatialLocationAmongRelevantOnes.To < DateTime.MaxValue)
            {
                brush = _closedObservingFacilityBrush;
            }

            MapViewModel.PointViewModels.Add(new PointViewModel(
                new PointD(
                    latestGeospatialLocationAmongRelevantOnes.Coordinate1, 
                    -latestGeospatialLocationAmongRelevantOnes.Coordinate2), 
                10, 
                brush));


            // Old
            //observingFacilityDataExtract.GeospatialLocations.ForEach(_ =>
            //{
            //    if (_ is not Domain.Entities.WIGOS.GeospatialLocations.Point point)
            //    {
            //        // Forekommer ikke i praksis
            //        return;
            //    }

            //    var brush = _activeObservingFacilityBrush;

            //    if (_historicalTimeOfInterest.Object.HasValue)
            //    {
            //        if (_historicalTimeOfInterest.Object.Value < _.From ||
            //            _historicalTimeOfInterest.Object.Value >= _.To)
            //        {
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        if (_.To < DateTime.MaxValue)
            //        {
            //            brush = _closedObservingFacilityBrush;
            //        }
            //    }

            //    MapViewModel.PointViewModels.Add(new PointViewModel(
            //        new PointD(point.Coordinate1, -point.Coordinate2), 10, brush));
            //});
        }
    }

    private void UpdateMapColoring()
    {
        if (_historicalTimeOfInterest.Object.HasValue ||
            _databaseTimeOfInterest.Object.HasValue)
        {
            MapViewModel.BackgroundBrush = _mapBrushSeaHistoric;

            foreach (var polygonViewModel in MapViewModel.PolygonViewModels)
            {
                polygonViewModel.Brush = _mapBrushLandHistoric;
            }
        }
        else
        {
            MapViewModel.BackgroundBrush = _mapBrushSeaCurrent;

            foreach (var polygonViewModel in MapViewModel.PolygonViewModels)
            {
                polygonViewModel.Brush = _mapBrushLandCurrent;
            }
        }

        if (_autoRefresh.Object)
        {
            _logger.WriteLine(LogMessageCategory.Information, "Emulating click on Find button (3)");
            ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
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

    private void UpdateControlBackground()
    {
        ControlBackgroundBrush = _historicalTimeOfInterest.Object.HasValue
            ? _controlBackgroundBrushHistoric
            : _controlBackgroundBrushCurrent;
    }

    private void UpdateStatusBar()
    {
        if (!_historicalTimeOfInterest.Object.HasValue)
        {
            StatusBarText = "Current situation";
        }
        else if (!_databaseTimeOfInterest.Object.HasValue)
        {
            StatusBarText = $"Historical situation of {_historicalTimeOfInterest.Object.Value.AsDateString()}";
        }
        else if (_historicalTimeOfInterest.Object.Value == _databaseTimeOfInterest.Object.Value)
        {
            StatusBarText = $"Historical situation of {_historicalTimeOfInterest.Object.Value.AsDateString()} as depicted by the database at that time";
        }
        else
        {
            StatusBarText = $"Historical situation of {_historicalTimeOfInterest.Object.Value.AsDateString()} as depicted by the database as of {_databaseTimeOfInterest.Object.Value.AsDateTimeString(false)}";
        }
    }

    private void UpdateCommands()
    {
        CreateObservingFacilityCommand.RaiseCanExecuteChanged();
        DeleteSelectedObservingFacilitiesCommand.RaiseCanExecuteChanged();
        ObservingFacilitiesDetailsViewModel.IsReadOnly = _databaseTimeOfInterest.Object.HasValue;
    }

    private void CreateNewObservingFacility()
    {
        var dialogViewModel = new CreateObservingFacilityDialogViewModel(MapViewModel.MousePositionWorld.Object.Value);

        if (_applicationDialogService.ShowDialog(dialogViewModel, _owner) != DialogResult.OK)
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

        var now = DateTime.UtcNow;

        // Bemærk, at vi sætter DateEstablished og DateClosed svarende til From og To for den ene lokation, som
        // den pågældende observing facility laves med
        var observingFacility = new ObservingFacility(
            Guid.NewGuid(),
            now)
        {
            Name = dialogViewModel.Name,
            DateEstablished = from,
            DateClosed = to
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

        if (!_historicalChangeTimes.Contains(observingFacility.DateEstablished))
        {
            _historicalChangeTimes.Add(observingFacility.DateEstablished);
        }

        if (observingFacility.DateClosed < DateTime.MaxValue)
        {
            if (!_historicalChangeTimes.Contains(observingFacility.DateClosed))
            {
                _historicalChangeTimes.Add(observingFacility.DateClosed);
            }
        }

        RefreshHistoricalTimeSeriesView(false);

        _databaseWriteTimes.Add(now);
        RefreshDatabaseTimeSeriesView();

        if (_autoRefresh.Object)
        {
            ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);
        }
    }

    private void CreateNewGeospatialLocation()
    {
        var mousePositionInMap = MapViewModel.MousePositionWorld.Object.Value;

        var dialogViewModel = new DefineGeospatialLocationDialogViewModel(
            DefineGeospatialLocationMode.Create,
            Math.Round(mousePositionInMap.X, 4),
            -Math.Round(mousePositionInMap.Y, 4),
            DateTime.UtcNow.Date,
            null);

        if (_applicationDialogService.ShowDialog(dialogViewModel, _owner) != DialogResult.OK)
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

        var selectedObservingFacility = ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Single();

        var now = DateTime.UtcNow;

        using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
        {
            var point = new Domain.Entities.WIGOS.GeospatialLocations.Point(Guid.NewGuid(), now)
            {
                From = from,
                To = to,
                Coordinate1 = double.Parse(dialogViewModel.Latitude, CultureInfo.InvariantCulture),
                Coordinate2 = double.Parse(dialogViewModel.Longitude, CultureInfo.InvariantCulture),
                CoordinateSystem = "WGS_84",
                AbstractEnvironmentalMonitoringFacilityId = selectedObservingFacility.Id,
                AbstractEnvironmentalMonitoringFacilityObjectId = selectedObservingFacility.ObjectId
            };

            unitOfWork.Points_Wigos.Add(point);

            if (point.From < selectedObservingFacility.DateEstablished ||
                point.To > selectedObservingFacility.DateClosed)
            {
                var observingFacilityFromRepo = unitOfWork.ObservingFacilities.Get(selectedObservingFacility.Id);

                observingFacilityFromRepo.Superseded = now;
                unitOfWork.ObservingFacilities.Update(observingFacilityFromRepo);

                var newObservingFacility = new ObservingFacility(Guid.NewGuid(), now)
                {
                    ObjectId = observingFacilityFromRepo.ObjectId,
                    Name = observingFacilityFromRepo.Name,
                    DateEstablished = point.From < observingFacilityFromRepo.DateEstablished 
                        ? point.From 
                        : observingFacilityFromRepo.DateEstablished,
                    DateClosed = point.To > observingFacilityFromRepo.DateClosed
                        ? point.To
                        : observingFacilityFromRepo.DateClosed
                };

                unitOfWork.ObservingFacilities.Add(newObservingFacility);
            }

            unitOfWork.Complete();
        }

        _databaseWriteTimes.Add(now);
        ObservingFacilityListViewModel.FindObservingFacilitiesCommand.Execute(null);

        if (ObservingFacilityListViewModel.SelectedObservingFacilities.Objects.Any())
        {
            ObservingFacilitiesDetailsViewModel.GeospatialLocationsViewModel.Populate();
        }

        UpdateMapPoints();
        RefreshDatabaseTimeSeriesView();
    }
}