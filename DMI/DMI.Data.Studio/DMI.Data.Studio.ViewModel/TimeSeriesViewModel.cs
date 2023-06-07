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

            _dateTimeAtOrigo = DateTime.UtcNow.Date - TimeSpan.FromDays(7);
            _timeSpanForXUnit = TimeSpan.FromDays(1);

            var dateTimeAtRightSideOfView = (DateTime.UtcNow.Date + TimeSpan.FromDays(1)).Date;

            var xRight  = (dateTimeAtRightSideOfView - _dateTimeAtOrigo) / _timeSpanForXUnit;

            ScatterChartViewModel = new ScatterChartViewModel(
                (x0, x1) => GeneratePoints(x0, x1), 38, 38, 0, 1);

            // Todo: Man skal kunne sætte WorldWindow, så det initielt starter med at dække 7 hele dage, inkl hele den igangværende dag
            // - Det er nok nødvendigt at tage udgangspukt i den første reelle viewport, der kommer ind..
            // Man kunne f.eks. fodre den et "foretrukkent" WorldWindow, som den så vil tilstræbe, når der lander en viewport.
            // Det kunne også passende være et centerpunkt og en bredde, og så skulle den selv beregne WorldWindow samt magnification ud fra det.

            DrawACoordinateSystem(ScatterChartViewModel);
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

        private List<PointD> GeneratePoints(
            double x0, 
            double x1)
        {
            var points = new List<PointD>();

            if (x1 <= x0)
            {
                return points;
            }

            // Find det time interval that corresponds to the World Window
            var t0 = _dateTimeAtOrigo + x0 * (_timeSpanForXUnit);
            var t1 = _dateTimeAtOrigo + x1 * (_timeSpanForXUnit);

            for (var t = t0; t <= t1; t += new TimeSpan(0, 15, 0))
            {
                // Find the x coordinate that corresponds to the current time
                var x = (t - _dateTimeAtOrigo) / _timeSpanForXUnit;

                // Vi viser bare en værdi der svarer til timetallet for det pågældende tidspunkt delt med 24
                points.Add(new PointD(x, 1.0 * t.Hour / 24));
            }

            return points;
        }
    }
}
