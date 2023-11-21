using System;
using System.Globalization;
using System.Windows;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class TimeSeriesViewModel : CoordinateSystemViewModel
    {
        public DateTime TimeAtOrigo { get; }

        public ObservableObject<DateTime?> TimeAtMousePosition { get; }

        public TimeSeriesViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio,
            double marginX,
            double marginY,
            double expansionFactor,
            DateTime timeAtOrigo) : base(worldWindowFocus, worldWindowSize, fitAspectRatio, marginX, marginY, expansionFactor)
        {
            TimeAtOrigo = timeAtOrigo;

            TimeAtMousePosition = new ObservableObject<DateTime?>();

            GeometryEditorViewModel.MousePositionWorld.PropertyChanged += (s, e) =>
            {
                TimeAtMousePosition.Object = GeometryEditorViewModel.MousePositionWorld.Object.HasValue
                    ? TimeAtOrigo + TimeSpan.FromDays(GeometryEditorViewModel.MousePositionWorld.Object.Value.X)
                    : null;
            };
        }

        protected override void UpdateCoordinateSystemForGeometryEditorViewModel()
        {
            var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
            var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;
            var y0 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y - GeometryEditorViewModel.WorldWindowSize.Height;
            var y1 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y;

            // We want margins and thickness to be independent on scaling
            var dx = GeometryEditorViewModel.MarginLeft / GeometryEditorViewModel.Scaling.Width;
            var dy = GeometryEditorViewModel.MarginBottom / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            if (ShowHorizontalGridLines)
            {
                DrawHorizontalGridLinesAndOrLabels(x0, dx, thickness);
            }

            if (ShowVerticalGridLines)
            {
                DrawVerticalGridLinesAndOrLabels(x0, y0, x1, y1, dx, dy, thickness);
            }
        }

        protected virtual void DrawVerticalGridLinesAndOrLabels(
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

            double lineSpacingX_World = 1.0;
            TimeSpan delta = TimeSpan.FromDays(1);
            var showSeconds = false;
            var showMinutes = false;
            var showHours = false;
            var showDays = false;

            if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 60.0)
            {
                // Operer med en linie pr sekund
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 60.0;
                delta = TimeSpan.FromSeconds(1);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 30.0)
            {
                // Operer med en linie pr 2 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 30.0;
                delta = TimeSpan.FromSeconds(2);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 12.0)
            {
                // Operer med en linie pr 5 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 12.0;
                delta = TimeSpan.FromSeconds(5);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 6.0)
            {
                // Operer med en linie pr 10 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 6.0;
                delta = TimeSpan.FromSeconds(10);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 4.0)
            {
                // Operer med en linie pr 15 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 4.0;
                delta = TimeSpan.FromSeconds(15);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 3.0)
            {
                // Operer med en linie pr 20 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 3.0;
                delta = TimeSpan.FromSeconds(20);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 2.0)
            {
                // Operer med en linie pr 30 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 2.0;
                delta = TimeSpan.FromSeconds(30);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0)
            {
                // Operer med en linie pr minut
                lineSpacingX_World = 1.0 / 24.0 / 60.0;
                delta = TimeSpan.FromMinutes(1);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 30.0)
            {
                // Operer med en linie pr 2 minutter
                lineSpacingX_World = 1.0 / 24.0 / 30.0;
                delta = TimeSpan.FromMinutes(2);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 12.0)
            {
                // Operer med en linie pr 5 minutter
                lineSpacingX_World = 1.0 / 24.0 / 12.0;
                delta = TimeSpan.FromMinutes(5);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 6.0)
            {
                // Operer med en linie pr 10 minutter
                lineSpacingX_World = 1.0 / 24.0 / 6.0;
                delta = TimeSpan.FromMinutes(10);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 4.0)
            {
                // Operer med en linie pr 15 minutter
                lineSpacingX_World = 1.0 / 24.0 / 4.0;
                delta = TimeSpan.FromMinutes(15);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 2.0)
            {
                // Operer med en linie pr 30 minutter
                lineSpacingX_World = 1.0 / 24.0 / 2.0;
                delta = TimeSpan.FromMinutes(30);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0)
            {
                // Operer med en linie pr time
                lineSpacingX_World = 1.0 / 24.0;
                delta = TimeSpan.FromHours(1);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 12.0)
            {
                // Operer med en linie pr 2 timer
                lineSpacingX_World = 1.0 / 12.0;
                delta = TimeSpan.FromHours(2);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 8.0)
            {
                // Operer med en linie pr 3 timer
                lineSpacingX_World = 1.0 / 8.0;
                delta = TimeSpan.FromHours(3);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 6.0)
            {
                // Operer med en linie pr 4 timer
                lineSpacingX_World = 1.0 / 6.0;
                delta = TimeSpan.FromHours(4);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 4.0)
            {
                // Operer med en linie pr 6 timer
                lineSpacingX_World = 1.0 / 4.0;
                delta = TimeSpan.FromHours(6);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 2.0)
            {
                // Operer med en linie pr 12 timer
                lineSpacingX_World = 1.0 / 2.0;
                delta = TimeSpan.FromHours(12);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0)
            {
                // Operer med en linie pr dag
                lineSpacingX_World = 1.0;
                delta = TimeSpan.FromDays(1);
                showDays = true;
            }
            else if (lineSpacingX_World_Min < 1.0 * 2.0)
            {
                // Operer med en linie pr 2 dage
                lineSpacingX_World = 1.0 * 2.0;
                delta = TimeSpan.FromDays(2);
                showDays = true;
            }
            else if (lineSpacingX_World_Min < 1.0 * 5.0)
            {
                // Operer med en linie pr 5 dage
                lineSpacingX_World = 1.0 * 5.0;
                delta = TimeSpan.FromDays(5);
                showDays = true;
            }
            else if (lineSpacingX_World_Min < 1.0 * 10.0)
            {
                // Operer med en linie pr 10 dage
                lineSpacingX_World = 1.0 * 10.0;
                delta = TimeSpan.FromDays(10);
                showDays = true;
            }

            var labelWidth = lineSpacingX_World * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af x-værdien for første linie
            var x = Math.Floor(x0 / lineSpacingX_World) * lineSpacingX_World;
            var timeSpan = RoundSeconds(TimeSpan.FromDays(x));
            var t = TimeAtOrigo + timeSpan; 

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    if (ShowVerticalGridLines)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            thickness,
                            _gridBrush);
                    }

                    var month = t.Date.Month;
                    var day = t.Date.Day;
                    var hour = t.Hour;
                    var minute = t.Minute;
                    var second = t.Second;

                    string label = "x";

                    if (showSeconds)
                    {
                        label = $"{hour}:{minute.ToString().PadLeft(2, '0')}:{second.ToString().PadLeft(2, '0')}";
                    }
                    else if (showMinutes)
                    {
                        label = $"{hour}:{minute.ToString().PadLeft(2, '0')}";
                    }
                    else if (showHours)
                    {
                        label = $"{hour}:00";
                    }
                    else if (showDays)
                    {
                        label = $"{day}/{month}";
                    }

                    GeometryEditorViewModel.AddLabel(
                        label,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);

                    // Denne blev brugt til at skrive en overordnet label ved hvert månedsskifte
                    if (day == 1 && false)  
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

                x += lineSpacingX_World;
                t += delta;
            }
        }

        // Helper for rounding Timespan to closest timespan where second is zero
        private static TimeSpan RoundSeconds(TimeSpan span)
        {
            return TimeSpan.FromSeconds(Math.Round(span.TotalSeconds));
        }
    }
}
