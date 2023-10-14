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
        protected bool _includeGrid = true;
        protected Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.35 };
        private bool _showHorizontalAxis;
        private bool _showVerticalAxis;
        private bool _showHorizontalGridLines;
        private bool _showVerticalGridLines;
        private double _dynamicXValue;
        private double _dynamicXValueViewPort;
        private bool _showDynamicXValue;
        private bool _lockWorldWindowOnDynamicXValue;

        public double DynamicXValue
        {
            get => _dynamicXValue;
            set
            {
                _dynamicXValue = value;

                DynamicXValueViewPort =
                    GeometryEditorViewModel.ConvertWorldXCoordinateToViewPortXCoordinate(_dynamicXValue);

                RaisePropertyChanged();
            }
        }

        public double DynamicXValueViewPort
        {
            get => _dynamicXValueViewPort;
            set
            {
                _dynamicXValueViewPort = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowDynamicXValue
        {
            get => _showDynamicXValue;
            set
            {
                _showDynamicXValue = value;
                RaisePropertyChanged();
            }
        }

        public bool LockWorldWindowOnDynamicXValue
        {
            get => _lockWorldWindowOnDynamicXValue;
            set
            {
                _lockWorldWindowOnDynamicXValue = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowHorizontalAxis
        {
            get { return _showHorizontalAxis; }
            set
            {
                _showHorizontalAxis = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowVerticalAxis
        {
            get { return _showVerticalAxis; }
            set
            {
                _showVerticalAxis = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowHorizontalGridLines
        {
            get { return _showHorizontalGridLines; }
            set
            {
                _showHorizontalGridLines = value;
                UpdateCoordinateSystemForGeometryEditorViewModel();
                RaisePropertyChanged();
            }
        }

        public bool ShowVerticalGridLines
        {
            get { return _showVerticalGridLines; }
            set
            {
                _showVerticalGridLines = value;
                UpdateCoordinateSystemForGeometryEditorViewModel();
                RaisePropertyChanged();
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio,
            double marginX,
            double marginY)
        {
            _showHorizontalAxis = true;
            _showVerticalAxis = true;
            _showHorizontalGridLines = true;
            _showVerticalGridLines = true;

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize, fitAspectRatio)
                {
                    MarginLeft = marginX,
                    MarginBottom = marginY,
                    ShowMarginLeft = marginX > 0,
                    ShowMarginBottom = marginY > 0
                };

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
                    GeometryEditorViewModel.MarginBottomOffset = 
                        GeometryEditorViewModel.ViewPortSize.Height - GeometryEditorViewModel.MarginBottom;
                    break;
                }
            }
        }

        private void GeometryEditorViewModel_WorldWindowUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            if (GeometryEditorViewModel.YAxisLocked)
            {
                GeometryEditorViewModel.ClearLabels("y");
            }
            else
            {
                GeometryEditorViewModel.ClearLabels();
            }
        }

        private void GeometryEditorViewModel_WorldWindowMajorUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            UpdateCoordinateSystemForGeometryEditorViewModel();
        }

        protected virtual void UpdateCoordinateSystemForGeometryEditorViewModel()
        {
            var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
            var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;
            var y0 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y - GeometryEditorViewModel.WorldWindowSize.Height;
            var y1 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y;

            // We want margins and thickness to be independent on scaling
            var dx = GeometryEditorViewModel.MarginLeft / GeometryEditorViewModel.Scaling.Width;
            var dy = GeometryEditorViewModel.MarginBottom / GeometryEditorViewModel.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel.Scaling.Width;

            // Clear the grid lines
            GeometryEditorViewModel.ClearLines();

            // Clear the labels
            GeometryEditorViewModel.ClearLabels();

            if (ShowHorizontalGridLines)
            {
                DrawHorizontalGridLinesAndLabels(x0, y0, x1, y1, dx, dy, thickness, 1.0);
            }

            if (ShowVerticalGridLines)
            {
                DrawVerticalGridLinesAndLabels(x0, y0, x1, y1, dx, dy, thickness, 1.0);
            }
        }

        protected void DrawHorizontalGridLinesAndLabels(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness,
            double expandFactor)
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

            // Hvor mange decimaler er der generelt på et tick?
            // (Det skal vi bruge for at kompensere for afrundingsfejl, så der ikke f.eks. kommer til at stå 0.60000000000012)
            var labelDecimals = (int)Math.Max(0, Math.Ceiling(-Math.Log10(lineSpacingY_World)));

            // Expand the window where grid lines are to be drawn in order to avoid empty areas appearing during panning
            var xMin = x0 - expandFactor * (x1 - x0);
            var xMax = x1 + expandFactor * (x1 - x0);
            var yMin = y0 - expandFactor * (y1 - y0);
            var yMax = y1 + expandFactor * (y1 - y0);

            // Find ud af første y-værdi
            var y = Math.Floor(yMin / lineSpacingY_World) * lineSpacingY_World;

            while (y < yMax)
            {
                if (_includeGrid)
                {
                    GeometryEditorViewModel.AddLine(
                        new PointD(xMin, y),
                        new PointD(xMax, y),
                        thickness,
                        _gridBrush);
                }

                var text = Math.Round(y, labelDecimals).ToString(CultureInfo.InvariantCulture);

                if (y > y0 + dy && y < y1)
                {
                    GeometryEditorViewModel.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0,
                        "y");
                }

                y += lineSpacingY_World;
            }
        }

        protected virtual void DrawVerticalGridLinesAndLabels(
            double x0,
            double y0,
            double x1,
            double y1,
            double dx,
            double dy,
            double thickness,
            double expandFactor)
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

            // Expand the window where grid lines are to be drawn in order to avoid empty areas appearing during panning
            var xMin = x0 - expandFactor * (x1 - x0);
            var xMax = x1 + expandFactor * (x1 - x0);
            var yMin = y0 - expandFactor * (y1 - y0);
            var yMax = y1 + expandFactor * (y1 - y0);

            // Find ud af første x-værdi
            var x = Math.Floor(xMin / lineSpacingX_World) * lineSpacingX_World;

            while (x < xMax)
            {
                if (_includeGrid)
                {
                    GeometryEditorViewModel.AddLine(
                        new PointD(x, yMin),
                        new PointD(x, yMax),
                        thickness,
                        _gridBrush);
                }

                var dateAsText = Math.Round(x, labelDecimals).ToString(CultureInfo.InvariantCulture);

                // Place label at ticks
                GeometryEditorViewModel.AddLabel(
                    dateAsText,
                    new PointD(x, y0 + dy),
                    labelWidth,
                    labelHeight,
                    new PointD(0, labelHeight / 2),
                    0.0,
                    "x");

                x += lineSpacingX_World;
            }
        }
    }
}
