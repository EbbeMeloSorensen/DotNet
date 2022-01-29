using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Charts;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Application;

namespace DMI.SMS.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly ObservableObject<Dictionary<int, RowCharacteristics>> _rowCharacteristicsMap;

        public PieChartViewModel StationTypePieChartViewModel { get; private set; }
        public PieChartViewModel StationOwnerPieChartViewModel { get; private set; }
        public PieChartViewModel StationStatusPieChartViewModel { get; private set; }
        public PieChartViewModel StationBusinessRuleViolationsPieChartViewModel { get; private set; }

        public StatisticsViewModel(
            ObjectCollection<StationInformation> StationInformations,
            ObservableObject<Dictionary<int, RowCharacteristics>> rowCharacteristicsMap)
        {
            _rowCharacteristicsMap = rowCharacteristicsMap;

            StationTypePieChartViewModel = new PieChartViewModel();
            StationOwnerPieChartViewModel = new PieChartViewModel();
            StationStatusPieChartViewModel = new PieChartViewModel();
            StationBusinessRuleViolationsPieChartViewModel = new PieChartViewModel();

            StationInformations.PropertyChanged += StationInformations_PropertyChanged;
        }

        private void StationInformations_PropertyChanged(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stationInformations = (sender as ObjectCollection<StationInformation>).Objects.ToList();

            InitializeStationTypePieChart(stationInformations);
            InitializeStationOwnerPieChartViewModel(stationInformations);
            InitializeStationStatusPieChartViewModel(stationInformations);
            InitializeStationBusinessRuleViolationsPieChartViewModel(stationInformations);
        }

        private void InitializeStationTypePieChart(
            IList<StationInformation> stationInformations)
        {
            var title = "Station Types";
            var diameter = 300;
            var distribution = new Dictionary<string, double>();
            var count = 0;

            foreach (StationType stationType in (StationType[])Enum.GetValues(typeof(StationType)))
            {
                count = stationInformations.Count(s => s.Stationtype == stationType);

                if (count == 0)
                {
                    continue;
                }

                distribution[stationType.ToString()] = count;
            }

            count = stationInformations.Count(s => !s.Stationtype.HasValue);

            if (count > 0)
            {
                distribution["Unspecified"] = count;
            }

            StationTypePieChartViewModel.Initialize(title, diameter, distribution);
        }

        private void InitializeStationOwnerPieChartViewModel(
            IList<StationInformation> stationInformations)
        {
            var title = "Station Owners";
            var diameter = 300;
            var distribution = new Dictionary<string, double>();
            var count = 0;

            foreach (StationOwner stationOwner in (StationOwner[])Enum.GetValues(typeof(StationOwner)))
            {
                count = stationInformations.Count(s => s.StationOwner == stationOwner);

                if (count == 0)
                {
                    continue;
                }

                distribution[stationOwner.ToString()] = count;
            }

            count = stationInformations.Count(s => !s.StationOwner.HasValue);

            if (count > 0)
            {
                distribution["Unspecified"] = count;
            }

            StationOwnerPieChartViewModel.Initialize(title, diameter, distribution);
        }

        private void InitializeStationStatusPieChartViewModel(
            IList<StationInformation> stationInformations)
        {
            var title = "Station Status";
            var diameter = 300;
            var distribution = new Dictionary<string, double>();

            distribution["Active"] = stationInformations.Count(s => s.Status == Status.Active);
            distribution["Inactive"] = stationInformations.Count(s => s.Status == Status.Inactive);

            var unspecifiedCount = stationInformations.Count(s => !s.Status.HasValue);

            if (unspecifiedCount > 0)
            {
                distribution["Unspecified"] = unspecifiedCount;
            }

            StationStatusPieChartViewModel.Initialize(title, diameter, distribution);
        }

        private void InitializeStationBusinessRuleViolationsPieChartViewModel(
            IList<StationInformation> stationInformations)
        {
            var title = "Business Rule violations";
            var diameter = 300;
            var distribution = new Dictionary<string, double>();
            var count = 0;

            var okCount = 0;
            var inViolationCount = 0;

            foreach (var stationInformation in stationInformations)
            {
                var characteristics = _rowCharacteristicsMap.Object[stationInformation.GdbArchiveOid];

                if (characteristics.RowCondition != RowCondition.Current)
                {
                    continue;
                }

                if (characteristics.ViolatedBusinessRules.Count == 0)
                {
                    okCount++;
                }
                else
                {
                    inViolationCount++;
                }
            }

            distribution["OK"] = okCount;
            distribution["In violation"] = inViolationCount;

            var customPalette = new List<Brush>
            {
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Red)
            };

            StationBusinessRuleViolationsPieChartViewModel.OverrideDefaultPalette(customPalette);

            StationBusinessRuleViolationsPieChartViewModel.Initialize(title, diameter, distribution);
        }
    }
}
