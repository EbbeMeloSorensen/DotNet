using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public delegate void UpdateModelCallBack();

    public class GeometryEditorViewModel : ViewModelBase
    {
        private int _yAxisFactor;
        private double? _initialScalingX;
        private double? _initialScalingY;
        private Point? _initialWorldWindowFocus;
        private Size? _initialWorldWindowSize;
        private bool _fitAspectRatio;
        private string _imagePath;
        protected Size _viewPortSize;
        protected Size _worldWindowSize;
        private Point _mousePositionViewport;
        protected Point _worldWindowUpperLeft;
        private Size _scaling;
        private Matrix _transformationMatrix;
        private Brush _defaultBrush;
        private Brush _backgroundBrush;
        private Brush _marginBrush;
        private Dictionary<int, ShapeViewModel> _shapeViewModelMap;
        private bool _xAxisLocked;
        private bool _yAxisLocked;
        private bool _aspectRatioLocked;

        private double _marginLeft;
        private double _marginBottom;
        private double _marginBottomOffset;
        private bool _showMarginLeft; 
        private bool _showMarginBottom; 

        public ObservableObject<Point?> MousePositionWorld { get; }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }

        public virtual Size ViewPortSize
        {
            get { return _viewPortSize; }
            set
            {
                if (!(value.Width > 0) ||
                    !(value.Height > 0)) return;

                _viewPortSize = value;
                UpdateWorldWindowSize();
                UpdateTransformationMatrix();
                RaisePropertyChanged();

                OnWorldWindowMajorUpdateOccured();
            }
        }

        public Size WorldWindowSize
        {
            get { return _worldWindowSize; }
            set
            {
                _worldWindowSize = value;
                RaisePropertyChanged();
            }
        }

        public Point MousePositionViewport
        {
            get { return _mousePositionViewport; }
            set
            {
                _mousePositionViewport = value;

                RaisePropertyChanged();

                MousePositionWorld.Object = new Point(
                    ConvertViewPortXCoordinateToWorldXCoordinate(_mousePositionViewport.X),
                    ConvertViewPortYCoordinateToWorldYCoordinate(_mousePositionViewport.Y));
            }
        }

        public Point WorldWindowUpperLeftLimit { get; set; }

        public Point WorldWindowBottomRightLimit { get; set; }

        public virtual Point WorldWindowUpperLeft
        {
            get { return _worldWindowUpperLeft; }
            set
            {
                _worldWindowUpperLeft.X = Math.Max(value.X, WorldWindowUpperLeftLimit.X);
                _worldWindowUpperLeft.Y = Math.Max(value.Y, WorldWindowUpperLeftLimit.Y);
                _worldWindowUpperLeft.X = Math.Min(_worldWindowUpperLeft.X, WorldWindowBottomRightLimit.X - WorldWindowSize.Width);
                _worldWindowUpperLeft.Y = Math.Min(_worldWindowUpperLeft.Y, WorldWindowBottomRightLimit.Y - WorldWindowSize.Height);

                UpdateTransformationMatrix();
                RaisePropertyChanged();
            }
        }

        public Size Scaling
        {
            get
            {
                return _scaling;
            }
            set
            {
                _scaling = value;

                // Set the Size of the World Window, i.e. not its position. The World Window size only depends on magnification and viewport size
                UpdateWorldWindowSize();

                if (!MousePositionWorld.Object.HasValue)
                {
                    FocusInCenterOfViewPort();
                }

                // Set the position of the World Window. Notice that this also affects the Transformation Matrix
                WorldWindowUpperLeft = new Point(
                    MousePositionWorld.Object.Value.X - _mousePositionViewport.X * _worldWindowSize.Width / _viewPortSize.Width,
                    MousePositionWorld.Object.Value.Y - _mousePositionViewport.Y * _worldWindowSize.Height / _viewPortSize.Height);

                RaisePropertyChanged();
            }
        }

        public Matrix TransformationMatrix
        {
            get { return _transformationMatrix; }
            set
            {
                _transformationMatrix = value;
                RaisePropertyChanged();
            }
        }

        public bool XAxisLocked
        {
            get { return _xAxisLocked; }
            set
            {
                _xAxisLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool YAxisLocked
        {
            get { return _yAxisLocked; }
            set
            {
                _yAxisLocked = value;
                RaisePropertyChanged();
            }
        }

        public bool AspectRatioLocked
        {
            get { return _aspectRatioLocked; }
            set
            {
                _aspectRatioLocked = value;
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

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;
                RaisePropertyChanged();
            }
        }

        public Brush MarginBrush
        {
            get => _marginBrush;
            set
            {
                _marginBrush = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PolygonViewModel> PolygonViewModels { get; }

        public ObservableCollection<PolylineViewModel> PolylineViewModels { get; }

        public ObservableCollection<PointViewModel> PointViewModels { get; }

        public ObservableCollection<ShapeViewModel> ShapeViewModels { get; }

        public ObservableCollection<LineViewModel> LineViewModels { get; }

        public ObservableCollection<LabelViewModel> LabelViewModels { get; private set; }

        public IEnumerable<int> AllShapeIds
        {
            get { return _shapeViewModelMap.Keys; }
        }

        public event EventHandler<WorldWindowUpdatedEventArgs> WorldWindowUpdateOccured;
        public event EventHandler<WorldWindowUpdatedEventArgs> WorldWindowMajorUpdateOccured;
        public event EventHandler<MouseEventArgs> MouseClickOccured;

        // A callback delegate passed to the view model will be kept and
        // invoked each time a frame refresh is needed
        public UpdateModelCallBack UpdateModelCallBack { get; set; }

        public GeometryEditorViewModel(
            int yAxisFactor) : this(yAxisFactor, 1, 1)
        {
        }

        public GeometryEditorViewModel(
            int yAxisFactor,
            double scalingX,
            double scalingY) : this()
        {
            _yAxisFactor = yAxisFactor;
            _initialScalingX = scalingX;
            _initialScalingY = scalingY;
        }

        public GeometryEditorViewModel(
            int yAxisFactor,
            double scalingX,
            double scalingY,
            Point worldWindowFocus) : this()
        {
            _yAxisFactor = yAxisFactor;
            _initialScalingX = scalingX;
            _initialScalingY = scalingY;
            _initialWorldWindowFocus = worldWindowFocus;
        }

        public GeometryEditorViewModel(
            int yAxisFactor,
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio) : this()
        {
            _yAxisFactor = yAxisFactor;
            _initialWorldWindowFocus = worldWindowFocus;
            _initialWorldWindowSize = worldWindowSize;
            _fitAspectRatio = fitAspectRatio;
        }

        protected GeometryEditorViewModel()
        {
            _backgroundBrush = new SolidColorBrush(Colors.WhiteSmoke);
            _marginBrush = new SolidColorBrush(Colors.White);
            _defaultBrush = new SolidColorBrush(Colors.Black);
            _shapeViewModelMap = new Dictionary<int, ShapeViewModel>();

            WorldWindowUpperLeftLimit = new Point(double.MinValue, double.MinValue);
            WorldWindowBottomRightLimit = new Point(double.MaxValue, double.MaxValue);

            MousePositionWorld = new ObservableObject<Point?> { Object = null };

            PolygonViewModels = new ObservableCollection<PolygonViewModel>();
            PolylineViewModels = new ObservableCollection<PolylineViewModel>();
            PointViewModels = new ObservableCollection<PointViewModel>();
            ShapeViewModels = new ObservableCollection<ShapeViewModel>();
            LineViewModels = new ObservableCollection<LineViewModel>();
            LabelViewModels = new ObservableCollection<LabelViewModel>();

            PropertyChanged += GeometryEditorViewModel_PropertyChanged;
        }

        private void GeometryEditorViewModel_PropertyChanged(
            object? sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ViewPortSize":
                    if (WorldWindowSize.Width < 0.000000001 &&
                        WorldWindowSize.Height < 0.000000001 &&
                        ViewPortSize.Width != 0 &&
                        ViewPortSize.Height != 0)
                    {
                        // Her er viewporten initialiseret for første gang, og så kan vi sætte World Window og skalering

                        if (_yAxisFactor == 1)
                        {
                            // Dette er passende for et ALMINDELIGT view

                            if (_initialScalingX.HasValue && 
                                _initialScalingY.HasValue)
                            {
                                Scaling = new Size(_initialScalingX.Value, _initialScalingY.Value);

                                if (_initialWorldWindowFocus.HasValue)
                                {
                                    throw new NotImplementedException();
                                }
                                else
                                {
                                    WorldWindowUpperLeft = new Point(0, 0);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            // Dette er passende for et MATEMATISK view

                            if (_initialScalingX.HasValue &&
                                _initialScalingY.HasValue)
                            {
                                Scaling = new Size(_initialScalingX.Value, _initialScalingY.Value);

                                if (_initialWorldWindowFocus.HasValue)
                                {
                                    WorldWindowUpperLeft = new Point(
                                        _initialWorldWindowFocus.Value.X - WorldWindowSize.Width / 2, 
                                        -ViewPortSize.Height / Scaling.Height - _initialWorldWindowFocus.Value.Y + WorldWindowSize.Height / 2);
                                }
                                else
                                {
                                    WorldWindowUpperLeft = new Point(0, -ViewPortSize.Height / Scaling.Height);
                                }
                            }
                            else if (_initialWorldWindowFocus.HasValue &&
                                     _initialWorldWindowSize.HasValue)
                            {
                                if (_fitAspectRatio)
                                {
                                    // Her skal World Window fylde hele viewporten ud

                                    Scaling = new Size(
                                        ViewPortSize.Width / (_initialWorldWindowSize.Value.Width),
                                        ViewPortSize.Height / _initialWorldWindowSize.Value.Height);

                                    WorldWindowUpperLeft = new Point(
                                        _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                                        _initialWorldWindowSize.Value.Height / 2 - _initialWorldWindowFocus.Value.Y - ViewPortSize.Height / Scaling.Height);
                                }
                                else
                                {
                                    if (ViewPortSize.Width / ViewPortSize.Height <=
                                        _initialWorldWindowSize.Value.Width / _initialWorldWindowSize.Value.Height)
                                    {
                                        // Dette er hvis World Window skal afgrænses af Viewportens VENSTRE og HØJRE side

                                        var scaling = ViewPortSize.Width / _initialWorldWindowSize.Value.Width;
                                        Scaling = new Size(scaling, scaling);

                                        WorldWindowUpperLeft = new Point(
                                            _initialWorldWindowFocus.Value.X - _initialWorldWindowSize.Value.Width / 2,
                                            -_initialWorldWindowFocus.Value.Y - WorldWindowSize.Height / 2);
                                    }
                                    else
                                    {
                                        // Dette er hvis World Window skal afgrænses af Viewportens NEDERSTE OG ØVERSTE side

                                        var scaling = ViewPortSize.Height / _initialWorldWindowSize.Value.Height;
                                        Scaling = new Size(scaling, scaling);

                                        WorldWindowUpperLeft = new Point(
                                            _initialWorldWindowFocus.Value.X - WorldWindowSize.Width / 2,
                                            _initialWorldWindowSize.Value.Height / 2 - _initialWorldWindowFocus.Value.Y - ViewPortSize.Height / scaling);
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Invalid specification of World Window");
                            }
                        }

                        OnWorldWindowMajorUpdateOccured();
                    }
                        
                    MarginBottomOffset = ViewPortSize.Height - MarginBottom;

                    break;
            }
        }

        // For zooming in and out
        public void ChangeScaling(
            double factor)
        {
            FocusInCenterOfViewPort();
            Scaling = new Size(
                Scaling.Width * factor,
                Scaling.Height * factor);
        }

        public void AddPoint(
            PointD point,
            double diameter,
            Brush brush = null)
        {
            PointViewModels.Add(new PointViewModel(new PointD(point.X, _yAxisFactor * point.Y), diameter, brush ?? _defaultBrush));
        }

        public void AddShape(
            int id,
            ShapeViewModel shapeViewModel)
        {
            shapeViewModel.Point = new PointD(
                shapeViewModel.Point.X, 
                shapeViewModel.Point.Y * _yAxisFactor);

            _shapeViewModelMap[id] = shapeViewModel;
            ShapeViewModels.Add(shapeViewModel);
        }

        public void RemoveShape(
            int id)
        {
            var shapeViewModel = _shapeViewModelMap[id];
            ShapeViewModels.Remove(shapeViewModel);
            _shapeViewModelMap.Remove(id);
        }

        public void AddLine(
            PointD point1,
            PointD point2,
            double thickness,
            Brush brush)
        {
            LineViewModels.Add(new LineViewModel(
                new PointD(point1.X, _yAxisFactor * point1.Y), 
                new PointD(point2.X, _yAxisFactor * point2.Y), 
                thickness, brush));
        }

        public void AddPolygon(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolygonViewModels.Add(new PolygonViewModel(
                points.Select(p => new PointD(p.X, _yAxisFactor * p.Y)), thickness, brush));
        }

        public void AddPolyline(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolylineViewModels.Add(new PolylineViewModel(
                points.Select(p => new PointD(p.X, _yAxisFactor * p.Y)), thickness, brush));
        }

        // Argumenter
        // point: angiver positionen af labelens centrum
        // width og height: angiver, hvor stor den skal være
        // shift: en forskydning af centeret, så det ikke nødvendigvis er labellens centrum men f.eks. dens øverste venstre hjørne, der positioneres
        public void AddLabel(
            string text,
            PointD point,
            double width,
            double height,
            PointD shift,
            double opacity,
            string tag = null,
            double? fixedViewPortXCoordinate = null,
            double? fixedViewPortYCoordinate = null)
        {
            LabelViewModels.Add(new LabelViewModel
            {
                Text = text,
                Point = new PointD(point.X, _yAxisFactor * point.Y), 
                Width = width, 
                Height = height,
                Shift = shift,
                Opacity = opacity,
                FixedViewPortXCoordinate = fixedViewPortXCoordinate,
                FixedViewPortYCoordinate = fixedViewPortYCoordinate
            });
        }

        public void ClearPolygons()
        {
            PolygonViewModels.Clear();
        }

        public void ClearPolylines()
        {
            PolylineViewModels.Clear();
        }

        public void ClearPoints()
        {
            PointViewModels.Clear();
        }

        public void ClearShapes()
        {
            ShapeViewModels.Clear();
            _shapeViewModelMap.Clear();
        }

        public void ClearLines()
        {
            LineViewModels.Clear();
        }

        public void ClearLabels()
        {
            LabelViewModels.Clear();
        }

        public void FocusInCenterOfViewPort()
        {
            MousePositionViewport = new Point(
                ViewPortSize.Width / 2,
                ViewPortSize.Height / 2);
        }

        public void SetFocusForWorldWindow(
            PointD focus)
        {
            WorldWindowUpperLeft = new Point(
                focus.X - WorldWindowSize.Width / 2,
                focus.Y - WorldWindowSize.Height / 2);
        }

        public void AdjustWorldWindowSoPointLiesInCentralSquare(
            PointD point,
            double maxOffsetXFraction,
            double maxOffsetYFraction,
            double correctionXFraction = 0.5,
            double correctionYFraction = 0.5)
        {
            var correctionX = WorldWindowSize.Width * (correctionXFraction - 0.5);
            var correctionY = WorldWindowSize.Height * (correctionYFraction - 0.5);

            // What is the distance between the current focus and the point
            var currentFocus = new PointD(
                _worldWindowUpperLeft.X + _worldWindowSize.Width / 2 + correctionX,
                _worldWindowUpperLeft.Y + _worldWindowSize.Height / 2 + correctionY);

            var dx = point.X - currentFocus.X;
            var dy = point.Y - currentFocus.Y;

            var shiftX = 0.0;
            var shiftY = 0.0;
            var maxDx = WorldWindowSize.Width * maxOffsetXFraction;
            var maxDy = WorldWindowSize.Height * maxOffsetYFraction;

            if (dx > maxDx)
            {
                shiftX = dx - maxDx;
            }
            else if (dx < -maxDx)
            {
                shiftX = maxDx + dx;
            }

            if (dy > maxDy)
            {
                shiftY = dy - maxDy;
            }
            else if (dy < -maxDy)
            {
                shiftY = maxDy + dy;
            }

            if (Math.Abs(shiftX) > 0.001 ||
                Math.Abs(shiftY) > 0.001)
            {
                WorldWindowUpperLeft = new Point(
                    WorldWindowUpperLeft.X + shiftX,
                    WorldWindowUpperLeft.Y + shiftY);
            }
        }

        public bool ContainsShape(
            int id)
        {
            return _shapeViewModelMap.ContainsKey(id);
        }

        public ShapeViewModel TryGetShape(
            int id)
        {
            _shapeViewModelMap.TryGetValue(id, out var result);
            return result;
        }

        // This method is called from the View class when it handles the CompositionTarget.Rendering event
        public void UpdateModel()
        {
            UpdateModelCallBack?.Invoke();
        }

        protected void UpdateTransformationMatrix()
        {
            TransformationMatrix = new Matrix(
                _scaling.Width,
                0,
                0,
                _scaling.Height,
                -_worldWindowUpperLeft.X * _scaling.Width,
                -_worldWindowUpperLeft.Y * _scaling.Height);
        }

        protected void UpdateWorldWindowSize()
        {
            if (_scaling.Width <= 0 ||
                _scaling.Height <= 0) return;

            WorldWindowSize = new Size(
                _viewPortSize.Width / _scaling.Width,
                _viewPortSize.Height / _scaling.Height);
        }

        // This method is called from the View class
        public void OnWorldWindowUpdateOccured()
        {
            var handler = WorldWindowUpdateOccured;

            if (handler != null)
            {
                handler(this, new WorldWindowUpdatedEventArgs(
                    WorldWindowUpperLeft,
                    WorldWindowSize));
            }
        }

        // This method is called from the View class
        public void OnWorldWindowMajorUpdateOccured()
        {
            var handler = WorldWindowMajorUpdateOccured;

            if (handler != null)
            {
                handler(this, new WorldWindowUpdatedEventArgs(
                    WorldWindowUpperLeft,
                    WorldWindowSize));
            }
        }

        // This method is called from the View class
        public void OnMouseClickOccured(
            Point mousePositionViewport)
        {
            var handler = MouseClickOccured;

            if (handler != null)
            {
                var mousePositionWorld = new Point(
                    ConvertViewPortXCoordinateToWorldXCoordinate(mousePositionViewport.X),
                    ConvertViewPortYCoordinateToWorldYCoordinate(mousePositionViewport.Y));

                handler(this, new MouseEventArgs(mousePositionWorld));
            }
        }

        public double ConvertViewPortXCoordinateToWorldXCoordinate(
            double viewPortXCoordinate)
        {
            return _worldWindowUpperLeft.X + viewPortXCoordinate / _scaling.Width;
        }

        public double ConvertViewPortYCoordinateToWorldYCoordinate(
            double viewPortYCoordinate)
        {
            return _worldWindowUpperLeft.Y + viewPortYCoordinate / _scaling.Height;
        }

        public double ConvertWorldXCoordinateToViewPortXCoordinate(
            double worldXCoordinate)
        {
            return (worldXCoordinate - _worldWindowUpperLeft.X) * _scaling.Width;
        }

        public double ConvertWorldYCoordinateToViewPortYCoordinate(
            double worldYCoordinate)
        {

            return (worldYCoordinate - _worldWindowUpperLeft.Y) * _scaling.Height;
        }
    }
}