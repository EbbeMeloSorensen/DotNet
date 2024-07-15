﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateSystemViewModel : ViewModelBase
    {
        protected Brush _gridBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.35 };

        private Brush _marginBrush;
        private double _marginLeft;
        private double _marginBottom;
        private double _marginBottomOffset;
        private bool _showMarginLeft;
        private bool _showMarginBottom;
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
        private string _labelForDynamicXValue;
        private bool _showPanningButtons;
        private string _xAxisOverallLabel1;
        private string _xAxisOverallLabel1Alignment;
        private string _xAxisOverallLabel2;
        private string _xAxisOverallLabel2Alignment;
        protected double _worldWindowExpansionFactor;
        protected Point _expandedWorldWindowUpperLeft;
        protected Size _expandedWorldWindowSize;
        private RelayCommand _panLeftCommand;
        private RelayCommand _panRightCommand;

        public Brush MarginBrush
        {
            get => _marginBrush;
            set
            {
                _marginBrush = value;
                RaisePropertyChanged();
            }
        }

        public double MarginLeft
        {
            get { return _marginLeft; }
            set
            {
                _marginLeft = value;
                RaisePropertyChanged();
            }
        }

        public double MarginBottom
        {
            get { return _marginBottom; }
            set
            {
                _marginBottom = value;
                RaisePropertyChanged();
            }
        }

        public double MarginBottomOffset
        {
            get { return _marginBottomOffset; }
            set
            {
                _marginBottomOffset = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowMarginLeft
        {
            get { return _showMarginLeft; }
            set
            {
                _showMarginLeft = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowMarginBottom
        {
            get { return _showMarginBottom; }
            set
            {
                _showMarginBottom = value;
                RaisePropertyChanged();
            }
        }

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

                var marginInWorldDistance = MarginLeft / GeometryEditorViewModel.Scaling.Width;

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

                        var marginInWorldDistance = MarginLeft / GeometryEditorViewModel.Scaling.Width;

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

        public string LabelForDynamicXValue
        {
            get => _labelForDynamicXValue;
            set
            {
                _labelForDynamicXValue = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowPanningButtons
        {
            get => _showPanningButtons;
            set
            {
                _showPanningButtons = value;
                RaisePropertyChanged();
            }
        }

        public string XAxisOverallLabel1
        {
            get => _xAxisOverallLabel1;
            set
            {
                _xAxisOverallLabel1 = value;
                RaisePropertyChanged();
            }
        }

        public string XAxisOverallLabel2
        {
            get => _xAxisOverallLabel2;
            set
            {
                _xAxisOverallLabel2 = value;
                RaisePropertyChanged();
            }
        }

        public string XAxisOverallLabel1Alignment
        {
            get => _xAxisOverallLabel1Alignment;
            set
            {
                _xAxisOverallLabel1Alignment = value;
                RaisePropertyChanged();
            }
        }

        public string XAxisOverallLabel2Alignment
        {
            get => _xAxisOverallLabel2Alignment;
            set
            {
                _xAxisOverallLabel2Alignment = value;
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

        public event EventHandler PanLeftClicked;
        public event EventHandler PanRightClicked;

        public ObservableCollection<LabelViewModel> AxisTickLabelViewModels { get; }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public RelayCommand PanLeftCommand
        {
            get
            {
                return _panLeftCommand ?? (_panLeftCommand = new RelayCommand(OnPanLeftClicked));
            }
        }

        public RelayCommand PanRightCommand
        {
            get
            {
                return _panRightCommand ?? (_panRightCommand = new RelayCommand(OnPanRightClicked));
            }
        }

        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio,
            double marginX,
            double marginY,
            double worldWindowExpansionFactor)
        {
            _marginBrush = new SolidColorBrush(Colors.White);
            _showHorizontalAxis = true;
            _showVerticalAxis = true;
            _showHorizontalGridLines = true;
            _showVerticalGridLines = true;
            _worldWindowExpansionFactor = worldWindowExpansionFactor;
            _xAxisOverallLabel1Alignment = "Center";
            Fraction = 0.5;
            MarginLeft = marginX;
            MarginBottom = marginY;
            ShowMarginLeft = marginX > 0;
            ShowMarginBottom = marginY > 0;

            AxisTickLabelViewModels = new ObservableCollection<LabelViewModel>();

            GeometryEditorViewModel = 
                new GeometryEditorViewModel(-1)
                {
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
                switch (e.PropertyName)
                {
                    case "ViewPortSize":
                        MarginBottomOffset = GeometryEditorViewModel.ViewPortSize.Height - MarginBottom;
                        break;
                }

                if (!LockWorldWindowOnDynamicXValue ||
                    e.PropertyName != "WorldWindowUpperLeft" ||
                    IsWorldWindowEnclosedByExpandedWorldWindow()) return;

                GeometryEditorViewModel.OnWorldWindowMajorUpdateOccured();
            };
        }

        protected virtual void UpdateCoordinateSystemForGeometryEditorViewModel()
        {
            if (GeometryEditorViewModel.WorldWindowSize.Width < 0.000000001 ||
                GeometryEditorViewModel.WorldWindowSize.Height < 0.000000001)
            {
                return;
            }

            var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
            var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;
            var y0 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y - GeometryEditorViewModel.WorldWindowSize.Height;
            var y1 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y;

            // We want margins and thickness to be independent on scaling
            var dx = MarginLeft / GeometryEditorViewModel.Scaling.Width;
            var dy = MarginBottom / GeometryEditorViewModel.Scaling.Height;

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
            AxisTickLabelViewModels.Clear();

            if (ShowHorizontalGridLines || ShowYAxisLabels)
            {
                DrawHorizontalGridLinesAndOrLabels(x0, dx, 1.0);
            }

            if (ShowVerticalGridLines || ShowXAxisLabels)
            {
                DrawVerticalGridLinesAndOrLabels(y0, dy, 1.0);
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
                    var point = new PointD(x0 + dx * 0.8, y);

                    var labelViewModel = new LabelViewModel
                    {
                        Text = Math.Round(y, labelDecimals).ToString(CultureInfo.InvariantCulture),
                        Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                        Width = MarginLeft,
                        Height = 20,
                        Shift = new PointD(-10, 0),
                        Opacity = 0.5,
                        FixedViewPortXCoordinate = 0,
                        FixedViewPortYCoordinate = null
                    };

                    AxisTickLabelViewModels.Add(labelViewModel);
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
                    var point = new PointD(x, y0 + dy);

                    var labelViewModel = new LabelViewModel
                    {
                        Text = Math.Round(x, labelDecimals).ToString(CultureInfo.InvariantCulture),
                        Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                        Width = labelWidth,
                        Height = labelHeight,
                        Shift = new PointD(0, labelHeight / 2),
                        Opacity = 0.5,
                        FixedViewPortXCoordinate = null,
                        FixedViewPortYCoordinate = MarginBottomOffset
                    };

                    AxisTickLabelViewModels.Add(labelViewModel);
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

        private void OnPanLeftClicked()
        {
            var handler = PanLeftClicked;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            GeometryEditorViewModel.OnWorldWindowMajorUpdateOccured();
        }

        private void OnPanRightClicked()
        {
            var handler = PanRightClicked;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            GeometryEditorViewModel.OnWorldWindowMajorUpdateOccured();
        }
    }
}
