using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D;
using Craft.ViewModels.Chronology;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Application;

namespace DMI.StatDB.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private string _mainWindowTitle;
        private Brush _pointBrush = new SolidColorBrush(Colors.DarkRed);

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public StationListViewModel StationListViewModel { get; private set; }
        public ChronologyViewModel ChronologyViewModel { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; private set; }

        public MainWindowViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            _mainWindowTitle = "StatDB Studio";

            StationListViewModel = new StationListViewModel(
                dataProvider,
                applicationDialogService);

            StationListViewModel.SelectedStations.PropertyChanged += SelectedStations_PropertyChanged;

            ChronologyViewModel = new ChronologyViewModel(new DateTime(2015, 1, 1), DateTime.UtcNow.TruncateToMilliseconds(), 50, 240);

            GeometryEditorViewModel = new GeometryEditorViewModel();
            GeometryEditorViewModel.Magnification = 240;
            GeometryEditorViewModel.WorldWindowUpperLeft = new System.Windows.Point(7.4, 53.9);

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
        }

        private void SelectedStations_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            var stations = sender as ObjectCollection<Station>;

            GeometryEditorViewModel.ClearPoints();
            ChronologyViewModel.VerticalLineViewModels.Clear();
            ChronologyViewModel.TimeIntervalBarViewModels.Clear();

            if (stations == null || !stations.Objects.Any())
            {
                ChronologyViewModel.IsVisible = false;
                return;
            }

            ChronologyViewModel.IsVisible = true;

            // Count total number of position records
            var nPositions = stations.Objects.Sum(station => station.Positions.Count);

            // Identify earliest time
            var startTimes = new List<DateTime>();

            foreach (var station in stations.Objects)
            {
                foreach (var position in station.Positions)
                {
                    startTimes.Add(position.StartTime);
                }
            }

            var earliestTime = new DateTime(1889, 1, 1);

            if (startTimes.Any())
            {
                earliestTime = startTimes.Min();
            }

            var latestTime = DateTime.UtcNow.TruncateToMilliseconds();
            var nYears = latestTime.Year - earliestTime.Year + 1;
            var startTimeOfEntireInterval = new DateTime(earliestTime.Year, 1, 1);
            var endTimeOfEntireInterval = new DateTime(latestTime.Year + 1, 1, 1);

            var totalNumberOfDaysForEntireInterval = (endTimeOfEntireInterval - startTimeOfEntireInterval).TotalDays;

            var widthOfLabelColumn = 50;
            var widthPrYear = 120;
            var heightOfHeader = 20;
            var heightPrPositionRecord = 20;
            var totalWidthOfMainPart = widthPrYear * nYears;
            var totalHeightOfMainPart = heightPrPositionRecord * nPositions;
            var imageWidth = widthOfLabelColumn + totalWidthOfMainPart;
            var imageHeight = heightOfHeader + totalHeightOfMainPart;

            ChronologyViewModel.ImageWidth = imageWidth;
            ChronologyViewModel.ImageHeight = imageHeight;

            // Initialize the vertical lines that will mark where the individual years start
            var year = earliestTime.Year;
            while (year < endTimeOfEntireInterval.Year)
            {
                var x = widthOfLabelColumn + totalWidthOfMainPart * (new DateTime(year, 1, 1) - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;

                ChronologyViewModel.VerticalLineViewModels.Add(new VerticalLineViewModel
                {
                    X = x,
                    Header = year.ToString(),
                    Height = imageHeight

                });

                year++;
            }

            // Initialize the time interval bars
            var positionCount = 0;
            foreach (var station in stations.Objects)
            {
                foreach (var position in station.Positions)
                {
                    if (position.Long.HasValue &&
                        position.Lat.HasValue)
                    {
                        var point = new Point2D(
                            position.Long.Value,
                            position.Lat.Value);

                        GeometryEditorViewModel.AddPoint(point, 20, _pointBrush);
                    }

                    //if (position.StartTime.HasValue)
                    //{
                        var startTime = position.StartTime;
                        var endTime = DateTime.UtcNow.TruncateToMilliseconds();

                        if (position.EndTime.HasValue &&
                            position.EndTime.Value < endTime)
                        {
                            endTime = position.EndTime.Value;
                        }

                        var leftOfBar = widthOfLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var right = widthOfLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                        var width = right - leftOfBar;

                        ChronologyViewModel.TimeIntervalBarViewModels.Add(new TimeIntervalBarViewModel
                        {
                            Label = station.StatID.ToString(),
                            Top = heightOfHeader + positionCount * heightPrPositionRecord,
                            LeftOfBar = leftOfBar,
                            Width = width,
                            Height = heightPrPositionRecord,
                            Brush = new SolidColorBrush(Colors.Gray)
                        });
                    //}

                    positionCount++;
                }
            }
        }
    }
}
