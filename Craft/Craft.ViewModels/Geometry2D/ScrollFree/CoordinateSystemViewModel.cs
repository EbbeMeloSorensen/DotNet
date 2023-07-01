using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateSystemViewModel : ViewModelBase
    {
        protected double _marginX;
        protected double _marginY;
        protected double _Y2;
        protected bool _includeGrid = true;
        protected Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.25 };

        public double MarginX
        {
            get
            {
                return _marginX;
            }
        }

        public double Y2 
        { 
            get
            {
                return _Y2;
            }
            set
            {
                _Y2 = value;
                RaisePropertyChanged();
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            double marginX,
            double marginY)
        {
            _marginX = marginX;
            _marginY = marginY;

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize);

            GeometryEditorViewModel.PropertyChanged += GeometryEditorViewModel_PropertyChanged;

            GeometryEditorViewModel.WorldWindowMajorUpdateOccured += 
                GeometryEditorViewModel_WorldWindowMajorUpdateOccured;

            GeometryEditorViewModel.WorldWindowUpdateOccured += 
                GeometryEditorViewModel_WorldWindowUpdateOccured;
        }

        private void GeometryEditorViewModel_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ViewPortSize":
                {
                    Y2 = GeometryEditorViewModel.ViewPortSize.Height - _marginY;
                    break;
                }
            }
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
        }

        protected virtual void UpdateCoordinateSystemForGeometryEditorViewModel(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            // We want margins and thickness to be independent on scaling
            var dx = _marginX / GeometryEditorViewModel.Scaling.Width;
            var dy = _marginY / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            GeometryEditorViewModel.ClearLines();
            GeometryEditorViewModel.ClearLabels();

            DrawHorizontalGridLines(x0, y0, x1, y1, dx, dy, thickness);
            DrawVerticalGridLines(x0, y0, x1, y1, dx, dy, thickness);
        }

        protected void DrawHorizontalGridLines(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness)
        {
            // 1: Find ud af spacing af horisontale grid lines
            var lineSpacingY_ViewPort_Min = 75.0;
            var lineSpacingY_World_Min = lineSpacingY_ViewPort_Min / GeometryEditorViewModel.Scaling.Height;
            var lineSpacingY_World = Math.Pow(10, Math.Ceiling(Math.Log10(lineSpacingY_World_Min)));

            // Her er lineSpacingX_World f.eks. 0.01, 0.1, 1, 10, 100 eller 1000

            if (lineSpacingY_World / 5 >= lineSpacingY_World_Min)
            {
                // Hvis den landede på f.eks. 10, så er der plads til at have en grid line for hver 2, dvs ved 0, 2, 4, 6, 8, 10 osv
                lineSpacingY_World /= 5;
            }
            else if (lineSpacingY_World / 2 >= lineSpacingY_World_Min)
            {
                // Hvis den landede på f.eks. 10, så er der plads til at have en grid line for hver 5, dvs ved 0, 5, 10, 15 osv
                lineSpacingY_World /= 2;
            }

            var lineSpacingY_ViewPort = lineSpacingY_World * GeometryEditorViewModel.Scaling.Height;

            // Hvor mange decimaler er der generelt på et tick?
            // (Det skal vi bruge for at kompensere for afrundingsfejl, så der ikke f.eks. kommer til at stå 0.60000000000012)
            var labelDecimals = (int)Math.Max(0, Math.Ceiling(-Math.Log10(lineSpacingY_World)));

            // Find ud af første y-værdi
            var y = Math.Floor(y0 / lineSpacingY_World) * lineSpacingY_World;

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

                    //var text = y.ToString(CultureInfo.InvariantCulture);
                    var text = Math.Round(y, labelDecimals).ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0);
                }

                y += lineSpacingY_World;
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
            // 1: Find ud af spacing af vertikale grid lines
            var lineSpacingX_ViewPort_Min = 75.0;
            var lineSpacingX_World_Min = lineSpacingX_ViewPort_Min / GeometryEditorViewModel.Scaling.Width;
            var lineSpacingX_World = Math.Pow(10, Math.Ceiling(Math.Log10(lineSpacingX_World_Min)));

            // Her er lineSpacingX_World f.eks. 0.01, 0.1, 1, 10, 100 eller 1000

            if (lineSpacingX_World / 5 >= lineSpacingX_World_Min)
            {
                // Hvis den landede på f.eks. 10, så er der plads til at have en grid line for hver 2, dvs ved 0, 2, 4, 6, 8, 10 osv
                lineSpacingX_World /= 5;
            }
            else if (lineSpacingX_World / 2 >= lineSpacingX_World_Min)
            {
                // Hvis den landede på f.eks. 10, så er der plads til at have en grid line for hver 5, dvs ved 0, 5, 10, 15 osv
                lineSpacingX_World /= 2;
            }

            var lineSpacingX_ViewPort = lineSpacingX_World * GeometryEditorViewModel.Scaling.Width;

            var labelWidth = lineSpacingX_ViewPort;
            var labelHeight = 20.0;

            // Hvor mange decimaler er der generelt på et tick?
            // (Det skal vi bruge for at kompensere for afrundingsfejl, så der ikke f.eks. kommer til at stå 0.60000000000012)
            var labelDecimals = (int) Math.Max(0, Math.Ceiling(-Math.Log10(lineSpacingX_World)));

            // Find ud af første x-værdi
            var x = Math.Floor(x0 / lineSpacingX_World) * lineSpacingX_World;

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

                    var text = Math.Round(x, labelDecimals).ToString(CultureInfo.InvariantCulture);

                    // Place label at ticks
                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);
                }

                x += lineSpacingX_World;
            }
        }
    }
}
