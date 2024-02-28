﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Craft.Utils;
using Craft.DataStructures.Graph;
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

        private IGraph<LabelledVertex, EmptyEdge> _graph;

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

            _graph = GenerateAGraph2();

            _points = new List<PointD>();
            _pointViewModels = new ObservableCollection<PointViewModel>();

            var pointIndex = 0;

            _graph.Vertices.ForEach(_ =>
            {
                var point = new PointD(300, 300);
                _points.Add(point);

                var pointViewModel = new PointViewModel(point, pointIndex++, 30);

                pointViewModel.ElementClicked += ElementViewModelElementClicked;
                pointViewModel.Label = _.Label;

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
            _points[_indexOfActivePoint] = point;

            UpdateLines();
        }

        public void PlacePoint(
            int pointIndex,
            PointD position)
        {
            _points[pointIndex] = position;
            PointViewModels[pointIndex].Point = position;
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
            Lines = new ObservableCollection<LineSegmentD>(
                _graph.Edges.Select(_ => new LineSegmentD(_points[_.VertexId1], _points[_.VertexId2])));
        }

        private IGraph<LabelledVertex, EmptyEdge> GenerateAGraph1()
        {
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Denmark"),        //  0
                new LabelledVertex("Sweden"),         //  1
                new LabelledVertex("Norway"),         //  2
                new LabelledVertex("Germany"),        //  3
                new LabelledVertex("United Kingdom"), //  4
                new LabelledVertex("Ireland"),        //  5
                new LabelledVertex("Netherlands"),    //  6
                new LabelledVertex("Belgium"),        //  7
                new LabelledVertex("France"),         //  8
                new LabelledVertex("Luxembourg"),     //  9
                new LabelledVertex("Finland"),        // 10
                new LabelledVertex("Spain"),          // 11
                new LabelledVertex("Portugal"),       // 12
                new LabelledVertex("Italy"),          // 13
                new LabelledVertex("Switzerland"),    // 14
                new LabelledVertex("Austria"),        // 15
                new LabelledVertex("Czech Republic"), // 16
                new LabelledVertex("Poland"),         // 17
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 10);
            graph.AddEdge(2, 10);
            graph.AddEdge(3, 6);
            graph.AddEdge(3, 7);
            graph.AddEdge(3, 8);
            graph.AddEdge(3, 9);
            graph.AddEdge(3, 14);
            graph.AddEdge(3, 15);
            graph.AddEdge(3, 16);
            graph.AddEdge(3, 17);
            graph.AddEdge(4, 5);
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(7, 9);
            graph.AddEdge(8, 9);
            graph.AddEdge(8, 11);
            graph.AddEdge(8, 13);
            graph.AddEdge(8, 14);
            graph.AddEdge(11, 12);
            graph.AddEdge(13, 14);
            graph.AddEdge(13, 15);
            graph.AddEdge(14, 15);
            graph.AddEdge(15, 16);
            graph.AddEdge(16, 17);

            return graph;
        }

        private IGraph<LabelledVertex, EmptyEdge> GenerateAGraph2()
        {
            var vertices = new List<LabelledVertex>
            {
                // North America
                new LabelledVertex("Alaska"),                //  0
                new LabelledVertex("Northwest Territory"),   //  1
                new LabelledVertex("Greenland"),             //  2
                new LabelledVertex("Alberta"),               //  3
                new LabelledVertex("Ontario"),               //  4
                new LabelledVertex("Quebec"),                //  5
                new LabelledVertex("Western United States"), //  6
                new LabelledVertex("Eastern United States"), //  7
                new LabelledVertex("Central America"),       //  8

                // South America
                new LabelledVertex("Venezuela"),   //  9
                new LabelledVertex("Peru"),        // 10
                new LabelledVertex("Argentina"),   // 11
                new LabelledVertex("Brazil"),      // 12

                // Europe
                new LabelledVertex("Iceland"),         // 13
                new LabelledVertex("Scandinavia"),     // 14
                new LabelledVertex("Great Britain"),   // 15
                new LabelledVertex("Northern Europe"), // 16
                new LabelledVertex("Ukraine"),         // 17
                new LabelledVertex("Western Europe"),  // 18
                new LabelledVertex("Southern Europe"), // 19

                // Africa
                new LabelledVertex("North Africa"), // 20
                new LabelledVertex("Egypt"),        // 21
                new LabelledVertex("East Africa"),  // 22
                new LabelledVertex("Congo"),        // 23
                new LabelledVertex("South Africa"), // 24
                new LabelledVertex("Madagascar"),   // 25

                // Asia
                new LabelledVertex("Siberia"),     // 26
                new LabelledVertex("Ural"),        // 27
                new LabelledVertex("Yakutsk"),     // 28
                new LabelledVertex("Kamchatka"),   // 29
                new LabelledVertex("Irkutsk"),     // 30
                new LabelledVertex("Afghanistan"), // 31
                new LabelledVertex("Mongolia"),    // 32
                new LabelledVertex("Japan"),       // 33
                new LabelledVertex("China"),       // 34
                new LabelledVertex("Middle East"), // 35
                new LabelledVertex("India"),       // 36
                new LabelledVertex("Siam"),        // 37

                // ~Oceania
                new LabelledVertex("Indonesia"),         // 38
                new LabelledVertex("New Guinea"),        // 39
                new LabelledVertex("Western Australia"), // 40
                new LabelledVertex("Eastern Australia"), // 41

            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 3);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 5);
            graph.AddEdge(2, 13);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 6);
            graph.AddEdge(4, 5);
            graph.AddEdge(4, 6);
            graph.AddEdge(4, 7);
            graph.AddEdge(5, 7);
            graph.AddEdge(6, 7);
            graph.AddEdge(6, 8);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);
            graph.AddEdge(9, 10);
            graph.AddEdge(9, 12);
            graph.AddEdge(10, 11);
            graph.AddEdge(10, 12);
            graph.AddEdge(11, 12);
            graph.AddEdge(12, 20);
            graph.AddEdge(13, 14);
            graph.AddEdge(13, 15);
            graph.AddEdge(14, 15);
            graph.AddEdge(14, 16);
            graph.AddEdge(14, 17);
            graph.AddEdge(15, 16);
            graph.AddEdge(15, 18);
            graph.AddEdge(16, 17);
            graph.AddEdge(16, 18);
            graph.AddEdge(16, 19);
            graph.AddEdge(17, 19);
            graph.AddEdge(18, 19);
            graph.AddEdge(18, 20);
            graph.AddEdge(19, 20);
            graph.AddEdge(19, 21);
            graph.AddEdge(19, 35);
            graph.AddEdge(17, 27);
            graph.AddEdge(17, 31);
            graph.AddEdge(17, 35);
            graph.AddEdge(20, 21);
            graph.AddEdge(20, 22);
            graph.AddEdge(20, 23);
            graph.AddEdge(21, 22);
            graph.AddEdge(21, 35);
            graph.AddEdge(22, 23);
            graph.AddEdge(22, 24);
            graph.AddEdge(22, 25);
            graph.AddEdge(22, 35);
            graph.AddEdge(23, 24);
            graph.AddEdge(24, 25);
            graph.AddEdge(26, 27);
            graph.AddEdge(26, 28);
            graph.AddEdge(26, 30);
            graph.AddEdge(26, 32);
            graph.AddEdge(26, 34);
            graph.AddEdge(27, 31);
            graph.AddEdge(27, 34);
            graph.AddEdge(28, 29);
            graph.AddEdge(28, 30);
            graph.AddEdge(29, 30);
            graph.AddEdge(29, 32);
            graph.AddEdge(29, 33);
            graph.AddEdge(30, 32);
            graph.AddEdge(31, 34);
            graph.AddEdge(31, 35);
            graph.AddEdge(31, 36);
            graph.AddEdge(32, 33);
            graph.AddEdge(32, 34);
            graph.AddEdge(34, 36);
            graph.AddEdge(34, 37);
            graph.AddEdge(35, 36);
            graph.AddEdge(36, 37);
            graph.AddEdge(37, 38);
            graph.AddEdge(38, 39);
            graph.AddEdge(38, 40);
            graph.AddEdge(39, 40);
            graph.AddEdge(39, 41);
            graph.AddEdge(40, 41);

            return graph;
        }
    }
}
