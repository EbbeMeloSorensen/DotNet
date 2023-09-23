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
            DateTime timeAtOrigo) : base(worldWindowFocus, worldWindowSize, fitAspectRatio, marginX, marginY)
        {
            _marginX = marginX;
            _marginY = marginY;
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

            // We want thickness to be independent on scaling
            var dx = _marginX / GeometryEditorViewModel.Scaling.Width;
            var dy = _marginY / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            if (ShowHorizontalGridLines)
            {
                DrawHorizontalGridLines(x0, y0, x1, y1, dx, dy, thickness);
            }

            if (ShowVerticalGridLines)
            {
                DrawVerticalGridLines(x0, y0, x1, y1, dx, dy, thickness);
            }
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
            // Jo mere man zoomer ind, jo større bliver scaling, og desto mindre bliver variablen lineSpacingX_World_Min,
            // som udtrykker, hvor stor afstanden mellem vertikale gitterlinier mindst vil skulle være i World koordinater.
            // Dvs hvis man har zoomet meget ind, så er der også mulighed for at operere med en høj densitet af gitterlinier.
            // Konstruktionen nedenfor svarer til den, der er lavet for et almindeligt koordinatsystem, men det er nok
            // ikke så smart for tidsserier..

            var lineSpacingX_ViewPort_Min = 75.0;
            var lineSpacingX_World_Min = lineSpacingX_ViewPort_Min / GeometryEditorViewModel.Scaling.Width;

            var lineSpacingX_World = 1.0;
            var delta = TimeSpan.FromDays(1);

            // Noget alla det her er nok bedre
            if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 60.0)
            {
                // Operer med en linie pr sekund
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 60.0;
                delta = TimeSpan.FromSeconds(1);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0)
            {
                // Operer med en linie pr minut
                lineSpacingX_World = 1.0 / 24.0 / 60.0;
                delta = TimeSpan.FromMinutes(1);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 12.0)
            {
                // Operer med en linie pr 5 minutter
                lineSpacingX_World = 1.0 / 24.0 / 12.0;
                delta = TimeSpan.FromMinutes(5);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 6.0)
            {
                // Operer med en linie pr 10 minutter
                lineSpacingX_World = 1.0 / 24.0 / 6.0;
                delta = TimeSpan.FromMinutes(10);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 4.0)
            {
                // Operer med en linie pr 15 minutter
                lineSpacingX_World = 1.0 / 24.0 / 4.0;
                delta = TimeSpan.FromMinutes(15);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 2.0)
            {
                // Operer med en linie pr 30 minutter
                lineSpacingX_World = 1.0 / 24.0 / 2.0;
                delta = TimeSpan.FromMinutes(30);
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0)
            {
                // Operer med en linie pr time
                lineSpacingX_World = 1.0 / 24.0;
                delta = TimeSpan.FromHours(1);
            }
            else if (lineSpacingX_World_Min < 1.0 / 12.0)
            {
                // Operer med en linie pr 2 timer
                lineSpacingX_World = 1.0 / 12.0;
                delta = TimeSpan.FromHours(2);
            }
            else if (lineSpacingX_World_Min < 1.0 / 8.0)
            {
                // Operer med en linie pr 3 timer
                lineSpacingX_World = 1.0 / 8.0;
                delta = TimeSpan.FromHours(3);
            }
            else if (lineSpacingX_World_Min < 1.0 / 6.0)
            {
                // Operer med en linie pr 4 timer
                lineSpacingX_World = 1.0 / 6.0;
                delta = TimeSpan.FromHours(4);
            }
            else if (lineSpacingX_World_Min < 1.0 / 4.0)
            {
                // Operer med en linie pr 6 timer
                lineSpacingX_World = 1.0 / 4.0;
                delta = TimeSpan.FromHours(6);
            }
            else if (lineSpacingX_World_Min < 1.0 / 2.0)
            {
                // Operer med en linie pr 12 timer
                lineSpacingX_World = 1.0 / 2.0;
                delta = TimeSpan.FromHours(12);
            }
            else if (lineSpacingX_World_Min < 1.0)
            {
                // Operer med en linie pr dag
                lineSpacingX_World = 1.0;
                delta = TimeSpan.FromDays(1);
            }
            else if (lineSpacingX_World_Min < 30.0)
            {
                // Operer med en linie pr måned
                var a = 0;
            }
            else if (lineSpacingX_World_Min < 365.0)
            {
                // Operer med en linie pr år
                var a = 0;
            }
            else if (lineSpacingX_World_Min < 3652.5)
            {
                // Operer med en linie pr årti
                var a = 0;
            }

            var labelWidth = lineSpacingX_World * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi (for en linie)
            var x = Math.Floor(x0 / lineSpacingX_World) * lineSpacingX_World;
            var timeSpan = RoundSeconds(TimeSpan.FromDays(x));
            var t = TimeAtOrigo + timeSpan; 
            //t = t.ToLocalTime(); // Ideen er ikke helt skæv, men det bliver alligevel noget klyt

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

                    var day = t.Date.Day;
                    var hour = t.Hour;
                    var minute = t.Minute;

                    var label = "x";

                    if (minute > 0)
                    {
                        label = $"{hour}:{minute.ToString().PadLeft(2, '0')}";
                    }
                    else if (hour > 0)
                    {
                        label = $"{hour}:00";
                    }
                    else
                    {
                        label = $"{day}";
                    }

                    GeometryEditorViewModel.AddLabel(
                        label,
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
