﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Tasks;
using DMI.SMS.ViewModel;
using DMI.SMS.Domain.Entities;
using DMI.StatDB.ViewModel;
using DMI.StatDB.Domain.Entities;
using DMI.SMS.Application;
using System.Configuration;

namespace DMI.Data.Studio.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Deprecated
        private Dictionary<SMS.Application.RowCondition, Brush> _rowConditionToBrushMap =
            new()
            {
                { SMS.Application.RowCondition.Current, new SolidColorBrush(Colors.DarkGreen) },
                { SMS.Application.RowCondition.OutDated, new SolidColorBrush(Colors.PaleGoldenrod) },
                { SMS.Application.RowCondition.Deleted, new SolidColorBrush(Colors.DarkRed) }
            };

        private readonly StatDB.Application.IUIDataProvider _statDBDataProvider;
        private readonly SMS.Persistence.IUnitOfWorkFactory _smsDBUnitOfWorkFactory;
        private readonly ObsDB.Persistence.IUnitOfWorkFactory _obsDBUnitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;
        private readonly ILogger _logger;
        private List<StationInformation> _selectedStationInformations;
        private List<Station> _selectedStations;
        private bool _includeOperationIntervalBars;
        private bool _includeObservationIntervalBars;
        private bool _includeTransactionTimeIntervalBars;
        private bool _includeDetailedTimeSeriesView;
        private bool _showSMSDBList;
        private bool _showStatDBList;
        private bool _autoPopulateStatDBStationList;
        private RelayCommand<object> _openSettingsDialogCommand;
        private RelayCommand<object> _openAboutDialogCommand;
        private RelayCommand<object> _createStationInformationCommand;
        private AsyncCommand<object> _exportDataCommand;
        private AsyncCommand<object> _importDataCommand;
        private AsyncCommand<object> _clearRepositoryCommand;
        private AsyncCommand<object> _extractMeteorologicalStationListCommand;
        private AsyncCommand<object> _extractOceanographicalStationListCommand;
        private Brush _stationInformationBrush = new SolidColorBrush(Colors.DarkRed);
        private Brush _stationBrush = new SolidColorBrush(Colors.DarkOrange);
        private Brush _StatDBTimeIntervalBrush = new SolidColorBrush(Colors.DarkSlateGray);
        private Brush _observationTimeIntervalBrush = new SolidColorBrush(Colors.Orange);
        private Brush _transactionTimeIntervalBrush = new SolidColorBrush(Colors.DarkSlateGray);
        private Brush _stationIdLabelBrush = new SolidColorBrush(Colors.Black);
        private Brush _positionDifferenceLabelBackgroundBrushDefault = new SolidColorBrush(Colors.White);
        private Brush _positionDifferenceLabelBackgroundBrushHighlight = new SolidColorBrush(Colors.Red);
        private Brush _positionDifferenceLabelForegroundBrush = new SolidColorBrush(Colors.Black);
        private bool _classifyRecordsWithCondition;
        private readonly ObservableObject<bool> _observableForClassifyRecordsWithCondition;
        private int _selectedOveralTabIndex;
        private int _selectedDetailTabIndex;

        private Application.Application _application;
        private SMS.Application.Application _smsApplication;

        private string _mainWindowTitle;

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public bool ClassifyRecordsWithCondition
        {
            get { return _classifyRecordsWithCondition; }
            set
            {
                _classifyRecordsWithCondition = value;
                RaisePropertyChanged();

                _observableForClassifyRecordsWithCondition.Object = value;
            }
        }

        public LogViewModel LogViewModel { get; }
        public TaskViewModel TaskViewModel { get; }
        public StationInformationListViewModel StationInformationListViewModel { get; private set; }
        public StationInformationDetailsViewModel StationInformationDetailsViewModel { get; private set; }
        public StationListViewModel StationListViewModel { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; private set; }
        public Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel ChronologyViewModel { get; private set; }
        public StatisticsViewModel StatisticsViewModel { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel { get; private set; }

        public bool IncludeOperationIntervalBars
        {
            get { return _includeOperationIntervalBars; }
            set
            {
                _includeOperationIntervalBars = value;
                RaisePropertyChanged();

                UpdateChronologyView();
            }
        }

        public bool IncludeObservationIntervalBars
        {
            get { return _includeObservationIntervalBars; }
            set
            {
                _includeObservationIntervalBars = value;
                RaisePropertyChanged();

                UpdateChronologyView();
            }
        }

        public bool IncludeTransactionTimeIntervalBars
        {
            get { return _includeTransactionTimeIntervalBars; }
            set
            {
                _includeTransactionTimeIntervalBars = value;
                RaisePropertyChanged();
                UpdateChronologyView();
            }
        }

        public bool IncludeDetailedTimeSeriesView
        {
            get { return _includeDetailedTimeSeriesView; }
            set
            {
                _includeDetailedTimeSeriesView = value;
                ChronologyViewModel.GeometryEditorViewModel.SelectRegionPossible = _includeDetailedTimeSeriesView;
                TimeSeriesViewModel.Active = _includeDetailedTimeSeriesView;

                if (!_includeDetailedTimeSeriesView)
                {
                    SelectedDetailTabIndex = 0;
                }

                RaisePropertyChanged();
                UpdateChronologyView();
            }
        }

        public bool ShowSMSDBList
        {
            get { return _showSMSDBList; }
            set
            {
                _showSMSDBList = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowStatDBList
        {
            get { return _showStatDBList; }
            set
            {
                _showStatDBList = value;
                RaisePropertyChanged();
            }
        }

        public bool AutoPopulateStatDBStationList
        {
            get { return _autoPopulateStatDBStationList; }
            set
            {
                _autoPopulateStatDBStationList = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedOveralTabIndex
        {
            get { return _selectedOveralTabIndex; }
            set
            {
                _selectedOveralTabIndex = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedDetailTabIndex
        {
            get { return _selectedDetailTabIndex; }
            set
            {
                _selectedDetailTabIndex = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> OpenSettingsDialogCommand
        {
            get { return _openSettingsDialogCommand ?? (_openSettingsDialogCommand = new RelayCommand<object>(OpenSettingsDialog)); }
        }

        public RelayCommand<object> OpenAboutDialogCommand
        {
            get { return _openAboutDialogCommand ?? (_openAboutDialogCommand = new RelayCommand<object>(OpenAboutDialog)); }
        }

        public RelayCommand<object> CreateStationInformationCommand
        {
            get { return _createStationInformationCommand ?? (_createStationInformationCommand = new RelayCommand<object>(CreateStationInformation)); }
        }

        public AsyncCommand<object> ExportDataCommand
        {
            get { return _exportDataCommand ?? (_exportDataCommand = new AsyncCommand<object>(ExportData, CanExportData)); }
        }

        public AsyncCommand<object> ExtractMeteorologicalStationListCommand
        {
            get { return _extractMeteorologicalStationListCommand ?? (
                    _extractMeteorologicalStationListCommand = new AsyncCommand<object>(ExtractMeteorologicalStationList)); }
        }

        public AsyncCommand<object> ExtractOceanographicalStationListCommand
        {
            get
            {
                return _extractOceanographicalStationListCommand ?? (
                  _extractOceanographicalStationListCommand = new AsyncCommand<object>(ExtractOceanographicalStationList));
            }
        }

        public AsyncCommand<object> ImportDataCommand
        {
            get { return _importDataCommand ?? (_importDataCommand = new AsyncCommand<object>(ImportData)); }
        }

        public AsyncCommand<object> ClearRepositoryCommand
        {
            get { return _clearRepositoryCommand ?? (_clearRepositoryCommand = new AsyncCommand<object>(ClearRepository)); }
        }

        // Parametrene her fås ved dependency injection
        public MainWindowViewModel(
            SMS.Persistence.IUnitOfWorkFactory smsUnitOfWorkFactory,
            SMS.IO.IDataIOHandler smsDataIOHandler,
            StatDB.Application.IUIDataProvider statDBDataProvider,
            ObsDB.Persistence.IUnitOfWorkFactory obsDBUnitOfWorkFactory,
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _statDBDataProvider = statDBDataProvider;
            _smsDBUnitOfWorkFactory = smsUnitOfWorkFactory;
            _obsDBUnitOfWorkFactory = obsDBUnitOfWorkFactory;
            _applicationDialogService = applicationDialogService;
            _logger = logger;

            _application = new Application.Application(
                smsUnitOfWorkFactory,
                _obsDBUnitOfWorkFactory,
                _logger);

            _smsApplication = new SMS.Application.Application(
                smsUnitOfWorkFactory,
                smsDataIOHandler,
                _logger);

            _mainWindowTitle = "DMI Data Studio";

            _observableForClassifyRecordsWithCondition = new ObservableObject<bool>();
            _observableForClassifyRecordsWithCondition.Object = true;
            _selectedOveralTabIndex = 0;

            LogViewModel = new LogViewModel(200);

            TaskViewModel = new TaskViewModel();

            StationInformationListViewModel = new StationInformationListViewModel(
                smsUnitOfWorkFactory, 
                _applicationDialogService,
                _observableForClassifyRecordsWithCondition);

            StationListViewModel = new StationListViewModel(
                statDBDataProvider,
                applicationDialogService);

            StatisticsViewModel = new StatisticsViewModel(
                StationInformationListViewModel.StationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            var worldWindowBoundingBoxNorthWest = new Point(10.25, 57.95);
            var worldWindowBoundingBoxSouthEast = new Point(13.75, 54.45);

            var worldWindowFocus = new Point(
                (worldWindowBoundingBoxNorthWest.X + worldWindowBoundingBoxSouthEast.X) / 2,
                (worldWindowBoundingBoxNorthWest.Y + worldWindowBoundingBoxSouthEast.Y) / 2);

            var worldWindowSize = new Size(
                Math.Abs(worldWindowBoundingBoxNorthWest.X - worldWindowBoundingBoxSouthEast.X),
                Math.Abs(worldWindowBoundingBoxNorthWest.Y - worldWindowBoundingBoxSouthEast.Y));

            GeometryEditorViewModel = new GeometryEditorViewModel(-1);
            GeometryEditorViewModel.InitializeWorldWindow(worldWindowFocus, worldWindowSize, false);

            var years = 10;
            var timeSpan = TimeSpan.FromDays(365.25 * years);
            var tFocus = DateTime.UtcNow - timeSpan / 2;
            var xFocus = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tFocus);

            ChronologyViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 0),
                new Size(timeSpan.TotalDays, 1),
                true,
                40,
                40,
                1,
                XAxisMode.CustomTickLabels,
                null)
            {
                LockWorldWindowOnDynamicXValue = false,
                ShowXAxisLabels = true,
                ShowYAxisLabels = true,
                ShowVerticalGridLines = false,
                ShowHorizontalGridLines = false,
                ShowVerticalAxis = true,
                Fraction = 0.9,
                ShowPanningButtons = false,
            };

            ChronologyViewModel.GeometryEditorViewModel.YScalingLocked = true;
            ChronologyViewModel.GeometryEditorViewModel.SelectRegionPossible = true;
            ChronologyViewModel.GeometryEditorViewModel.SelectedRegionLimitedVertically = false;
            ChronologyViewModel.GeometryEditorViewModel.SelectRegionPossible = IncludeDetailedTimeSeriesView;
            ChronologyViewModel.GeometryEditorViewModel.InitializeWorldWindow(new Size(0.1, 1), new Point(xFocus, 0)); 

            var tMin = new DateTime(1950, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tMax = new DateTime(2050, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var xMin = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tMin);
            var xMax = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tMax);

            ChronologyViewModel.GeometryEditorViewModel.WorldWindowUpperLeftLimit = new Point(xMin, 0);
            ChronologyViewModel.GeometryEditorViewModel.WorldWindowBottomRightLimit = new Point(xMax, 10000);

            ChronologyViewModel.GeometryEditorViewModel.SelectedRegion.PropertyChanged += (s, e) =>
            {
                if (ChronologyViewModel.GeometryEditorViewModel.SelectedRegion.Object == null)
                {
                    return;
                }

                var timeWindow = TimeSpan.FromDays(365);
                var utcNow = DateTime.UtcNow;
                var tFocus = utcNow.Date - timeWindow / 2;
                var xFocus = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tFocus);

                var bb = ChronologyViewModel.GeometryEditorViewModel.SelectedRegion.Object;

                var focus = new Point(
                    bb.Left + bb.Width / 2,
                    10);

                var size = new Size(bb.Width, 30);
                var fitAspectRatio = true;

                if (!TimeViewsInSync())
                {
                    TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.InitializeWorldWindow(focus, size, fitAspectRatio);
                }
            };

            StationInformationDetailsViewModel = new StationInformationDetailsViewModel(
                smsUnitOfWorkFactory,
                applicationDialogService,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            TimeSeriesViewModel = new TimeSeriesViewModel(
                obsDBUnitOfWorkFactory,
                StationListViewModel.SelectedStations);

            TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
            {
                SyncRoiInChronologyViewWithTimeSeriesView();
            };

            TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                SyncRoiInChronologyViewWithTimeSeriesView();
            };

            StationListViewModel.SelectedStations.PropertyChanged += 
                SelectedStations_PropertyChanged;

            StationInformationListViewModel.SelectedStationInformations.PropertyChanged += 
                SelectedStationInformations_PropertyChanged;

            StationInformationDetailsViewModel.RepositoryOperationPerformed += 
                StationInformationDetailsViewModel_RepositoryOperationPerformed;

            _logger = new ViewModelLogger(_application.Logger, LogViewModel);
            //_logger = null; // Set to null to disable logging

            _application.Logger = _logger;
            TimeSeriesViewModel.Logger = _logger;

            _includeOperationIntervalBars = true;
            _includeObservationIntervalBars = true;
            _includeTransactionTimeIntervalBars = false;
            _showSMSDBList = true;
            _showStatDBList = true;
            _autoPopulateStatDBStationList = true;

            //DrawRoughOutlineOfDenmarkOnMap();
            DrawMapOfDenmark();

            SelectedDetailTabIndex = 0; 

            _application.Logger?.WriteLine(LogMessageCategory.Information, "DMI.Data.Studio.UI.WPF started up");
        }

        private void StationInformationDetailsViewModel_RepositoryOperationPerformed(
            object sender, 
            EventArgs e)
        {
            StationInformationListViewModel.Refresh();
        }

        private void SelectedStationInformations_PropertyChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stationInformations = sender as ObjectCollection<StationInformation>;
            _selectedStationInformations = stationInformations.Objects.ToList();

            //var logMessage = $"StationInformation selection changed ({_selectedStationInformations.Count} rows selected)";
            //_application.Logger?.WriteLine(LogMessageCategory.Information, logMessage);

            UpdateMapView();

            if (!AutoPopulateStatDBStationList)
            {
                UpdateChronologyView();
            }

            if (AutoPopulateStatDBStationList &&
                _selectedStationInformations.Count() == 1)
            {
                var rowCharacteristicsMap = StationInformationListViewModel.RowCharacteristicsMap.Object;

                var selectedStationInformationRow = _selectedStationInformations.Single();
                var rowCondition = rowCharacteristicsMap[selectedStationInformationRow.GdbArchiveOid].RowCondition;

                if (rowCondition == RowCondition.Current &&
                    selectedStationInformationRow.StationIDDMI.HasValue)
                {
                    // Initialize filter for StatDB station list
                    StationListViewModel.FindStationsViewModel.StationIdFilter = $"{selectedStationInformationRow.StationIDDMI}";
                    StationListViewModel.FindStationsCommand.ExecuteAsync(null);

                    // Find ud af, hvilket id vi forventer i StatDB
                    var nanoqStationID = int.Parse(StationListViewModel.FindStationsViewModel.StationIdFilter.AsNanoqStationId());

                    // Programmatisk sæt en evt matchende station til selected i statdb-listen
                    var matchingStationViewModelInStatDB =
                        StationListViewModel.StationViewModels.SingleOrDefault(_ => _.Station.StatID == nanoqStationID);

                    if (matchingStationViewModelInStatDB != null)
                    {
                        StationListViewModel.SelectedStationViewModels.Add(matchingStationViewModelInStatDB);
                    }
                }
            }
        }

        private void SelectedStations_PropertyChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stations = sender as ObjectCollection<Station>;
            _selectedStations = stations.Objects.ToList();

            //var logMessage = $"Station selection changed ({_selectedStations.Count} rows selected)";
            //_application.Logger?.WriteLine(LogMessageCategory.Information, logMessage);

            UpdateMapView();
            UpdateChronologyView();
        }

        private void UpdateMapView()
        {
            GeometryEditorViewModel.ClearPoints();

            // Add points representing stations from statdb
            if (_selectedStations != null && _selectedStations.Any())
            {
                foreach (var station in _selectedStations)
                {
                    foreach (var position in station.Positions)
                    {
                        if (position.Long.HasValue &&
                            position.Lat.HasValue)
                        {
                            var point = new PointD(
                                position.Long.Value,
                                position.Lat.Value);

                            GeometryEditorViewModel.AddPoint(point, 20, _stationBrush);
                        }
                    }
                }
            }

            // Add points representing stations from sms
            if (_selectedStationInformations != null && _selectedStationInformations.Any())
            {
                foreach (var stationInformation in _selectedStationInformations)
                {
                    if (!stationInformation.Wgs_lat.HasValue ||
                        !stationInformation.Wgs_long.HasValue)
                    {
                        continue;
                    }

                    var point = new PointD(
                        stationInformation.Wgs_long.Value,
                        stationInformation.Wgs_lat.Value);

                    GeometryEditorViewModel.AddPoint(point, 10, _stationInformationBrush);
                }
            }
        }

        private async void UpdateChronologyView()
        {
            var shapesAdded = 0;

            //_application.Logger?.WriteLine(LogMessageCategory.Information, "Updating Chronological View");

            var shapesBefore = ChronologyViewModel.GeometryEditorViewModel.ShapeViewModels.Count();

            ChronologyViewModel.GeometryEditorViewModel.ClearShapes();
            ChronologyViewModel.GeometryEditorViewModel.ClearLabels();
            ChronologyViewModel.CustomXAxisLabels.Object = new List<string>();

            var shapesAfter = ChronologyViewModel.GeometryEditorViewModel.ShapeViewModels.Count();

            //_application.Logger?.WriteLine(LogMessageCategory.Information, $"Shapes: {shapesBefore} -> {shapesAfter}");

            // Determine if there is anything to draw at all
            // (and thus whether the view should be visible)
            var stationsIncluded = 
                _selectedStations != null && 
                _selectedStations.Any();

            var stationInformationsIncluded = 
                _selectedStationInformations != null &&
                _selectedStationInformations.Any();

            var earliestTime = new DateTime(1950, 1, 1);

            // Now set latest time to current time, and derive number of years and days
            
            var latestTime = DateTime.UtcNow.TruncateToMilliseconds();
            var nYears = latestTime.Year - earliestTime.Year + 1;
            var startTimeOfEntireInterval = new DateTime(earliestTime.Year, 1, 1);
            var endTimeOfEntireInterval = new DateTime(latestTime.Year + 1, 1, 1);
            var totalNumberOfDaysForEntireInterval = (endTimeOfEntireInterval - startTimeOfEntireInterval).TotalDays;

            // Determine the total number of time interval bars we need to display
            int nTimeIntervalBars = 0;

            if (stationsIncluded)
            {
                nTimeIntervalBars += _selectedStations.Sum(station => station.Positions.Count);
            }

            var nStationInformations = 0;

            if (AutoPopulateStatDBStationList)
            {
                stationInformationsIncluded = false;
            }

            if (stationInformationsIncluded)
            {
                nStationInformations = _selectedStationInformations.Count();
                nTimeIntervalBars += nStationInformations;
            }

            var widthOfLaneLabelColumn = 50;
            var widthOfBarLabelBufferColumn = 240;
            var widthPrYear = 120;
            var heightOfHeader = 20;
            var heightPrPositionRecord = 20;
            var totalWidthOfMainPart = widthPrYear * nYears;
            var totalHeightOfMainPart = heightPrPositionRecord * nTimeIntervalBars;
            var imageWidth = widthOfLaneLabelColumn + totalWidthOfMainPart + widthOfBarLabelBufferColumn;
            var imageHeight = heightOfHeader + totalHeightOfMainPart;

            // Initialize the vertical lines that will mark where the individual years start
            var year = earliestTime.Year;
            var lastYear = endTimeOfEntireInterval.Year;

            var timeIntervalBarCount = 0;

            var customLabelsForNewView = new List<string>();
            var barHeight = 20;
            var y = -barHeight / 2;

            if (stationInformationsIncluded)
            {
                var rowCharacteristicsMap = StationInformationListViewModel.RowCharacteristicsMap.Object;

                StationInformation previousStationInformation = null;

                foreach (var stationInformation in _selectedStationInformations)
                {
                    if (IncludeOperationIntervalBars)
                    {
                        if (stationInformation.DateFrom.HasValue)
                        {
                            var startTime = stationInformation.DateFrom.Value;
                            var endTime = DateTime.UtcNow.TruncateToMilliseconds();

                            if (stationInformation.DateTo.HasValue)
                            {
                                endTime = stationInformation.DateTo.Value;
                            }

                            var xStart = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(startTime);
                            var xEnd = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(endTime);

                            var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var width = right - leftOfBar;

                            var rowCondition = rowCharacteristicsMap[stationInformation.GdbArchiveOid].RowCondition;

                            RectangleViewModel bar = rowCondition switch
                            {
                                SMS.Application.RowCondition.Deleted => new RedBar(),
                                SMS.Application.RowCondition.OutDated => new YellowBar(),
                                _ => new GreenBar()
                            };

                            bar.Point = new PointD(xStart + (xEnd - xStart) / 2, y);
                            bar.Width = xEnd - xStart;
                            bar.Height = barHeight;

                            ChronologyViewModel.GeometryEditorViewModel.AddShape(1, bar);
                            shapesAdded++;

                            customLabelsForNewView.Add(stationInformation.StationIDDMI.ToString());
                        }
                    }

                    if (IncludeTransactionTimeIntervalBars)
                    {
                        var startTime = stationInformation.GdbFromDate;
                        var endTime = stationInformation.GdbToDate.Year == 9999 ? DateTime.UtcNow.TruncateToMilliseconds() : stationInformation.GdbToDate;

                        var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var width = right - leftOfBar;
                        width = Math.Max(width, 1);
                        var spacingBetweenBarAndLabel = 3;

                        var fieldsUpdatedInSubsequentRecord = rowCharacteristicsMap[stationInformation.GdbArchiveOid].FieldsUpdatedInSubsequentRecord;

                        if (StationInformationListViewModel.FindStationInformationsViewModel.CurrentOption == Option.Option3)
                        {
                            fieldsUpdatedInSubsequentRecord = new HashSet<string>(fieldsUpdatedInSubsequentRecord.Intersect(StationInformationListViewModel.HistoricallyRelevantFields));
                        }

                        var label = fieldsUpdatedInSubsequentRecord.Count == 0
                            ? ""
                            : fieldsUpdatedInSubsequentRecord.Aggregate((c, n) => $"{c}, {n}");

                        var xStart = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(startTime);
                        var xEnd = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(endTime);

                        ChronologyViewModel.GeometryEditorViewModel.AddShape(1, new GrayBar
                        {
                            Point = new PointD(xStart + (xEnd - xStart) / 2, y),
                            Width = xEnd - xStart,
                            Height = barHeight
                        });

                        shapesAdded++;

                        ChronologyViewModel.GeometryEditorViewModel.LabelViewModels.Add(new TransactionTimeIntervalLabel
                        {
                            Text = $" {label}",
                            Point = new PointD(xEnd, -y),
                            Width = 100,
                            Height = 20,
                            Shift = new PointD(50, 0),
                            Opacity = 0.0
                        });
                    }

                    if (IncludeObservationIntervalBars && false)
                    {
                        var stationId = stationInformation.StationIDDMI.HasValue
                            ? $"{stationInformation.StationIDDMI.Value}".PadLeft(5, '0')
                            : "";

                        var intervals = await GetObservationIntervalsForStation(stationId); // 3

                        var barHeightRatio = 0.3;

                        if (intervals != null && intervals.Any())
                        {
                            // Make bars for the observation intervals (originating from ObsDB)
                            foreach (var interval in intervals)
                            {
                                var xStart = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(interval.Item1);
                                var xEnd = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(interval.Item2);

                                ChronologyViewModel.GeometryEditorViewModel.AddShape(1, new OrangeBar
                                {
                                    Point = new PointD(xStart + (xEnd - xStart) / 2, y),
                                    Width = xEnd - xStart,
                                    Height = barHeight * barHeightRatio
                                });

                                shapesAdded++;

                                var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item1 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item2 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                var width = right - leftOfBar;
                            }
                        }
                    }

                    previousStationInformation = stationInformation;

                    timeIntervalBarCount++;
                    y -= barHeight;
                }
            }

            if (stationsIncluded)
            {
                foreach (var station in _selectedStations)
                {
                    var stationIndex = 0;

                    Position previousPosition = null;

                    foreach (var position in station.Positions)
                    {
                        var startTime = position.StartTime;
                        var endTime = DateTime.UtcNow.TruncateToMilliseconds();

                        if (position.EndTime.HasValue &&
                            position.EndTime.Value < endTime)
                        {
                            endTime = position.EndTime.Value;
                        }

                        var xStart = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(startTime);
                        var xEnd = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(endTime);

                        RectangleViewModel bar = new GrayBar
                        {
                            Point = new PointD(xStart + (xEnd - xStart) / 2, y),
                            Width = xEnd - xStart,
                            Height = barHeight
                        };

                        ChronologyViewModel.GeometryEditorViewModel.AddShape(1, bar);

                        shapesAdded++;

                        customLabelsForNewView.Add(station.StatID.ToString());

                        if (previousPosition != null)
                        {
                            var labelText = "?";
                            var labelWidth = 100;
                            var labelBackgroundBrush = _positionDifferenceLabelBackgroundBrushDefault;

                            if (previousPosition.Lat.HasValue &&
                                previousPosition.Long.HasValue &&
                                position.Lat.HasValue &&
                                position.Long.HasValue)
                            {
                                var dx = position.Lat.Value - previousPosition.Lat.Value;
                                var dy = position.Long.Value - previousPosition.Long.Value;

                                // Using "Euclidean" distance for now
                                var diff = Math.Sqrt(dx * dx + dy * dy);

                                labelText = $"{diff:N10}";

                                if (position.StartTime.Year == 2018 &&
                                    position.StartTime.Month == 12 &&
                                    position.StartTime.Day == 4 &&
                                    position.StartTime.Hour == 10 &&
                                    position.StartTime.Minute == 31 &&
                                    position.StartTime.Second == 55 &&
                                    position.StartTime == previousPosition.EndTime)
                                {
                                    //labelText += $" (sms launch 2018-12-04 10:31:55) Lat: {previousPosition.Lat} -> {position.Lat}, Long: {previousPosition.Long} -> {position.Long}";
                                    //labelWidth = 1000;
                                    labelText += $" (sms launch)";
                                    labelWidth = 150;
                                }
                            }
                            else
                            {
                                labelBackgroundBrush = _positionDifferenceLabelBackgroundBrushHighlight;
                            }

                            var labelViewModel = new PositionDifferenceLabel(
                                labelBackgroundBrush,
                                _positionDifferenceLabelForegroundBrush)
                            {
                                Text = labelText,
                                Point = new PointD(xStart, -(y + barHeight / 2)),
                                Width = labelWidth,
                                Height = barHeight * 0.75,
                                Shift = new PointD(0, 0),
                                Opacity = 0.5
                            };

                            ChronologyViewModel.GeometryEditorViewModel.LabelViewModels.Add(labelViewModel);
                        }

                        var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var width = right - leftOfBar;

                        if (IncludeObservationIntervalBars)
                        {
                            var stationId = station.StatID.ToString();
                            stationId = stationId.Substring(0, stationId.Length - 2);
                            stationId = $"{stationId}".PadLeft(5, '0');
                            var intervals = await GetObservationIntervalsForStation(stationId); // 4

                            var barHeightRatio = 0.3;

                            if (intervals != null && intervals.Any())
                            {
                                // Make bars for the observation intervals (originating from ObsDB)
                                foreach (var interval in intervals)
                                {
                                    var xStartInterval = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(interval.Item1);
                                    var xEndInterval = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(interval.Item2);

                                    var contained = xStartInterval >= xStart && xEndInterval <= xEnd;
                                    var outside = xEndInterval < xStart || xStartInterval > xEnd;

                                    RectangleViewModel bar2;
                                    
                                    if (contained)
                                    {
                                        bar2 = new LightGreenBar();
                                    }
                                    else if (outside)
                                    {
                                        bar2 = new OrangeBar();
                                    }
                                    else
                                    {
                                        bar2 = new LightRedBar();
                                    }

                                    bar2.Point = new PointD(xStartInterval + (xEndInterval - xStartInterval) / 2, y);
                                    bar2.Width = xEndInterval - xStartInterval;
                                    bar2.Height = barHeight * barHeightRatio;

                                    if (!(bar2 is OrangeBar))
                                    {
                                        ChronologyViewModel.GeometryEditorViewModel.AddShape(1, bar2);

                                        shapesAdded++;
                                    }

                                    leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item1 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                    right = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item2 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                    width = right - leftOfBar;
                                }
                            }
                        }

                        previousPosition = position;
                        timeIntervalBarCount++;
                        y -= barHeight;
                    }
                }
            }

            ChronologyViewModel.CustomXAxisLabels.Object = customLabelsForNewView;

            var tMax = new DateTime(2050, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var xMax = Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel.ConvertDateTimeToXValue(tMax);

            ChronologyViewModel.GeometryEditorViewModel.WorldWindowBottomRightLimit = new Point(xMax, totalHeightOfMainPart + 40);

            //_application.Logger?.WriteLine(LogMessageCategory.Information, $"Shapes: 0 -> {shapesAdded}");
        }

        private void OpenSettingsDialog(
            object owner)
        {
            var dialogViewModel = new SettingsDialogViewModel(
                _smsDBUnitOfWorkFactory,
                _statDBDataProvider);

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void OpenAboutDialog(
            object owner)
        {
            var dialogViewModel = new AboutDialogViewModel();

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void CreateStationInformation(
            object owner)
        {
            var dialogViewModel = new CreateStationInformationDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

            var newStationInformation = new StationInformation
            {
                StationName = dialogViewModel.StationName,
                StationIDDMI = int.Parse(dialogViewModel.Stationid_dmi),
                Stationtype = SharedData.StationTypeDisplayTextMap.Single(kvp => kvp.Value == dialogViewModel.StationType).Key,
                StationOwner = SharedData.StationOwnerDisplayTextMap.Single(kvp => kvp.Value == dialogViewModel.StationOwner).Key,
                Country = SharedData.CountryDisplayTextMap.Single(kvp => kvp.Value == dialogViewModel.Country).Key,
                Status = SharedData.StatusDisplayTextMap.Single(kvp => kvp.Value == dialogViewModel.Status).Key,
                CreatedUser = SharedData.LoggedInUser,
                CreatedDate = currentTime,
                GdbFromDate = currentTime,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            };

            if (string.IsNullOrEmpty(dialogViewModel.DateFrom))
            {
                newStationInformation.DateFrom = new DateTime?();
            }
            else
            {
                dialogViewModel.DateFrom.TryParsingAsDateTime(out var dateFrom);
                newStationInformation.DateFrom = dateFrom;
            }

            if (string.IsNullOrEmpty(dialogViewModel.DateTo))
            {
                newStationInformation.DateTo = new DateTime?();
            }
            else
            {
                dialogViewModel.DateTo.TryParsingAsDateTime(out var dateTo);
                newStationInformation.DateTo = dateTo;
            }

            if (string.IsNullOrEmpty(dialogViewModel.Wgs_lat))
            {
                newStationInformation.Wgs_lat = new double?();
            }
            else
            {
                newStationInformation.Wgs_lat = double.Parse(dialogViewModel.Wgs_lat, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(dialogViewModel.Wgs_long))
            {
                newStationInformation.Wgs_long = new double?();
            }
            else
            {
                newStationInformation.Wgs_long = double.Parse(dialogViewModel.Wgs_long, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(dialogViewModel.Hha))
            {
                newStationInformation.Hha = new double?();
            }
            else
            {
                newStationInformation.Hha = double.Parse(dialogViewModel.Hha, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(dialogViewModel.Hhp))
            {
                newStationInformation.Hhp = new double?();
            }
            else
            {
                newStationInformation.Hhp = double.Parse(dialogViewModel.Hhp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }

            using (var unitOfWork = _smsDBUnitOfWorkFactory.GenerateUnitOfWork())
            {
                newStationInformation.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                newStationInformation.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                unitOfWork.StationInformations.Add(newStationInformation);
            }

            // Vis stationen i list viewet samt på kortet
            StationInformationListViewModel.FindStationInformationsViewModel.NameFilter = newStationInformation.StationName;
            StationInformationListViewModel.FindStationInformationsViewModel.StationIdFilter = newStationInformation.StationIDDMI.ToString();
            StationInformationListViewModel.Refresh();
        }

        private async Task ExportData(
            object owner)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Json Files(*.json)|*.json|Xml Files(*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            TaskViewModel.Show("Exporting data", false);
            RefreshCommandAvailability();

            await _smsApplication.ExportData(
                dialog.FileName,
                false,
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            if (!TaskViewModel.Abort)
            {
                var dialogViewModel =
                    new MessageBoxDialogViewModel($"Exported data succesfully to {dialog.FileName}", false);

                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }

            TaskViewModel.Hide();
            RefreshCommandAvailability();
        }

        private bool CanExportData(
            object owner)
        {
            return !TaskViewModel.Busy;
        }

        private async Task ExtractMeteorologicalStationList(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListDialogViewModel(
                "Extract Meteorological Stations");

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            DateTime? rollBackDate = null;

            if (!string.IsNullOrEmpty(dialogViewModel.Date))
            {
                dialogViewModel.Date.TryParsingAsDateTime(out var temp);
                rollBackDate = temp;
            }

            TaskViewModel.Show("Extracting Meteorological Stations", false);
            RefreshCommandAvailability();

            await _application.ExtractMeteorologicalStations(
                rollBackDate,
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed Extraction of Meteorological Stations", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }

            TaskViewModel.Hide();
            RefreshCommandAvailability();
        }

        private async Task ExtractOceanographicalStationList(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListDialogViewModel(
                "Extract Oceanographical Stations");

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            DateTime? rollBackDate = null;

            if (!string.IsNullOrEmpty(dialogViewModel.Date))
            {
                dialogViewModel.Date.TryParsingAsDateTime(out var temp);
                rollBackDate = temp;
            }

            TaskViewModel.Show("Extracting Oceanographical Stations", false);
            RefreshCommandAvailability();

            await _application.ExtractOceanographicalStations(
                rollBackDate,
                (progress, currentActivity) =>
                {
                    TaskViewModel.Progress = progress;
                    TaskViewModel.NameOfCurrentSubtask = currentActivity;
                    return TaskViewModel.Abort;
                });

            if (!TaskViewModel.Abort)
            {
                var messageBoxDialog = new MessageBoxDialogViewModel("Completed Extraction of Oceanographical Stations", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);
            }

            TaskViewModel.Hide();
            RefreshCommandAvailability();
        }

        private async Task ImportData(
            object owner)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Json Files(*.json)|*.json|Xml Files(*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            TaskViewModel.Show("Importing Data", false);
            RefreshCommandAvailability();

            await _smsApplication.ImportData(
                dialog.FileName);

            if (!TaskViewModel.Abort)
            {
                var dialogViewModel =
                    new MessageBoxDialogViewModel($"{dialog.FileName} was imported succesfully", false);

                _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
            }

            TaskViewModel.Hide();
            RefreshCommandAvailability();
        }

        private async Task ClearRepository(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Clear Repository?", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            TaskViewModel.Show("Clearing Repository", false);

            await _smsApplication.ClearRepository();

            var messageBoxDialog = new MessageBoxDialogViewModel("Completed clearing repository", false);
                _applicationDialogService.ShowDialog(messageBoxDialog, owner as Window);

            TaskViewModel.Hide();
            RefreshCommandAvailability();
        }

        private void RefreshCommandAvailability()
        {
            ExportDataCommand.RaiseCanExecuteChanged();
            ImportDataCommand.RaiseCanExecuteChanged();
            //MakeBreakfastCommand.RaiseCanExecuteChanged();
            //ExtractMeteorologicalStationsCommand.RaiseCanExecuteChanged();
            //ExtractOceanographicalStationsCommand.RaiseCanExecuteChanged();
        }

        private async Task<List<Tuple<DateTime, DateTime>>> GetObservationIntervalsForStation(
            string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                return null;
            }

            var nanoqStationId = SMS.ViewModel.StringExtensions.AsNanoqStationId(stationId, true);
            var parameter = "temp_dry";
            var maxTolerableDifferenceBetweenTwoObservationsInDays = 20.0;

            if (stationId.Substring(0, 2) == "05")
            {
                parameter = "precip_past6h";
            }

            parameter = "precip_past10min";

            var intervals = await _application.ExtractObservationIntervals(
                nanoqStationId,
                parameter,
                maxTolerableDifferenceBetweenTwoObservationsInDays,
                1953,
                2024,
                true);

            return intervals;
        }

        private void DrawRoughOutlineOfDenmarkOnMap()
        {
            var lineThickness = 0.02;
            var brush = new SolidColorBrush(Colors.Black);

            var fyn_p1 = new PointD(9.8, 55.36);
            var fyn_p2 = new PointD(10.31, 55.62);
            var fyn_p3 = new PointD(10.83, 55.23);
            var fyn_p4 = new PointD(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness, brush);

            var jylland_p1 = new PointD(8.65, 54.92);
            var jylland_p2 = new PointD(8.08, 55.57);
            var jylland_p3 = new PointD(8.13, 56.56);
            var jylland_p4 = new PointD(8.61, 57.12);
            var jylland_p5 = new PointD(9.61, 57.27);
            var jylland_p6 = new PointD(10.62, 57.74);
            var jylland_p7 = new PointD(10.43, 56.52);
            var jylland_p8 = new PointD(10.89, 56.49);
            var jylland_p9 = new PointD(10.76, 56.17);
            var jylland_p10 = new PointD(10.33, 56.26);
            var jylland_p11 = new PointD(10.2, 55.83);
            var jylland_p12 = new PointD(9.6, 55.42);
            var jylland_p13 = new PointD(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness, brush);

            var sjalland_p1 = new PointD(10.98, 55.74);
            var sjalland_p2 = new PointD(12.26, 56.14);
            var sjalland_p3 = new PointD(12.67, 55.6);
            var sjalland_p4 = new PointD(12.05, 54.98);
            var sjalland_p5 = new PointD(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness, brush);

            var bornholm_p1 = new PointD(14.77, 55.3);
            var bornholm_p2 = new PointD(15.16, 55.14);
            var bornholm_p3 = new PointD(15.08, 54.98);
            var bornholm_p4 = new PointD(14.68, 55.09);
            GeometryEditorViewModel.AddLine(bornholm_p1, bornholm_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(bornholm_p2, bornholm_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(bornholm_p3, bornholm_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(bornholm_p4, bornholm_p1, lineThickness, brush);

            var gronland_p1 = new PointD(-61.08, 81.89);
            var gronland_p2 = new PointD(-32.43, 83.63);
            var gronland_p3 = new PointD(-11.34, 81.46);
            var gronland_p4 = new PointD(-22.23, 70.12);
            var gronland_p5 = new PointD(-43.33, 59.77);
            var gronland_p6 = new PointD(-58.45, 75.57);
            var gronland_p7 = new PointD(-68.47, 76.09);
            var gronland_p8 = new PointD(-72.51, 78.44);
            GeometryEditorViewModel.AddLine(gronland_p1, gronland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p2, gronland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p3, gronland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p4, gronland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p5, gronland_p6, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p6, gronland_p7, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p7, gronland_p8, lineThickness, brush);
            GeometryEditorViewModel.AddLine(gronland_p8, gronland_p1, lineThickness, brush);
        }

        private void DrawMapOfDenmark()
        {
            // Load GML file of Denmark
            //var fileName = @".\Data\Denmark.gml";
            var fileName = @".\Data\DenmarkAndGreenland.gml";
            Craft.DataStructures.IO.DataIOHandler.ExtractGeometricPrimitivesFromGMLFile(fileName, out var polygons);

            // Add the regions of Denmark to the map as polygons
            var lineThickness = 0.005;
            var brush = new SolidColorBrush(new Color { R = 150, G = 255, B = 150, A = 127});

            foreach (var polygon in polygons)
            {
                GeometryEditorViewModel.AddPolygon(polygon.Select(p => new PointD(p[1], p[0])), lineThickness, brush);
            }
        }

        private void SyncRoiInChronologyViewWithTimeSeriesView()
        {
            var worldWindowUpperLeft = TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft;
            var worldWindowSize = TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowSize;

            ChronologyViewModel.GeometryEditorViewModel.SelectedRegion.Object = new BoundingBox
            {
                Left = worldWindowUpperLeft.X,
                Top = worldWindowUpperLeft.Y,
                Width = worldWindowSize.Width,
                Height = 2000
            };
        }

        private bool TimeViewsInSync()
        {
            var bb1 = ChronologyViewModel.GeometryEditorViewModel.SelectedRegion.Object;

            if (bb1 == null)
            {
                return false;
            }

            var bb2 = new BoundingBox
            {
                Left = TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowUpperLeft.X,
                Width = TimeSeriesViewModel.ScatterChartViewModel.GeometryEditorViewModel.WorldWindowSize.Width,
            };

            if (Math.Abs(bb1.Left - bb2.Left) > 0.00000001 ||
                Math.Abs(bb1.Width - bb2.Width) > 0.00000001)
            {
                return false;
            }

            return true;
        }
    }
}
