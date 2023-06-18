using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using GalaSoft.MvvmLight;

namespace DMI.Data.Studio.ViewModel
{
    // Denne klasse:
    //   - HAR en ScatterChartViewModel
    //   - BESTEMMER, hvad tidspunkt origo (x = 0) svarer til, samt hvad x = 1 svarer til
    //   - BESTEMMER initiel position af World Window (efterfølgende kommunikeres brugerens justeringer af WorldWindow fra ScatterChartViewModel)
    //   Når der sker en "major" opdatering af World Window, så hentes nye tidsseriedata fra datakilden
    public class TimeSeriesViewModel : ViewModelBase
    {
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;

        private DateTime _dateTimeAtOrigo;
        private TimeSpan _timeSpanForXUnit;

        public string Greeting { get; set; }

        public GeometryEditorViewModel ScatterChartViewModel { get; set; }

        public TimeSeriesViewModel()
        {
            Greeting = "Greetings from TimeSeriesViewModel";

            _dateTimeAtOrigo = DateTime.UtcNow.Date - TimeSpan.FromDays(7);
            _timeSpanForXUnit = TimeSpan.FromDays(1);

            var dateTimeAtRightSideOfView = (DateTime.UtcNow.Date + TimeSpan.FromDays(1)).Date;

            var x0 = 0;
            var x1 = (dateTimeAtRightSideOfView - _dateTimeAtOrigo) / _timeSpanForXUnit;
            var y0 = 0.0;
            var y1 = 2.0;

            var worldWindowFocus = new Point(
                (x0 + x1) / 2,
                (y0 + y1) / 2);

            var worldWindowSize = new Size(
                x1 - x0,
                y1 - y0);

            ScatterChartViewModel = new GeometryEditorViewModel(
                -1, worldWindowFocus, worldWindowSize);

            ScatterChartViewModel.WorldWindowMajorUpdateOccured += ScatterChartViewModel_WorldWindowMajorUpdateOccured;

            DrawACoordinateSystem(ScatterChartViewModel);
        }

        private void ScatterChartViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            var points = new List<PointD>();

            // Find the time interval that corresponds to the World Window
            var x0 = e.WorldWindowUpperLeft.X;
            var x1 = x0 + e.WorldWindowSize.Width;
            var t0 = _dateTimeAtOrigo + x0 * (_timeSpanForXUnit);
            var t1 = _dateTimeAtOrigo + x1 * (_timeSpanForXUnit);

            for (var t = t0; t <= t1; t += new TimeSpan(0, 15, 0))
            {
                // Find the x coordinate that corresponds to the current time
                var x = (t - _dateTimeAtOrigo) / _timeSpanForXUnit;

                // Vi viser bare en værdi der svarer til timetallet for det pågældende tidspunkt delt med 24
                points.Add(new PointD(x, 1.0 * t.Hour / 24));
            }

            ScatterChartViewModel.ClearPolylines();
            ScatterChartViewModel.AddPolyline(points, _curveThickness, _curveBrush);
        }

        private void DrawACoordinateSystem(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Coordinate System
            var coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
            var coordinateSystemThickness = 0.05;

            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-10, 0), new PointD(10, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(9.7, -0.2), new PointD(10, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(9.7, 0.2), new PointD(10, 0), coordinateSystemThickness, coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -3), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 2.7), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 3), new PointD(0.2, 2.7), coordinateSystemThickness, coordinateSystemBrush);

            // X Axis ticks
            for (var x = -9; x <= 9; x++)
            {
                geometryEditorViewModel.AddLine(new PointD(x, -0.2), new PointD(x, 0.2), coordinateSystemThickness, coordinateSystemBrush);
            }
        }
    }
}
