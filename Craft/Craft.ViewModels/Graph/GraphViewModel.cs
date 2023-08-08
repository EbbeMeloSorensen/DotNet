using System.Collections.Generic;
using System.Collections.ObjectModel;
using Craft.Utils;
using Craft.ViewModels.Common;
using Craft.ViewModels.Geometry2D.Scrolling;

namespace Craft.ViewModels.Graph
{
    public class GraphViewModel : ImageEditorViewModel
    {
        private List<PointD> _points;
        private ObservableCollection<LineSegmentD> _lines;
        private ObservableCollection<PointViewModel> _pointViewModels;
        private bool _pointWasClicked;
        private PointViewModel _activeViewModel;
        private int _indexOfActivePoint;
        private PointD _initialPoint; // At start of drag

        public ObservableCollection<LineSegmentD> Lines
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

        public ObservableCollection<PointViewModel> PointViewModels
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

        public GraphViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            _points = new List<PointD>
            {
                new PointD(300, 100),
                new PointD(350, 120),
                new PointD(250, 150),
                new PointD(320, 150)
            };

            var pointDiameters = new[] { 30, 30, 30, 10 };

            _pointViewModels = new ObservableCollection<PointViewModel>();

            var pointIndex = 0;

            _points.ForEach(p =>
            {
                var pointViewModel = new PointViewModel(p, pointIndex, pointDiameters[pointIndex]);
                pointIndex++;

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _pointViewModels.Add(pointViewModel);
            });

            UpdateLines();
        }

        public void MovePoint(
            PointD offset)
        {
            var point = new PointD(
                _initialPoint.X + offset.X,
                _initialPoint.Y + offset.Y);

            _activeViewModel.Point = point;

            _points[_indexOfActivePoint] = new PointD(point.X, point.Y);

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
            Lines = new ObservableCollection<LineSegmentD>
            {
                new LineSegmentD(_points[0], _points[1]),
                new LineSegmentD(_points[1], _points[2]),
                new LineSegmentD(_points[2], _points[0])
            };
        }
    }
}
