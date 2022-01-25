using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Craft.Algorithms.GuiTest2.Common;
using Craft.DataStructures.Graph;
using Craft.Utils;
using Craft.Math;
using Craft.ImageEditor.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace Craft.Algorithms.GuiTest2.Tab3
{
    public class Tab3ViewModel : ImageEditorViewModel
    {
        private Point2D _viewPoint;
        private GraphAdjacencyMatrix<Point2DVertex, EmptyEdge> _graph;
        private ObservableCollection<LineSegment2D> _wallLines;
        private ObservableCollection<LineSegment2D> _triangleLines;
        private ObservableCollection<Triangle2DViewModel> _triangleViewModels;
        private ObservableCollection<Point2DViewModel> _vertexViewModels;
        private bool _pointWasClicked;
        private Point2DViewModel _activeViewModel;
        private int _indexOfActiveVertex;
        private Point2D _initialVertex;
        private RelayCommand _recalculateCommand;

        public ObservableCollection<LineSegment2D> WallLines
        {
            get
            {
                return _wallLines;
            }
            set
            {
                _wallLines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LineSegment2D> TriangleLines
        {
            get
            {
                return _triangleLines;
            }
            set
            {
                _triangleLines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Point2DViewModel> VertexViewModels
        {
            get
            {
                return _vertexViewModels;
            }
            set
            {
                _vertexViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Triangle2DViewModel> TriangleViewModels
        {
            get
            {
                return _triangleViewModels;
            }
            set
            {
                _triangleViewModels = value;
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

        public RelayCommand RecalculateCommand
        {
            get
            {
                return _recalculateCommand ?? (_recalculateCommand = new RelayCommand(UpdateVisibleRegion));
            }
        }

        public Tab3ViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            var vertices = new List<Point2DVertex>
            {
                new Point2DVertex(10, 10),
                new Point2DVertex(310, 10),
                new Point2DVertex(310, 310),
                new Point2DVertex(10, 310),
                new Point2DVertex(140, 30),
                new Point2DVertex(260, 30),
                new Point2DVertex(260, 100),
                new Point2DVertex(140, 100),
                new Point2DVertex(30, 200),
                new Point2DVertex(30, 270),
                new Point2DVertex(100, 270),
                new Point2DVertex(100, 200)
            };

            _viewPoint = new Point2D(155, 155);

            // Den her skal vi bruge til at identificere nabo-punkterne
            // nb - det er lidt skørt, at du er nødt til at angive en cost,
            // når det jo ikke er relevant for applikationen her, og du endda
            // angiver, at der skal bruges EmptyEdge
            _graph = new GraphAdjacencyMatrix<Point2DVertex, EmptyEdge>(vertices, false);
            _graph.AddEdge(0, 1, 1);
            _graph.AddEdge(1, 2, 1);
            _graph.AddEdge(2, 3, 1);
            _graph.AddEdge(3, 0, 1);
            _graph.AddEdge(4, 5, 1);
            _graph.AddEdge(5, 6, 1);
            _graph.AddEdge(6, 7, 1);
            _graph.AddEdge(7, 4, 1);
            _graph.AddEdge(8, 9, 1);
            _graph.AddEdge(9, 10, 1);
            _graph.AddEdge(10, 11, 1);
            _graph.AddEdge(11, 8, 1);

            _vertexViewModels = new ObservableCollection<Point2DViewModel>();

            var vertexIndex = 0;
            foreach (var vertex in _graph.Vertices)
            {
                var pointViewModel = new Point2DViewModel(new Point2D(vertex.X, vertex.Y), vertexIndex, 10);

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _vertexViewModels.Add(pointViewModel);

                vertexIndex++;
            }

            var viewPointViewModel = new Point2DViewModel(
                new Point2D(_viewPoint.X, _viewPoint.Y), vertexIndex, 20);

            viewPointViewModel.ElementClicked += ElementViewModelElementClicked;
            _vertexViewModels.Add(viewPointViewModel);

            UpdateLines();
            UpdateVisibleRegion();
        }

        public void MovePoint(PointD offset)
        {
            var point = new Point2D(
                _initialVertex.X + offset.X,
                _initialVertex.Y + offset.Y);

            _activeViewModel.Point = point;

            if (_indexOfActiveVertex < _graph.VertexCount)
            {
                _graph.UpdateVertex(_indexOfActiveVertex, new Point2DVertex(point.X, point.Y));
            }
            else
            {
                _viewPoint = new Point2D(point.X, point.Y);
            }

            UpdateLines();
            UpdateVisibleRegion();
        }

        private void ElementViewModelElementClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pointWasClicked = true;
            _indexOfActiveVertex = e.ElementId;
            _activeViewModel = VertexViewModels[_indexOfActiveVertex];
            _initialVertex = _activeViewModel.Point;
        }

        private void UpdateLines()
        {
            // Det her er lige rigeligt grimt..

            WallLines = new ObservableCollection<LineSegment2D>(
                _graph.Edges.Select(e => new LineSegment2D(
                    new Point2D(_graph.Vertices[e.VertexId1].X, _graph.Vertices[e.VertexId1].Y),
                    new Point2D(_graph.Vertices[e.VertexId2].X, _graph.Vertices[e.VertexId2].Y))));
        }

        private void UpdateVisibleRegion()
        {
            var triangles = VisibleRegion.IdentifyVisibleRegion(_graph, _viewPoint);

            TriangleLines = new ObservableCollection<LineSegment2D>();
            TriangleViewModels = new ObservableCollection<Triangle2DViewModel>();

            triangles.ForEach(t =>
            {
                TriangleLines.Add(new LineSegment2D(t.Point1, t.Point2));
                TriangleLines.Add(new LineSegment2D(t.Point2, t.Point3));
                TriangleLines.Add(new LineSegment2D(t.Point3, t.Point1));

                TriangleViewModels.Add(new Triangle2DViewModel(new Triangle2D(
                    t.Point1,
                    t.Point2,
                    t.Point3)));
            });
        }
    }
}
