using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Craft.Logging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D;
using Craft.ViewModels.Chronology;
using DMI.SMS.ViewModel;
using DMI.SMS.Domain.Entities;
using DMI.StatDB.ViewModel;
using DMI.StatDB.Domain.Entities;

namespace DMI.Data.Studio.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Dictionary<SMS.Application.RowCondition, Brush> _rowConditionToBrushMap =
            new Dictionary<SMS.Application.RowCondition, Brush>
            {
                { SMS.Application.RowCondition.Current, new SolidColorBrush(Colors.DarkGreen) },
                { SMS.Application.RowCondition.OutDated, new SolidColorBrush(Colors.PaleGoldenrod) },
                { SMS.Application.RowCondition.Deleted, new SolidColorBrush(Colors.DarkRed) }
            };

        private readonly SMS.Application.IUIDataProvider _smsDataProvider;
        private readonly StatDB.Application.IUIDataProvider _statDBDataProvider;
        private readonly IDialogService _applicationDialogService;
        private readonly ILogger _logger;
        private List<StationInformation> _selectedStationInformations;
        private List<Station> _selectedStations;
        private bool _includeOperationIntervalBars;
        private bool _includeObservationIntervalBars;
        private bool _includeTransactionTimeIntervalBars;
        private RelayCommand<object> _openSettingsDialogCommand;
        private RelayCommand<object> _openAboutDialogCommand;
        private RelayCommand<object> _createStationInformationCommand;
        private RelayCommand<object> _exportDataCommand;
        private AsyncCommand<object> _generateMeteorologicalStationListCommand;
        private RelayCommand<object> _importDataCommand;
        private RelayCommand<object> _restoreTestDatabaseCommand;
        private Brush _stationInformationBrush = new SolidColorBrush(Colors.DarkRed);
        private Brush _stationBrush = new SolidColorBrush(Colors.DarkOrange);
        private Brush _StatDBTimeIntervalBrush = new SolidColorBrush(Colors.DarkSlateGray);
        private Brush _observationTimeIntervalBrush = new SolidColorBrush(Colors.Orange);
        private Brush _transactionTimeIntervalBrush = new SolidColorBrush(Colors.DarkSlateGray);
        private Brush _stationIdLabelBrush = new SolidColorBrush(Colors.Black);
        private bool _classifyRecordsWithCondition;
        private readonly ObservableObject<bool> _observableForClassifyRecordsWithCondition;
        private int _selectedOveralTabIndex;

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

        public StationInformationListViewModel StationInformationListViewModel { get; private set; }
        public StationInformationDetailsViewModel StationInformationDetailsViewModel { get; private set; }
        public StationListViewModel StationListViewModel { get; private set; }
        public MathematicalGeometryEditorViewModel GeometryEditorViewModel { get; private set; }
        public ChronologyViewModel ChronologyViewModel { get; private set; }
        public StatisticsViewModel StatisticsViewModel { get; private set; }

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

        public int SelectedOveralTabIndex
        {
            get { return _selectedOveralTabIndex; }
            set
            {
                _selectedOveralTabIndex = value;
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

        public RelayCommand<object> ExportDataCommand
        {
            get { return _exportDataCommand ?? (_exportDataCommand = new RelayCommand<object>(ExportData)); }
        }

        public AsyncCommand<object> GenerateMeteorologicalStationListCommand
        {
            get { return _generateMeteorologicalStationListCommand ?? (
                    _generateMeteorologicalStationListCommand = new AsyncCommand<object>(GenerateMeteorologicalStationList)); }
        }

        public RelayCommand<object> ImportDataCommand
        {
            get { return _importDataCommand ?? (_importDataCommand = new RelayCommand<object>(ImportData)); }
        }

        public RelayCommand<object> RestoreTestDatabaseCommand
        {
            get { return _restoreTestDatabaseCommand ?? (_restoreTestDatabaseCommand = new RelayCommand<object>(RestoreTestDatabase)); }
        }

        public MainWindowViewModel(
            SMS.Application.IUIDataProvider smsDataProvider,
            StatDB.Application.IUIDataProvider statDBDataProvider,
            IDialogService applicationDialogService,
            ILogger logger)
        {
            _smsDataProvider = smsDataProvider;
            _statDBDataProvider = statDBDataProvider;
            _applicationDialogService = applicationDialogService;
            _logger = logger;

            _smsDataProvider.Initialize(logger);
            _statDBDataProvider.Initialize(logger);

            _smsApplication = new SMS.Application.Application(
                _smsDataProvider, _logger);

            _mainWindowTitle = "DMI Data Studio";

            _observableForClassifyRecordsWithCondition = new ObservableObject<bool>();
            _observableForClassifyRecordsWithCondition.Object = true;
            _selectedOveralTabIndex = 0;

            StationInformationListViewModel = new StationInformationListViewModel(
                smsDataProvider, 
                _applicationDialogService,
                _observableForClassifyRecordsWithCondition);

            StationListViewModel = new StationListViewModel(
                statDBDataProvider,
                applicationDialogService);

            StatisticsViewModel = new StatisticsViewModel(
                StationInformationListViewModel.StationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            GeometryEditorViewModel = new MathematicalGeometryEditorViewModel(100, 7.92, 54.82);
            DrawRoughOutlineOfDenmarkOnMap();
            
            ChronologyViewModel = new ChronologyViewModel(new DateTime(2015, 1, 1), DateTime.UtcNow.TruncateToMilliseconds(), 50, 240);

            StationInformationDetailsViewModel = new StationInformationDetailsViewModel(
                smsDataProvider,
                applicationDialogService,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            StationListViewModel.SelectedStations.PropertyChanged += 
                SelectedStations_PropertyChanged;

            StationInformationListViewModel.SelectedStationInformations.PropertyChanged += 
                SelectedStationInformations_PropertyChanged;

            StationInformationDetailsViewModel.RepositoryOperationPerformed += 
                StationInformationDetailsViewModel_RepositoryOperationPerformed;

            _includeOperationIntervalBars = true;
            _includeObservationIntervalBars = false;
            _includeTransactionTimeIntervalBars = true;
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

            UpdateMapView();
            UpdateChronologyView();

            if (_selectedStationInformations.Count > 0)
            {
                //SelectedOveralTabIndex = 1;
            }
        }

        private void SelectedStations_PropertyChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stations = sender as ObjectCollection<Station>;
            _selectedStations = stations.Objects.ToList();
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
                            var point = new Point2D(
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

                    var point = new Point2D(
                        stationInformation.Wgs_long.Value,
                        stationInformation.Wgs_lat.Value);

                    GeometryEditorViewModel.AddPoint(point, 10, _stationInformationBrush);
                }
            }
        }

        private void UpdateChronologyView()
        {
            ChronologyViewModel.VerticalLineViewModels.Clear();
            ChronologyViewModel.TimeIntervalBarViewModels.Clear();

            // Determine if there is anything to draw at all
            // (and thus whether the view should be visible)
            var stationsIncluded = 
                _selectedStations != null && 
                _selectedStations.Any();

            var stationInformationsIncluded = 
                _selectedStationInformations != null &&
                _selectedStationInformations.Any();

            if (stationsIncluded || stationInformationsIncluded)
            {
                ChronologyViewModel.IsVisible = true;
            }
            else
            {
                ChronologyViewModel.IsVisible = false;
                return;
            }

            // Identify earliest time
            var startTimes = new List<DateTime>();

            if (IncludeOperationIntervalBars)
            {
                if (stationsIncluded)
                {
                    foreach (var station in _selectedStations)
                    {
                        foreach (var position in station.Positions)
                        {
                            startTimes.Add(position.StartTime);
                        }
                    }
                }

                if (stationInformationsIncluded)
                {
                    startTimes.AddRange(_selectedStationInformations
                        .Select(s => s.DateFrom)
                        .Where(d => d.HasValue)
                        .Select(d => d.Value));
                }
            }

            if (IncludeObservationIntervalBars)
            {
                if (stationInformationsIncluded)
                {
                    _selectedStationInformations.ForEach(s =>
                    {
                        if (s.StationIDDMI.HasValue)
                        {
                            var stationId = $"{s.StationIDDMI.Value}".PadLeft(5, '0');
                            var intervals = GetObservationIntervalsForStation(stationId);

                            if (intervals != null)
                            {
                                startTimes.AddRange(intervals.Select(i => i.Item1));
                            }
                        }
                    });
                }

                if (stationsIncluded)
                {
                    _selectedStations.ForEach(s =>
                    {
                        var stationId = s.StatID.ToString();
                        stationId = stationId.Substring(0, stationId.Length - 2);
                        stationId = $"{stationId}".PadLeft(5, '0');

                        var intervals = GetObservationIntervalsForStation(stationId);

                        if (intervals != null)
                        {
                            startTimes.AddRange(intervals.Select(i => i.Item1));
                        }
                    });
                }
            }

            var earliestTime = new DateTime(1889, 1, 1);

            if (startTimes.Any())
            {
                earliestTime = startTimes.Min();
            }

            // (earliest time identified)
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

            ChronologyViewModel.ImageWidth = imageWidth;
            ChronologyViewModel.ImageHeight = imageHeight;

            // Initialize the vertical lines that will mark where the individual years start
            var year = earliestTime.Year;
            var lastYear = endTimeOfEntireInterval.Year;
            while (year <= lastYear)
            {
                var x = widthOfLaneLabelColumn + totalWidthOfMainPart * (new DateTime(year, 1, 1) - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;

                ChronologyViewModel.VerticalLineViewModels.Add(new VerticalLineViewModel
                {
                    X = x,
                    Header = year < lastYear ? year.ToString() : "",
                    Height = imageHeight
                });

                year++;
            }

            var timeIntervalBarCount = 0;
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

                            var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var width = right - leftOfBar;

                            var rowCondition = rowCharacteristicsMap[stationInformation.GdbArchiveOid].RowCondition;

                            ChronologyViewModel.TimeIntervalBarViewModels.Add(
                                new Craft.ViewModels.Chronology.TimeIntervalBarViewModel
                                {
                                    Label = stationInformation.StationIDDMI.ToString(),
                                    LabelBrush = _stationIdLabelBrush,
                                    Top = heightOfHeader + timeIntervalBarCount * heightPrPositionRecord,
                                    LeftOfBar = leftOfBar,
                                    Width = width,
                                    Height = heightPrPositionRecord,
                                    Brush = _rowConditionToBrushMap[rowCondition]
                                });
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

                        ChronologyViewModel.TimeIntervalBarViewModels.Add(
                            new Craft.ViewModels.Chronology.TimeIntervalBarViewModel
                            {
                                Label = label,
                                LeftOfLabel = right + spacingBetweenBarAndLabel,
                                LabelBrush = _transactionTimeIntervalBrush,
                                Top = heightOfHeader + timeIntervalBarCount * heightPrPositionRecord,
                                LeftOfBar = leftOfBar,
                                Width = width,
                                Height = heightPrPositionRecord,
                                Brush = _transactionTimeIntervalBrush
                            });
                    }

                    if (IncludeObservationIntervalBars)
                    {
                        var stationId = stationInformation.StationIDDMI.HasValue
                            ? $"{stationInformation.StationIDDMI.Value}".PadLeft(5, '0')
                            : "";

                        var intervals = GetObservationIntervalsForStation(stationId);

                        var barHeightRatio = 0.3;

                        if (intervals != null && intervals.Any())
                        {
                            // Make bars for the observation intervals (originating from ObsDB)
                            foreach (var interval in intervals)
                            {
                                var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item1 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item2 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                var width = right - leftOfBar;

                                ChronologyViewModel.TimeIntervalBarViewModels.Add(
                                    new Craft.ViewModels.Chronology.TimeIntervalBarViewModel
                                    {
                                        Label = "",
                                        Top = heightOfHeader + timeIntervalBarCount * heightPrPositionRecord + heightPrPositionRecord * (0.5 - barHeightRatio / 2),
                                        LeftOfBar = leftOfBar,
                                        Width = width,
                                        Height = heightPrPositionRecord * barHeightRatio,
                                        Brush = _observationTimeIntervalBrush
                                    });
                            }
                        }
                    }

                    previousStationInformation = stationInformation;

                    timeIntervalBarCount++;
                }
            }

            if (stationsIncluded)
            {
                foreach (var station in _selectedStations)
                {
                    foreach (var position in station.Positions)
                    {
                        var startTime = position.StartTime;
                        var endTime = DateTime.UtcNow.TruncateToMilliseconds();

                        if (position.EndTime.HasValue &&
                            position.EndTime.Value < endTime)
                        {
                            endTime = position.EndTime.Value;
                        }

                        var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var width = right - leftOfBar;

                        ChronologyViewModel.TimeIntervalBarViewModels.Add(
                            new Craft.ViewModels.Chronology.TimeIntervalBarViewModel
                            {
                                Label = station.StatID.ToString(),
                                LabelBrush = _stationIdLabelBrush,
                                Top = heightOfHeader + timeIntervalBarCount * heightPrPositionRecord,
                                LeftOfBar = leftOfBar,
                                Width = width,
                                Height = heightPrPositionRecord,
                                Brush = _StatDBTimeIntervalBrush
                            });

                        if (IncludeObservationIntervalBars)
                        {
                            var stationId = station.StatID.ToString();
                            stationId = stationId.Substring(0, stationId.Length - 2);
                            stationId = $"{stationId}".PadLeft(5, '0');
                            var intervals = GetObservationIntervalsForStation(stationId);

                            var barHeightRatio = 0.3;

                            if (intervals != null && intervals.Any())
                            {
                                // Make bars for the observation intervals (originating from ObsDB)
                                foreach (var interval in intervals)
                                {
                                    leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item1 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                    right = widthOfLaneLabelColumn + totalWidthOfMainPart * (interval.Item2 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                                    width = right - leftOfBar;

                                    ChronologyViewModel.TimeIntervalBarViewModels.Add(
                                        new Craft.ViewModels.Chronology.TimeIntervalBarViewModel
                                        {
                                            Label = "",
                                            Top = heightOfHeader + timeIntervalBarCount * heightPrPositionRecord + heightPrPositionRecord * (0.5 - barHeightRatio / 2),
                                            LeftOfBar = leftOfBar,
                                            Width = width,
                                            Height = heightPrPositionRecord * barHeightRatio,
                                            Brush = _observationTimeIntervalBrush
                                        });
                                }
                            }
                        }

                        timeIntervalBarCount++;
                    }
                }
            }
        }

        private void OpenSettingsDialog(
            object owner)
        {
            var dialogViewModel = new SettingsDialogViewModel(
                _smsDataProvider,
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

            _smsDataProvider.CreateStationInformation(newStationInformation, true);

            // Vis stationen i list viewet samt på kortet
            StationInformationListViewModel.FindStationInformationsViewModel.NameFilter = newStationInformation.StationName;
            StationInformationListViewModel.FindStationInformationsViewModel.StationIdFilter = newStationInformation.StationIDDMI.ToString();
            StationInformationListViewModel.Refresh();
        }

        private void ExportData(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Export Data?", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            _smsDataProvider.ExportData(
                @"C:\Temp\SMSData_Export.json",
                StationInformationListViewModel.FindStationInformationsViewModel.FilterAsExpressionCollection());

            dialogViewModel = new MessageBoxDialogViewModel("Exported data succesfully to C:\\Temp\\SMSData.json", false);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private async Task GenerateMeteorologicalStationList(
            object owner)
        {
            await _smsApplication.ExtractMeteorologicalStations(null);
        }

        private void ImportData(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Import Data?\n\n(this requires the presence of a data file named C:\\Temp\\SMSData.xml and will overwrite any data in the repository)", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            _smsDataProvider.ImportData(@"C:\Temp\SMSData.xml");

            dialogViewModel = new MessageBoxDialogViewModel("C:\\Temp\\SMSData.xml was imported succesfully", false);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void RestoreTestDatabase(
            object owner)
        {
            var dialogViewModel = new MessageBoxDialogViewModel("Restore Test database?\n\n(this will undo any changes applied to the contents of the test database)", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            File.Copy(@"C:\Temp\SMSData.xml", @"C:\Temp\SMSFileRepository.xml", true);

            _smsDataProvider.Initialize(_logger);

            dialogViewModel = new MessageBoxDialogViewModel("Test database restored", false);
            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);

            if (StationInformationListViewModel.ItemCount > 0)
            {
                StationInformationListViewModel.Refresh();
            }
        }

        private List<Tuple<DateTime, DateTime>> GetObservationIntervalsForStation(
            string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                return null;
            }

            var directoryName = Path.Combine(@".\Data", $"{stationId}");
            var searchPattern = "temp_dry*.*";
            var maxTolerableDifferenceBetweenTwoObservationsInDays = 20.0;

            if (stationId.Substring(0, 2) == "05")
            {
                searchPattern = "precip_past6h*.*";
            }

            return _smsDataProvider.ReadObservationIntervalsForStation(
                directoryName,
                searchPattern,
                maxTolerableDifferenceBetweenTwoObservationsInDays);
        }

        private void DrawRoughOutlineOfDenmarkOnMap()
        {
            var lineThickness = 0.02;

            var fyn_p1 = new Point2D(9.8, 55.36);
            var fyn_p2 = new Point2D(10.31, 55.62);
            var fyn_p3 = new Point2D(10.83, 55.23);
            var fyn_p4 = new Point2D(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness);

            var jylland_p1 = new Point2D(8.65, 54.92);
            var jylland_p2 = new Point2D(8.08, 55.57);
            var jylland_p3 = new Point2D(8.13, 56.56);
            var jylland_p4 = new Point2D(8.61, 57.12);
            var jylland_p5 = new Point2D(9.61, 57.27);
            var jylland_p6 = new Point2D(10.62, 57.74);
            var jylland_p7 = new Point2D(10.43, 56.52);
            var jylland_p8 = new Point2D(10.89, 56.49);
            var jylland_p9 = new Point2D(10.76, 56.17);
            var jylland_p10 = new Point2D(10.33, 56.26);
            var jylland_p11 = new Point2D(10.2, 55.83);
            var jylland_p12 = new Point2D(9.6, 55.42);
            var jylland_p13 = new Point2D(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness);

            var sjalland_p1 = new Point2D(10.98, 55.74);
            var sjalland_p2 = new Point2D(12.26, 56.14);
            var sjalland_p3 = new Point2D(12.67, 55.6);
            var sjalland_p4 = new Point2D(12.05, 54.98);
            var sjalland_p5 = new Point2D(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness);

            var bornholm_p1 = new Point2D(14.77, 55.3);
            var bornholm_p2 = new Point2D(15.16, 55.14);
            var bornholm_p3 = new Point2D(15.08, 54.98);
            var bornholm_p4 = new Point2D(14.68, 55.09);
            GeometryEditorViewModel.AddLine(bornholm_p1, bornholm_p2, lineThickness);
            GeometryEditorViewModel.AddLine(bornholm_p2, bornholm_p3, lineThickness);
            GeometryEditorViewModel.AddLine(bornholm_p3, bornholm_p4, lineThickness);
            GeometryEditorViewModel.AddLine(bornholm_p4, bornholm_p1, lineThickness);

            var gronland_p1 = new Point2D(-61.08, 81.89);
            var gronland_p2 = new Point2D(-32.43, 83.63);
            var gronland_p3 = new Point2D(-11.34, 81.46);
            var gronland_p4 = new Point2D(-22.23, 70.12);
            var gronland_p5 = new Point2D(-43.33, 59.77);
            var gronland_p6 = new Point2D(-58.45, 75.57);
            var gronland_p7 = new Point2D(-68.47, 76.09);
            var gronland_p8 = new Point2D(-72.51, 78.44);
            GeometryEditorViewModel.AddLine(gronland_p1, gronland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p2, gronland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p3, gronland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p4, gronland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p5, gronland_p6, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p6, gronland_p7, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p7, gronland_p8, lineThickness);
            GeometryEditorViewModel.AddLine(gronland_p8, gronland_p1, lineThickness);
        }
    }
}
