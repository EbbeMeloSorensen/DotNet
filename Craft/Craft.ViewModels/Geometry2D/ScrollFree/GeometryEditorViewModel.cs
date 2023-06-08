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
        private string _imagePath;
        protected Size _viewPortSize;
        protected Size _worldWindowSize;
        private Point _mousePositionViewport;
        private Point _mousePositionWorld;
        protected Point _worldWindowUpperLeft;
        private Size _scaling;
        private Matrix _transformationMatrix;
        private Brush _defaultBrush;
        private Dictionary<PointD, Tuple<double, Brush>> _pointToDiameterMap;
        private Dictionary<int, ShapeViewModel> _shapeViewModelMap;

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
                _viewPortSize = value;
                UpdateWorldWindowSize();
                UpdateTransformationMatrix();
                RaisePropertyChanged();
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

                MousePositionWorld = new Point(
                    _worldWindowUpperLeft.X + _mousePositionViewport.X / _scaling.Width,
                    _worldWindowUpperLeft.Y + _mousePositionViewport.Y / _scaling.Height);
            }
        }

        public Point MousePositionWorld
        {
            get { return _mousePositionWorld; }
            set
            {
                _mousePositionWorld = value;
                RaisePropertyChanged();
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

                // Set the position of the World Window. Notice that this also affects the Transformation Matrix
                WorldWindowUpperLeft = new Point(
                    _mousePositionWorld.X - _mousePositionViewport.X * _worldWindowSize.Width / _viewPortSize.Width,
                    _mousePositionWorld.Y - _mousePositionViewport.Y * _worldWindowSize.Height / _viewPortSize.Height);

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

        public ObservableCollection<PolygonViewModel> PolygonViewModels { get; }

        public ObservableCollection<PolylineViewModel> PolylineViewModels { get; }

        public ObservableCollection<PointViewModel> PointViewModels { get; }

        public ObservableCollection<ShapeViewModel> ShapeViewModels { get; }

        public ObservableCollection<LineViewModel> LineViewModels { get; }

        public IEnumerable<int> AllShapeIds
        {
            get { return _shapeViewModelMap.Keys; }
        }

        public UpdateModelCallBack UpdateModelCallBack { get; set; }

        // A callback delegate passed to the constructor will be kept by the view model and
        // invoked each time a frame refresh is needed
        public GeometryEditorViewModel(
            double initialMagnificationX,
            double initialMagnificationY,
            double initialWorldWindowUpperLeftX,
            double initialWorldWindowUpperLeftY)
        {
            WorldWindowUpperLeftLimit = new Point(double.MinValue, double.MinValue);
            WorldWindowBottomRightLimit = new Point(double.MaxValue, double.MaxValue);
            _scaling = new Size(initialMagnificationX, initialMagnificationY);
            _worldWindowUpperLeft = new Point(initialWorldWindowUpperLeftX, initialWorldWindowUpperLeftY);
            _defaultBrush = new SolidColorBrush(Colors.Black);

            PolygonViewModels = new ObservableCollection<PolygonViewModel>();
            PolylineViewModels = new ObservableCollection<PolylineViewModel>();
            PointViewModels = new ObservableCollection<PointViewModel>();
            ShapeViewModels = new ObservableCollection<ShapeViewModel>();
            LineViewModels = new ObservableCollection<LineViewModel>();

            _pointToDiameterMap = new Dictionary<PointD, Tuple<double, Brush>>();
            _shapeViewModelMap = new Dictionary<int, ShapeViewModel>();

            UpdateTransformationMatrix();
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

        public virtual void AddPoint(
            PointD point,
            double diameter,
            Brush brush = null)
        {
            // Here, we use a so-called null-coalescing expression for setting using the default brush unless one is provided
            _pointToDiameterMap[point] = new Tuple<double, Brush>(diameter, brush ?? _defaultBrush);
            UpdatePointViewModels();
        }

        public virtual void AddShape(
            int id,
            ShapeViewModel shapeViewModel)
        {
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

        public virtual void AddLine(
            PointD point1,
            PointD point2,
            double thickness,
            Brush brush)
        {
            LineViewModels.Add(new LineViewModel(point1, point2, thickness, brush));
        }

        public virtual void AddPolygon(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolygonViewModels.Add(new PolygonViewModel(points, thickness, brush));
        }

        public virtual void AddPolyline(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolylineViewModels.Add(new PolylineViewModel(points, thickness, brush));
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
            _pointToDiameterMap.Clear();
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

            if (System.Math.Abs(shiftX) > 0.001 ||
                System.Math.Abs(shiftY) > 0.001)
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

            UpdatePointViewModels();
        }

        protected void UpdateWorldWindowSize()
        {
            WorldWindowSize = new Size(
                _viewPortSize.Width / _scaling.Width,
                _viewPortSize.Height / _scaling.Height);
        }

        private void UpdatePointViewModels()
        {
            PointViewModels.Clear();

            var temp = new List<PointViewModel>();

            foreach (var kvp in _pointToDiameterMap)
            {
                var diameter = kvp.Value.Item1;
                var brush = kvp.Value.Item2;

                temp.Add(new PointViewModel(TransformPoint(kvp.Key), diameter, brush));
            }

            temp = temp.OrderByDescending(pvm => pvm.Diameter).ToList();

            foreach (var pointViewModel in temp)
            {
                PointViewModels.Add(pointViewModel);
            }
        }

        private PointD TransformPoint(PointD point)
        {
            return new PointD(
                _scaling.Width * point.X - _worldWindowUpperLeft.X * _scaling.Width,
                _scaling.Height * point.Y - _worldWindowUpperLeft.Y * _scaling.Height);
        }
    }
}