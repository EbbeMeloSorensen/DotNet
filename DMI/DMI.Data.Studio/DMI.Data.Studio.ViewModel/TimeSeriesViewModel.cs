using System;
using System.Collections.Generic;
using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using GalaSoft.MvvmLight;

namespace DMI.Data.Studio.ViewModel
{
    // Denne klasse:
    //   HAR en ScatterChartViewModel
    //   BESTEMMER, hvad tidspunkt origo (x = 0) svarer til, samt hvad x = 1 svarer til
    //   BESTEMMER initiel position af World Window (efterfølgende kommunikeres brugerens justeringer af WorldWindow fra ScatterChartViewModel)
    //   Når der sker en "major" opdatering af World Window, så hentes nye tidsseriedata fra datakilden
    public class TimeSeriesViewModel : ViewModelBase
    {
        private DateTime _dateTimeAtOrigo;
        private TimeSpan _timeSpanForXUnit;

        public string Greeting { get; set; }

        public ScatterChartViewModel ScatterChartViewModel { get; set; }

        public TimeSeriesViewModel()
        {
            Greeting = "Greetings from TimeSeriesViewModel";

            _dateTimeAtOrigo = DateTime.UtcNow - TimeSpan.FromDays(7);
            _timeSpanForXUnit = TimeSpan.FromDays(1);

            ScatterChartViewModel = new ScatterChartViewModel(
                (x0, x1) => GeneratePoints(x0, x1), 38, 38, -7, -4);

            DrawACoordinateSystem(ScatterChartViewModel);
        }

        private void DrawACoordinateSystem(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Coordinate System
            var coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
            var coordinateSystemThickness = 0.05;

            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-4, 0), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(3.7, -0.2), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(3.7, 0.2), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -3), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 2.7), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 3), new PointD(0.2, 2.7), coordinateSystemThickness, coordinateSystemBrush);
        }

        private List<PointD> GeneratePoints(
            double x0, 
            double x1)
        {
            var points = new List<PointD>();

            for (var x = x0; x <= x1; x += 0.1)
            {
                points.Add(new PointD(x, Math.Sin(x)));
            }

            return points;
        }
    }
}
