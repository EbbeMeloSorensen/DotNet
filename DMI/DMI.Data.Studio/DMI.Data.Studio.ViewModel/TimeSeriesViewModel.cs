using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Application;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace DMI.Data.Studio.ViewModel
{
    // Denne klasse:
    //   - HAR en TimeSeriesViewModel - ja det hedder det samme for now
    //   - BESTEMMER, hvad tidspunkt origo (x = 0) svarer til, samt hvad x = 1 svarer til
    //   - BESTEMMER initiel position af World Window (efterfølgende kommunikeres brugerens justeringer af WorldWindow fra ScatterChartViewModel)
    //   Når der sker en "major" opdatering af World Window, så hentes nye tidsseriedata fra datakilden
    public class TimeSeriesViewModel : ViewModelBase
    {
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;
        private readonly IUIDataProvider _smsDataProvider;
        private readonly ObjectCollection<StationInformation> _selectedStationInformations;
        private DateTime _timeAtOrigo;
        private TimeSpan _timeSpanForXUnit = TimeSpan.FromDays(1);
        private string _stationId;
        private string _directoryName;
        private string _searchPattern;
        private DateTime _t0;
        private DateTime _t1;

        public Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel ScatterChartViewModel { get; set; }

        public TimeSeriesViewModel(
            IUIDataProvider smsDataProvider,
            ObjectCollection<StationInformation> selectedStationInformations)
        {
            _smsDataProvider = smsDataProvider;
            _selectedStationInformations = selectedStationInformations;

            var timeWindow = TimeSpan.FromDays(7);
            var utcNow = DateTime.UtcNow;
            _timeAtOrigo = utcNow.Date - TimeSpan.FromDays(7);
            _timeAtOrigo = new DateTime(2014, _timeAtOrigo.Month, _timeAtOrigo.Day);
            var tFocus = _timeAtOrigo + timeWindow / 2;
            var xFocus = (tFocus - _timeAtOrigo) / TimeSpan.FromDays(1.0);

            ScatterChartViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 10),
                new Size(14, 30),
                true,
                25,
                60,
                _timeAtOrigo);

            ScatterChartViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured +=
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            _selectedStationInformations.PropertyChanged += _selectedStationInformations_PropertyChanged;
        }

        private void _selectedStationInformations_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var stationInformations = sender as ObjectCollection<StationInformation>;

            if (stationInformations == null || stationInformations.Objects.Count() == 0)
            {
                _stationId = null;
                _directoryName = null;
                _searchPattern = null;
            }
            else if (stationInformations.Objects.Count() == 1)
            {
                _stationId = $"{stationInformations.Objects.Single().StationIDDMI}00";
                _directoryName = Path.Combine(@"C:\\Data\\Observations", $"{_stationId}", "temp_dry");
                _searchPattern = $"{_stationId}_temp_dry_2014.txt";
            }

            UpdateCurve();
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            Craft.ViewModels.Geometry2D.ScrollFree.WorldWindowUpdatedEventArgs e)
        {
            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();

            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

            _t0 = _timeAtOrigo + x0 * (_timeSpanForXUnit);
            _t1 = _timeAtOrigo + x1 * (_timeSpanForXUnit);

            UpdateCurve();
        }

        private void UpdateCurve()
        {
            if (_stationId == null)
            {
                return;
            }

            var observations = _smsDataProvider
                .ReadObservationsForStation(_directoryName, _searchPattern)
                .Where(o => o.Item1 >= _t0)
                .Where(o => o.Item1 <= _t1)
                .OrderBy(o => o.Item1);

            var points = new List<PointD>();

            foreach (var observation in observations)
            {
                var t = observation.Item1;

                // Find the x coordinate that corresponds to the current time
                var x = (t - _timeAtOrigo) / _timeSpanForXUnit;

                // Add the point to the polyline
                points.Add(new PointD(x, observation.Item2));
            }

            ScatterChartViewModel.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
        }
    }
}
