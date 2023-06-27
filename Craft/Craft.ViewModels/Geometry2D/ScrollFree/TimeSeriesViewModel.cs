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

                    //GeometryEditorViewModel.AddLabel(
                    //    text,
                    //    new PointD(x, y0 + dy),
                    //    labelWidth,
                    //    labelHeight,
                    //    new PointD(0, labelHeight / 2),
                    //    0.0);

                    // Place label between ticks
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
