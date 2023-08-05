using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.Scrolling;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Application;
//using DMI.Utils;

namespace DMI.SMS.ViewModel
{
    public class StationInformationCollectionViewModel : ImageEditorViewModel
    {
        private IUIDataProvider _dataProvider;
        private readonly ObjectCollection<StationInformation> _selectedStationInformations;
        private readonly ObservableObject<Dictionary<int, RowCharacteristics>> _rowConditionMap;
        private IEnumerable<StationInformation> _stationInformations;
        private ObservableCollection<TimeLineViewModel> _timeLineViewModels;
        private ObservableCollection<TimeIntervalBarViewModel> _operationIntervalBarViewModels;
        private ObservableCollection<TimeIntervalBarViewModel> _observationIntervalBarViewModels;
        private bool _isVisible;
        private bool _includeOperationIntervalsBars;
        private bool _includeObservationIntervalsBars;

        private Dictionary<RowCondition, Brush> _conditionToBrushMap = new Dictionary<RowCondition, Brush>
        {
            { RowCondition.Current, new SolidColorBrush(Colors.DarkGreen) },
            { RowCondition.OutDated, new SolidColorBrush(Colors.DarkOrange) },
            { RowCondition.Deleted, new SolidColorBrush(Colors.DarkRed) }
        };

        public ObservableCollection<TimeLineViewModel> TimeLineViewModels
        {
            get
            {
                return _timeLineViewModels;
            }
            set
            {
                _timeLineViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TimeIntervalBarViewModel> OperationIntervalBarViewModels
        {
            get
            {
                return _operationIntervalBarViewModels;
            }
            set
            {
                _operationIntervalBarViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TimeIntervalBarViewModel> ObservationIntervalBarViewModels
        {
            get
            {
                return _observationIntervalBarViewModels;
            }
            set
            {
                _observationIntervalBarViewModels = value;
                RaisePropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeOperationIntervalsBars
        {
            get { return _includeOperationIntervalsBars; }
            set
            {
                _includeOperationIntervalsBars = value;
                RaisePropertyChanged();

                Refresh();
            }
        }

        public bool IncludeObservationIntervalsBars
        {
            get { return _includeObservationIntervalsBars; }
            set
            {
                _includeObservationIntervalsBars = value;
                RaisePropertyChanged();

                Refresh();
            }
        }

        public StationInformationCollectionViewModel(
            IUIDataProvider dataProvider,
            ObjectCollection<StationInformation> selectedStationInformations,
            ObservableObject<Dictionary<int, RowCharacteristics>> rowConditionMap)
        {
            _dataProvider = dataProvider;
            _includeOperationIntervalsBars = true;
            _includeObservationIntervalsBars = false;
            _selectedStationInformations = selectedStationInformations;
            _rowConditionMap = rowConditionMap;

            _selectedStationInformations.PropertyChanged += Initialize;
        }

        private void Initialize(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stationInformations = sender as ObjectCollection<StationInformation>;

            if (stationInformations == null || !stationInformations.Objects.Any())
            {
                _stationInformations = null;
                IsVisible = false;
                return;
            }

            _stationInformations = stationInformations.Objects;

            Refresh();

            IsVisible = true;
        }

        private void Refresh()
        {
            if (_stationInformations == null)
            {
                return;
            }

            var allSelectedStationInformation = _stationInformations.ToList();

            var nStations = allSelectedStationInformation.Count();
            var startTimes = new List<DateTime>();
            var endTimes = new List<DateTime>();

            if (IncludeOperationIntervalsBars)
            {
                startTimes.AddRange(_stationInformations
                    .Select(s => s.DateFrom)
                    .Where(d => d.HasValue)
                    .Select(d => d.Value));

                endTimes.AddRange(_stationInformations
                    .Select(s => s.DateTo)
                    .Where(d => d.HasValue)
                    .Select(d => d.Value));

                if (_stationInformations.Any(s => !s.DateTo.HasValue))
                {
                    endTimes.Add(DateTime.UtcNow.TruncateToMilliseconds());
                }
            }

            if (IncludeObservationIntervalsBars)
            {
                allSelectedStationInformation.ForEach(s =>
                {
                    var intervals = GetObservationIntervalsForStation(s);

                    if (intervals != null)
                    {
                        startTimes.AddRange(intervals.Select(i => i.Item1));
                        endTimes.AddRange(intervals.Select(i => i.Item2));
                    }
                });
            }

            var earliestTime = new DateTime(1953, 1, 1);
            var latestTime = DateTime.UtcNow.TruncateToMilliseconds();

            if (startTimes.Any())
            {
                earliestTime = startTimes.Min();
            }

            if (endTimes.Any())
            {
                latestTime = endTimes.Max();
            }

            var nYears = latestTime.Year - earliestTime.Year + 1;
            var startTimeOfEntireInterval = new DateTime(earliestTime.Year, 1, 1);
            var endTimeOfEntireInterval = new DateTime(latestTime.Year + 1, 1, 1);
            var totalNumberOfDaysForEntireInterval = (endTimeOfEntireInterval - startTimeOfEntireInterval).TotalDays;

            var widthPrYear = 120;
            //var widthPrYear = 24;
            var heightPrStationInformation = 20;

            ImageWidth = widthPrYear * nYears;
            ImageHeight = heightPrStationInformation * nStations;

            var timeLineViewModels = new List<TimeLineViewModel>();

            var year = earliestTime.Year;

            // Initialize the vertical lines that will mark where the individual years start
            while (year < endTimeOfEntireInterval.Year)
            {
                var x = ImageWidth * (new DateTime(year, 1, 1) - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;

                timeLineViewModels.Add(new TimeLineViewModel { X = x, Height = ImageHeight, Header = year.ToString() });

                year++;
            }

            // Turn them into an observable collection
            TimeLineViewModels = new ObservableCollection<TimeLineViewModel>(timeLineViewModels);

            var operationBarViewModels = new List<TimeIntervalBarViewModel>();
            var observationBarViewModels = new List<TimeIntervalBarViewModel>();

            // Make the bars showing the intervals where the stations are active
            // as well as the bars showing where there are observations
            for (var i = 0; i < nStations; i++)
            {
                var stationInformation = allSelectedStationInformation[i];

                if (IncludeOperationIntervalsBars)
                {
                    if (stationInformation.DateFrom.HasValue)
                    {
                        var dateFrom = stationInformation.DateFrom.Value;
                        DateTime dateTo = DateTime.UtcNow.TruncateToMilliseconds();

                        if (stationInformation.DateTo.HasValue)
                        {
                            dateTo = stationInformation.DateTo.Value;
                        }

                        var left = ImageWidth * (dateFrom - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var right = ImageWidth * (dateTo - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var width = right - left;

                        var rowCondition = _rowConditionMap.Object[stationInformation.GdbArchiveOid].RowCondition;

                        operationBarViewModels.Add(new TimeIntervalBarViewModel
                        {
                            Top = i * heightPrStationInformation,
                            Left = left,
                            Width = width,
                            Height = heightPrStationInformation,
                            Brush = _conditionToBrushMap[rowCondition]
                        });
                    }
                }

                if (IncludeObservationIntervalsBars)
                {
                    var intervals = GetObservationIntervalsForStation(stationInformation);

                    var barHeightRatio = 0.3;

                    if (intervals != null && intervals.Any())
                    {
                        // Make bars for the observation intervals (originating from ObsDB)
                        foreach (var interval in intervals)
                        {
                            var left = ImageWidth * (interval.Item1 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var right = ImageWidth * (interval.Item2 - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                            var width = right - left;

                            observationBarViewModels.Add(new TimeIntervalBarViewModel
                            {
                                Top = i * heightPrStationInformation + heightPrStationInformation * (0.5 - barHeightRatio / 2),
                                Left = left,
                                Width = width,
                                Height = heightPrStationInformation * barHeightRatio
                            });
                        }
                    }
                }
            }

            OperationIntervalBarViewModels = new ObservableCollection<TimeIntervalBarViewModel>(operationBarViewModels);
            ObservationIntervalBarViewModels = new ObservableCollection<TimeIntervalBarViewModel>(observationBarViewModels);
        }

        private List<Tuple<DateTime, DateTime>> GetObservationIntervalsForStation(
            StationInformation stationInformation)
        {
            var stationId = $"{stationInformation.StationIDDMI}".PadLeft(5, '0');
            var nanoqStationId = stationId.AsNanoqStationId();
            var parameter = "temp_dry";
            var maxTolerableDifferenceBetweenTwoObservationsInDays = 20.0;

            if (stationInformation.StationIDDMI.HasValue &&
                stationInformation.StationIDDMI.Value.ToString().Substring(0, 1) == "5")
            {
                parameter = "precip_past6h";
            }

            return _dataProvider.ReadObservationIntervalsForStation(
                nanoqStationId,
                parameter,
                maxTolerableDifferenceBetweenTwoObservationsInDays);
        }
    }
}
