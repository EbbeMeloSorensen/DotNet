using System.Collections.Generic;
using System.Collections.ObjectModel;
using Craft.Algorithms.GuiTest.Common;
using Craft.ImageEditor.ViewModel;
using Craft.Math;
using Craft.Utils;

namespace Craft.Algorithms.GuiTest.Tab4
{
    public class Tab4ViewModel : ImageEditorViewModel
    {
        private List<Point2D> _points;
        private ObservableCollection<LineSegment2D> _lines;
        private ObservableCollection<Point2DViewModel> _pointViewModels;
        private ObservableCollection<Point2DViewModel> _rasterPointViewModels;
        private bool _pointWasClicked;
        private Point2DViewModel _activeViewModel;
        private int _indexOfActivePoint;
        private Point2D _initialPoint; // At start of drag
        private Triangle2D _triangle;
        private double _magnification;

        public ObservableCollection<LineSegment2D> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Point2DViewModel> PointViewModels
        {
            get
            {
                return _pointViewModels;
            }
            set
            {
                _pointViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Point2DViewModel> RasterPointViewModels
        {
            get
            {
                return _rasterPointViewModels;
            }
            set
            {
                _rasterPointViewModels = value;
                RaisePropertyChanged();
            }
        }

        // We use this to communicate to the view if a point was clicked when a mouse down event occured
        public bool PointWasClicked
        {
            get
            {
                var result = _pointWasClicked;
                _pointWasClicked = false;
                return result;
            }
        }

        public Tab4ViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            _points = new List<Point2D>
            {
                new Point2D(100, 100),
                new Point2D(500, 200),
                new Point2D(200, 300)
            };

            _pointViewModels = new ObservableCollection<Point2DViewModel>();
            _rasterPointViewModels = new ObservableCollection<Point2DViewModel>();

            var pointIndex = 0;

            _points.ForEach(p =>
            {
                var pointViewModel = new Point2DViewModel(p, pointIndex, 15);
                pointIndex++;

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _pointViewModels.Add(pointViewModel);
            });

            _magnification = 10;

            UpdateLines();
            UpdateTriangle();
            UpdateRasterPoints();
        }

        public void MovePoint(PointD offset)
        {
            var point = new Point2D(
                _initialPoint.X + offset.X,
                _initialPoint.Y + offset.Y);

            _activeViewModel.Point = point;

            _points[_indexOfActivePoint] = new Point2D(point.X, point.Y);

            UpdateLines();
        }

        public void MovePointComplete()
        {
            UpdateTriangle();
            UpdateRasterPoints();
        }

        private void ElementViewModelElementClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pointWasClicked = true;
            _indexOfActivePoint = e.ElementId;
            _activeViewModel = PointViewModels[_indexOfActivePoint];
            _initialPoint = _activeViewModel.Point;
        }

        private void UpdateLines()
        {
            Lines = new ObservableCollection<LineSegment2D>
            {
                new LineSegment2D(_points[0], _points[1]),
                new LineSegment2D(_points[1], _points[2]),
                new LineSegment2D(_points[2], _points[0])
            };
        }

        private void UpdateTriangle()
        {
            _triangle = new Triangle2D(
                new Point2D(_points[0].X / _magnification, _points[0].Y / _magnification),
                new Point2D(_points[1].X / _magnification, _points[1].Y / _magnification),
                new Point2D(_points[2].X / _magnification, _points[2].Y / _magnification));
        }

        private void UpdateRasterPoints()
        {
            var rows = 100;
            var columns = 100;
            var raster = new int[rows, columns];

            _triangle.Rasterize(raster, 0, 1);

            RasterPointViewModels = new ObservableCollection<Point2DViewModel>();

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    if (raster[r, c] == 0) continue;

                    RasterPointViewModels.Add(
                        new Point2DViewModel(new Point2D(c * _magnification, r * _magnification), 0, 5));
                }
            }
        }
    }
}
