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
        private bool _showHorizontalAxis;
        private bool _showVerticalAxis;
        private bool _showHorizontalGridLines;
        private bool _showVerticalGridLines;
        private double _xValueOfInterest;
        private double _xValueOfInterestViewPort;
        private bool _showXValueOfInterest;
        private bool _lockWorldWindowOnXValueOfInterest;

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

        public double XValueOfInterest
        {
            get => _xValueOfInterest;
            set
            {
                _xValueOfInterest = value;

                XValueOfInterestViewPort =
                    GeometryEditorViewModel.ConvertWorldXCoordinateToViewPortXCoordinate(_xValueOfInterest);

                RaisePropertyChanged();
            }
        }

        public double XValueOfInterestViewPort
        {
            get => _xValueOfInterestViewPort;
            set
            {
                _xValueOfInterestViewPort = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowXValueOfInterest
        {
            get => _showXValueOfInterest;
            set
            {
                _showXValueOfInterest = value;
                RaisePropertyChanged();
            }
        }

        public bool LockWorldWindowOnXValueOfInterest
        {
            get => _lockWorldWindowOnXValueOfInterest;
            set
            {
                _lockWorldWindowOnXValueOfInterest = value;
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
            _marginX = marginX;
            _marginY = marginY;
            _showHorizontalAxis = true;
            _showVerticalAxis = true;
            _showHorizontalGridLines = true;
            _showVerticalGridLines = true;

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1, worldWindowFocus, worldWindowSize, fitAspectRatio);

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
            UpdateCoordinateSystemForGeometryEditorViewModel();
        }

        protected virtual void UpdateCoordinateSystemForGeometryEditorViewModel()
        {
            var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
            var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;
            var y0 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y - GeometryEditorViewModel.WorldWindowSize.Height;
            var y1 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y;

            // We want margins and thickness to be independent on scaling
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
            var lineSpacingY_ViewPort_Min = 50.0;
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

                    var dateAsText = Math.Round(x, labelDecimals).ToString(CultureInfo.InvariantCulture);

                    // Place label at ticks
                    GeometryEditorViewModel.AddLabel(
                        dateAsText,
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
