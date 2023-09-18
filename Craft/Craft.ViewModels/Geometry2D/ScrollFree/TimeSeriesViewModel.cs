using System;
using System.Globalization;
using System.Windows;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class TimeSeriesViewModel : CoordinateSystemViewModel
    {
        private DateTime _timeAtOrigo;
        private DateTime _timeAtCursor;

        public DateTime TimeAtCursor
        {
            get { return _timeAtCursor; }
            set
            {
                _timeAtCursor = value;
                RaisePropertyChanged();
            }
        }

        public TimeSeriesViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio,
            double marginX,
            double marginY,
            DateTime timeAtOrigo) : base(worldWindowFocus, worldWindowSize, fitAspectRatio, marginX, marginY)
        {
            _marginX = marginX;
            _marginY = marginY;
            _timeAtOrigo = timeAtOrigo;
            _timeAtCursor = new DateTime(1975, 7, 24, 0, 0, 0);
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
            var lineSpacingX_ViewPort_Min = 75.0;
            var lineSpacingX_World_Min = lineSpacingX_ViewPort_Min / GeometryEditorViewModel.Scaling.Width;
            var lineSpacingX_World = Math.Pow(10, Math.Ceiling(Math.Log10(lineSpacingX_World_Min)));

            // Her er lineSpacingX_World f.eks. 0.01, 0.1, 1, 10, 100 eller 1000

            // 1: Find ud af spacing af linier for x-aksen
            var spacingX = 1.0;
            var labelWidth = spacingX * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi (for en linie)
            var x = Math.Floor(x0 / spacingX) * spacingX;

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    var t = _timeAtOrigo + TimeSpan.FromDays(x);
                    var day = t.Date.Day;

                    if (lineSpacingX_World > 10.1 && day != 1)
                    {
                        x += spacingX;
                        continue;
                    }
                    else if (lineSpacingX_World > 1.1 && (day != 1 && day % 5 != 0 || day > 25))
                    {
                        x += spacingX;
                        continue;
                    }

                    if (_includeGrid)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            thickness,
                            _gridBrush);
                    }

                    var dateText = day.ToString();

                    GeometryEditorViewModel.AddLabel(
                        dateText,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);

                    if (day == 1)
                    {
                        var monthText = t.Date.ToString("MMM", CultureInfo.InvariantCulture);

                        GeometryEditorViewModel.AddLabel(
                            monthText,
                            new PointD(x, y0 + dy),
                            labelWidth,
                            labelHeight,
                            new PointD(0, 1.5 * labelHeight),
                            0.0);

                        var yearText = t.Year.ToString();

                        GeometryEditorViewModel.AddLabel(
                            yearText,
                            new PointD(x, y0 + dy),
                            labelWidth,
                            labelHeight,
                            new PointD(0, 2.5 * labelHeight),
                            0.0);
                    }
                }

                x += spacingX;
            }
        }
    }
}
