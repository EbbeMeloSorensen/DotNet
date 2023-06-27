using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateSystemViewModel
    {
        private bool _includeGrid = true;
        private bool _includeTicks = false;
        private Brush _coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
        private Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.25 };
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize);

            GeometryEditorViewModel.WorldWindowMajorUpdateOccured += 
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            GeometryEditorViewModel.WorldWindowUpdateOccured += 
                GeometryEditorViewModel_WorldWindowUpdateOccured;
        }

        private void GeometryEditorViewModel_WorldWindowUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            GeometryEditorViewModel.ClearLabels();
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            UpdateCoordinateSystemForGeometryEditorViewModel(
                e.WorldWindowUpperLeft.X,
                e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width,
                -e.WorldWindowUpperLeft.Y - e.WorldWindowSize.Height,
                -e.WorldWindowUpperLeft.Y);

            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

            var points = new List<PointD>();
            for (var x = x0; x <= x1; x += 0.1)
            {
                //points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                points.Add(new PointD(x, Math.Exp(-0.01 * x * x) * Math.Sin(3 * x))); // (gaussian and sinus)
            }

            GeometryEditorViewModel.ClearPolylines();
            GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
        }

        private void UpdateCoordinateSystemForGeometryEditorViewModel(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            // We want thickness to be independent on scaling
            var dx = 20 / GeometryEditorViewModel.Scaling.Width;
            var dy = 20 / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            // 1: Find ud af spacing af ticks for x-aksen
            var spacingX = 1.0;
            var labelWidth = spacingX * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi
            var x = Math.Floor(x0 / spacingX) * spacingX;

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            thickness,
                            _gridBrush);
                    }

                    if (_includeTicks)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, y0 + dy * 0.8),
                            new PointD(x, y0 + dy * 1.2),
                            thickness,
                            _coordinateSystemBrush);
                    }

                    var text = x.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);
                }

                x += spacingX;
            }

            // 1: Find ud af spacing af ticks for y-aksen
            var spacingY = 1.0;

            // Find ud af første y-værdi
            var y = Math.Floor(y0 / spacingY) * spacingY;

            while (y < y1)
            {
                if (y > y0 + dy)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x0 + dx, y),
                            new PointD(x1, y),
                            thickness,
                            _gridBrush);
                    }

                    if (_includeTicks)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x0 + dx * 0.8, y),
                            new PointD(x0 + dx * 1.2, y),
                            thickness,
                            _coordinateSystemBrush);
                    }

                    var text = y.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0);
                }

                y += spacingY;
            }
        }
    }
}
