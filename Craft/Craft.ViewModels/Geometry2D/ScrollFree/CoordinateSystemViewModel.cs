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
        protected Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.35 };
        private bool _showHorizontalAxis;
        private bool _showVerticalAxis;
        private bool _showHorizontalGridLines;
        private bool _showVerticalGridLines;
        private bool _showXAxisLabels;
        private bool _showYAxisLabels;
        private double? _staticXValue;
        private double _staticXValueViewPort;
        private bool _showStaticXValue;
        private double? _dynamicXValue;
        private double _dynamicXValueViewPort;
        private double _dynamicXValueViewPortWhenLocked;
        private bool _showDynamicXValue;
        private bool _lockWorldWindowOnDynamicXValue;
        protected double _worldWindowExpansionFactor;
        protected Point _expandedWorldWindowUpperLeft;
        protected Size _expandedWorldWindowSize;
        
        public double Fraction { get; set; }

        public double? StaticXValue
        {
            get => _staticXValue;
            set
            {
                _staticXValue = value;
                UpdateStaticXValueViewPort();
                RaisePropertyChanged();
            }
        }

        private void UpdateStaticXValueViewPort()
        {
            if (_staticXValue.HasValue)
            {
                StaticXValueViewPort =
                    GeometryEditorViewModel.ConvertWorldXCoordinateToViewPortXCoordinate(_staticXValue.Value);
            }
            else
            {
                ShowStaticXValue = false;
            }
        }

        public double StaticXValueViewPort
        {
            get => _staticXValueViewPort;
            set
            {
                _staticXValueViewPort = value;

                // Figure out if the line representing the static x value should be visible
                var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
                var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;

                var marginInWorldDistance = GeometryEditorViewModel.MarginLeft / GeometryEditorViewModel.Scaling.Width;

                ShowStaticXValue =
                    StaticXValue >= x0 + marginInWorldDistance &&
                    StaticXValue <= x1;

                RaisePropertyChanged();
            }
        }

        public bool ShowStaticXValue
        {
            get => _showStaticXValue;
            set
            {
                _showStaticXValue = value;
                RaisePropertyChanged();
            }
        }

        // This Value is highlighted with a vertical line in the coordinate system.
        // The host of the CoordinateSystemViewModel may assign the "UpdateModelCallback" callback function to
        // a method where it sets the Dynamic XValue property to an arbitrary value.
        // If the CoordinateSystemViewModel is configured for focusing on the dynamic x value, the World Window
        // will also move, when the dynamic x value is updated.
        public double? DynamicXValue
        {
            get => _dynamicXValue;
            set
            {
                _dynamicXValue = value;

                if (_dynamicXValue.HasValue)
                {
                    DynamicXValueViewPort =
                        GeometryEditorViewModel.ConvertWorldXCoordinateToViewPortXCoordinate(_dynamicXValue.Value);
                }
                else
                {
                    ShowDynamicXValue = false;
                }

                UpdateStaticXValueViewPort();

                RaisePropertyChanged();
            }
        }

        public double DynamicXValueViewPortWhenLocked
        {
            get => _dynamicXValueViewPortWhenLocked;
            set
            {
                _dynamicXValueViewPortWhenLocked = value;
                RaisePropertyChanged();
            }
        }

        public double DynamicXValueViewPort
        {
            get => _dynamicXValueViewPort;
            set
            {
                _dynamicXValueViewPort = value;

                if (DynamicXValue.HasValue)
                {
                    if (LockWorldWindowOnDynamicXValue)
                    {
                        ShowDynamicXValue = false;

                        // Position the World Window so that the x value of interest is in the middle
                        GeometryEditorViewModel.WorldWindowUpperLeft = new Point(
                            DynamicXValue.Value - Fraction * GeometryEditorViewModel.WorldWindowSize.Width,
                            GeometryEditorViewModel.WorldWindowUpperLeft.Y);
                    }
                    else
                    {
                        // Figure out if the line representing the x value of interest should be visible
                        var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
                        var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;

                        var marginInWorldDistance = GeometryEditorViewModel.MarginLeft / GeometryEditorViewModel.Scaling.Width;

                        ShowDynamicXValue =
                            DynamicXValue >= x0 + marginInWorldDistance &&
                            DynamicXValue <= x1;
                    }
                }
                else
                {
                    ShowDynamicXValue = false;
                }

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

        public bool ShowXAxisLabels
        {
            get { return _showXAxisLabels; }
            set
            {
                _showXAxisLabels = value;
                UpdateCoordinateSystemForGeometryEditorViewModel();
                RaisePropertyChanged();
            }
        }

        public bool ShowYAxisLabels
        {
            get { return _showYAxisLabels; }
            set
            {
                _showYAxisLabels = value;
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
            double marginY,
            double worldWindowExpansionFactor)
        {
            _showHorizontalAxis = true;
            _showVerticalAxis = true;
            _showHorizontalGridLines = true;
            _showVerticalGridLines = true;
            _worldWindowExpansionFactor = worldWindowExpansionFactor;
            Fraction = 0.5;

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1)
                {
                    MarginLeft = marginX,
                    MarginBottom = marginY,
                    ShowMarginLeft = marginX > 0,
                    ShowMarginBottom = marginY > 0
                };

            GeometryEditorViewModel.InitializeWorldWindow(worldWindowFocus, worldWindowSize, fitAspectRatio);

            GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
            {
                UpdateStaticXValueViewPort();
            };

            GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                UpdateCoordinateSystemForGeometryEditorViewModel();
                UpdateStaticXValueViewPort();

                DynamicXValueViewPortWhenLocked = Fraction * GeometryEditorViewModel.ViewPortSize.Width;
            };

            GeometryEditorViewModel.PropertyChanged += (s, e) =>
            {
                if (!LockWorldWindowOnDynamicXValue ||
                    e.PropertyName != "WorldWindowUpperLeft" ||
                    IsWorldWindowEnclosedByExpandedWorldWindow()) return;

                GeometryEditorViewModel.OnWorldWindowMajorUpdateOccured();
            };
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

            _expandedWorldWindowUpperLeft = new Point(
                x0 - _worldWindowExpansionFactor * (x1 - x0),
                y0 - _worldWindowExpansionFactor * (y1 - y0));

            _expandedWorldWindowSize = new Size(
                (1 + 2 * _worldWindowExpansionFactor) * (x1 - x0),
                (1 + 2 * _worldWindowExpansionFactor) * (y1 - y0));

            // Clear the grid lines
            GeometryEditorViewModel.ClearLines();

            // Clear the labels
            GeometryEditorViewModel.ClearLabels();

            if (ShowHorizontalGridLines || ShowYAxisLabels)
            {
                DrawHorizontalGridLinesAndOrLabels(x0, dx, thickness);
            }

            if (ShowVerticalGridLines || ShowXAxisLabels)
            {
                DrawVerticalGridLinesAndOrLabels(y0, dy, thickness);
            }
        }

        protected void DrawHorizontalGridLinesAndOrLabels(
            double x0,
            double dx,
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

            // Hvor mange decimaler er der generelt på et tick?
            // (Det skal vi bruge for at kompensere for afrundingsfejl, så der ikke f.eks. kommer til at stå 0.60000000000012)
            var labelDecimals = (int)Math.Max(0, Math.Ceiling(-Math.Log10(lineSpacingY_World)));

            // Find ud af første y-værdi
            var y = Math.Floor(_expandedWorldWindowUpperLeft.Y / lineSpacingY_World) * lineSpacingY_World;

            while (y < _expandedWorldWindowUpperLeft.Y + _expandedWorldWindowSize.Height)
            {
                if (ShowHorizontalGridLines)
                {
                    GeometryEditorViewModel.AddLine(
                        new PointD(_expandedWorldWindowUpperLeft.X, y),
                        new PointD(_expandedWorldWindowUpperLeft.X + _expandedWorldWindowSize.Width, y),
                        thickness,
                        _gridBrush);
                }

                if (_showYAxisLabels)
                {
                    GeometryEditorViewModel.AddLabel(
                        Math.Round(y, labelDecimals).ToString(CultureInfo.InvariantCulture),
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0,
                        0);
                }

                y += lineSpacingY_World;
            }
        }

        protected virtual void DrawVerticalGridLinesAndOrLabels(
            double y0,
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
            var x = Math.Floor(_expandedWorldWindowUpperLeft.X / lineSpacingX_World) * lineSpacingX_World;

            while (x < _expandedWorldWindowUpperLeft.X + _expandedWorldWindowSize.Width)
            {
                if (ShowVerticalGridLines)
                {
                    GeometryEditorViewModel.AddLine(
                        new PointD(x, _expandedWorldWindowUpperLeft.Y),
                        new PointD(x, _expandedWorldWindowUpperLeft.Y + _expandedWorldWindowSize.Height),
                        thickness,
                        _gridBrush);
                }

                if (ShowXAxisLabels)
                {
                    GeometryEditorViewModel.AddLabel(
                        Math.Round(x, labelDecimals).ToString(CultureInfo.InvariantCulture),
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0,
                        null,
                        GeometryEditorViewModel.MarginBottomOffset);
                }

                x += lineSpacingX_World;
            }
        }

        private bool IsWorldWindowEnclosedByExpandedWorldWindow()
        {
            return
                GeometryEditorViewModel.WorldWindowUpperLeft.X > _expandedWorldWindowUpperLeft.X &&
                GeometryEditorViewModel.WorldWindowUpperLeft.Y > _expandedWorldWindowUpperLeft.Y &&
                GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width < _expandedWorldWindowUpperLeft.X + _expandedWorldWindowSize.Width &&
                GeometryEditorViewModel.WorldWindowUpperLeft.Y + GeometryEditorViewModel.WorldWindowSize.Height < _expandedWorldWindowUpperLeft.Y + _expandedWorldWindowSize.Height;
        }
    }
}
