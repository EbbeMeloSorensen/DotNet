using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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
        private DateTime _timeAtOrigo;
        private TimeSpan _timeSpanForXUnit = TimeSpan.FromDays(1);

        public Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel ScatterChartViewModel { get; set; }

        public TimeSeriesViewModel(
            IUIDataProvider smsDataProvider)
        {
            _smsDataProvider = smsDataProvider;

            var timeWindow = TimeSpan.FromDays(7);
            var utcNow = DateTime.UtcNow;
            _timeAtOrigo = utcNow.Date - TimeSpan.FromDays(7);
            var tFocus = utcNow - timeWindow / 2;
            var xFocus = (tFocus - _timeAtOrigo) / TimeSpan.FromDays(1.0);

            ScatterChartViewModel = new Craft.ViewModels.Geometry2D.ScrollFree.TimeSeriesViewModel(
                new Point(xFocus, 0),
                new Size(7, 0.001),
                25,
                60,
                _timeAtOrigo);

            ScatterChartViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured +=
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            Craft.ViewModels.Geometry2D.ScrollFree.WorldWindowUpdatedEventArgs e)
        {
            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

            var directoryName = @"C:\\Data\\Observations\\06041";
            var searchPattern = "temp_dry_06041_2023.txt";
            var observations = _smsDataProvider.ReadObservationsForStation(directoryName, searchPattern);

            var t0 = _timeAtOrigo + x0 * (_timeSpanForXUnit);
            var t1 = _timeAtOrigo + x1 * (_timeSpanForXUnit);

            var points = new List<PointD>();

            foreach (var observation in observations)
            {
                var t = observation.Item1;

                if (t < t0 || t > t1)
                {
                    continue;
                }

                // Find the x coordinate that corresponds to the current time
                var x = (t - _timeAtOrigo) / _timeSpanForXUnit;

                // Add the point to the polyline
                points.Add(new PointD(x, observation.Item2));
            }

            ScatterChartViewModel.GeometryEditorViewModel.ClearPolylines();
            ScatterChartViewModel.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
        }
    }
}
