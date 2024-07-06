using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Chronology;

namespace Craft.UIElements.GuiTest.Tab4
{
    public class Tab4ViewModel : ViewModelBase
    {
        private string _timeAtCursor;

        public string TimeAtCursor
        {
            get { return _timeAtCursor; }
            set
            {
                _timeAtCursor = value;
                RaisePropertyChanged();
            }
        }

        public ChronologyViewModel ChronologyViewModel { get; private set; }

        public Tab4ViewModel()
        {
            var startTime = DateTime.UtcNow.TruncateToMilliseconds() - new TimeSpan(0, 15, 0);
            var endTime = DateTime.UtcNow.TruncateToMilliseconds() + new TimeSpan(0, 15, 0);
            ChronologyViewModel = new ChronologyViewModel(startTime, endTime, 50, 240);
            DrawSomeDummyStuff();

            TimeAtCursor = "Coming soon: Time at cursor";
        }

        private void DrawSomeDummyStuff()
        {
            //var dummyData = new List<Tuple<string, DateTime, DateTime>>
            //{
            //    new Tuple<string, DateTime, DateTime>("Bamse", new DateTime(2012, 7, 24), new DateTime(2019, 1, 15)),
            //    new Tuple<string, DateTime, DateTime>("Kylling", new DateTime(2019, 1, 15), new DateTime(2019, 10, 7)),
            //    new Tuple<string, DateTime, DateTime>("Luna", new DateTime(2019, 10, 7), new DateTime(9999, 12, 31)),
            //};

            var dummyData = new List<Tuple<string, DateTime, DateTime>>
            {
                new Tuple<string, DateTime, DateTime>("Bamse", new DateTime(2021, 8, 15, 0, 0, 0), new DateTime(2021, 8, 15, 4, 0, 0)),
                new Tuple<string, DateTime, DateTime>("Kylling", new DateTime(2021, 8, 16, 0, 0, 0), new DateTime(2021, 9, 21, 15, 4, 12)),
                new Tuple<string, DateTime, DateTime>("Luna", new DateTime(2021, 9, 21, 15, 4, 12), new DateTime(9999, 12, 31, 23, 59, 59)),
            };

            var earliestTime = dummyData.Min(x => x.Item2);
            int nTimeIntervalBars = 3;

            var latestTime = DateTime.Now;
            var nYears = latestTime.Year - earliestTime.Year + 1;
            var startTimeOfEntireInterval = new DateTime(earliestTime.Year, 1, 1);
            var endTimeOfEntireInterval = new DateTime(latestTime.Year + 1, 1, 1);
            //var endTimeOfEntireInterval = new DateTime(latestTime.Year + 4, 1, 1); // We include 3 extra years in the future to make sure there is room for labels regarding updated fields
            var totalNumberOfDaysForEntireInterval = (endTimeOfEntireInterval - startTimeOfEntireInterval).TotalDays;

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

            // Initialize the time interval bars
            var positionCount = 0;
            foreach (var entry in dummyData)
            {
                var startTime = entry.Item2;
                var endTime = DateTime.Now;

                if (entry.Item3 < endTime)
                {
                    endTime = entry.Item3;
                }

                var leftOfBar = widthOfLaneLabelColumn + totalWidthOfMainPart * (startTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                var right = widthOfLaneLabelColumn + totalWidthOfMainPart * (endTime - startTimeOfEntireInterval).TotalDays / totalNumberOfDaysForEntireInterval;
                var width = right - leftOfBar;

                ChronologyViewModel.TimeIntervalBarViewModels.Add(new TimeIntervalBarViewModel
                {
                    Label = entry.Item1,
                    Top = heightOfHeader + positionCount * heightPrPositionRecord,
                    LeftOfBar = leftOfBar,
                    Width = width,
                    Height = heightPrPositionRecord,
                    Brush = new SolidColorBrush(Colors.Gray),
                    LabelBrush = new SolidColorBrush(Colors.Black)
                });

                positionCount++;
            }
        }
    }
}
