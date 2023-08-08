using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Craft.Algorithms.GuiTest.Common;
using Craft.Utils;
using Craft.Math;
using Craft.ViewModels.Geometry2D.Scrolling;

namespace Craft.Algorithms.GuiTest.Tab2
{
    // General Geometry Manipulation
    public class Tab2ViewModel : ImageEditorViewModel
    {
        private List<Point2D> _points;
        private ObservableCollection<LineSegment2D> _lines;
        private ObservableCollection<Point2DViewModel> _pointViewModels;
        private bool _pointWasClicked;
        private Point2DViewModel _activeViewModel;
        private int _indexOfActivePoint;
        private PointF _initialPoint; // At start of drag

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

        public Tab2ViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            _points = new List<Point2D>
            {
                new Point2D(300, 100),
                new Point2D(350, 120),
                new Point2D(250, 150),
                new Point2D(320, 150)
            };

            var pointDiameters = new[] {15, 15, 15, 30};

            _pointViewModels = new ObservableCollection<Point2DViewModel>();

            var pointIndex = 0;

            _points.ForEach(p =>
            {
                var pointViewModel = new Point2DViewModel(p.AsPointF(), pointIndex, pointDiameters[pointIndex]);
                pointIndex++;

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _pointViewModels.Add(pointViewModel);
            });

            UpdateLines();
        }

        public void MovePoint(PointD offset)
        {
            var point = new Point2D(
                _initialPoint.X + offset.X,
                _initialPoint.Y + offset.Y);

            _activeViewModel.Point = point.AsPointF();

            _points[_indexOfActivePoint] = new Point2D(point.X, point.Y); 

            UpdateLines();
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
    }
}
