using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class TimeSeriesViewModel : CoordinateSystemViewModel
    {
        public TimeSeriesViewModel(
            Point worldWindowFocus,
            Size worldWindowSize) : base(worldWindowFocus, worldWindowSize)
        {
            _marginX = 25;
            _marginY = 50;
        }

        protected override void UpdateCoordinateSystemForGeometryEditorViewModel(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            // We want thickness to be independent on scaling
            var dx = _marginX / GeometryEditorViewModel.Scaling.Width;
            var dy = _marginY / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            DrawHorizontalGridLines(x0, y0, x1, y1, dx, dy, thickness);
            DrawVerticalGridLines(x0, y0, x1, y1, dx, dy, thickness);
        }

        protected virtual void DrawVerticalGridLines(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness)
        {
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

                    var text = x.ToString(CultureInfo.InvariantCulture);

                    // Place DATE label (between ticks)
                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(labelWidth / 2, labelHeight / 2),
                        0.333);
                }

                x += spacingX;
            }
        }
    }
}
