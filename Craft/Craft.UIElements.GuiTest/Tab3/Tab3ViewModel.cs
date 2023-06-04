using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Craft.Utils;
using Craft.Utils.Linq;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        private RelayCommand _zoomInForGeometryEditor1Command;
        private RelayCommand _zoomOutForGeometryEditor1Command;

        public RelayCommand ZoomInForGeometryEditor1Command
        {
            get
            {
                return _zoomInForGeometryEditor1Command ?? (_zoomInForGeometryEditor1Command = new RelayCommand(ZoomInForGeometryEditor1));
            }
        }

        public RelayCommand ZoomOutForGeometryEditor1Command
        {
            get
            {
                return _zoomOutForGeometryEditor1Command ?? (_zoomOutForGeometryEditor1Command = new RelayCommand(ZoomOutForGeometryEditor1));
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }
        public MathematicalGeometryEditorViewModel MathematicalGeometryEditorViewModel { get; }
        public ScatterChartViewModel ScatterChartViewModel { get; }
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel = new GeometryEditorViewModel(1, 1, 250, 75);
            MathematicalGeometryEditorViewModel = new MathematicalGeometryEditorViewModel(1, 1, 250, 75);
            ScatterChartViewModel = new ScatterChartViewModel(38, 38, -7, -4);

            ImageEditorViewModel = new ImageEditorViewModel(1200, 900);

            DrawAHouse(GeometryEditorViewModel);
            DrawAHouse(MathematicalGeometryEditorViewModel);

            DrawACurve(ScatterChartViewModel);
        }

        private void DrawAHouse(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Frame
            var frameBrush = new SolidColorBrush(Colors.DarkRed);
            var frameThickness = 3.0;

            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(0, 0),
                    new PointD(0, 200),
                    new PointD(200, 300),
                    new PointD(400, 200),
                    new PointD(400, 0)
                },
                frameThickness,
                frameBrush);

            // Door
            var doorAndWindowFrameBrush = new SolidColorBrush(Colors.GhostWhite);
            var doorAndWindowFrameThickness = 2;

            geometryEditorViewModel.AddPolyline(new List<PointD>
                {
                    new PointD(50, 0),
                    new PointD(50, 150),
                    new PointD(150, 150),
                    new PointD(150, 0)
                },
                doorAndWindowFrameThickness,
                doorAndWindowFrameBrush);

            geometryEditorViewModel.AddShape(1, new RectangleViewModel
            {
                Point = new PointD(100, 75),
                Width = 95,
                Height = 145
            });

            // Window
            geometryEditorViewModel.AddLine(new PointD(250, 75), new PointD(250, 150), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddLine(new PointD(250, 150), new PointD(350, 150), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddLine(new PointD(350, 150), new PointD(350, 75), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddLine(new PointD(350, 75), new PointD(250, 75), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddShape(2, new RectangleViewModel
            {
                Point = new PointD(300, 112.5),
                Width = 95,
                Height = 70
            });

            // Sun
            var sunRayBrush = new SolidColorBrush(Colors.DarkOrange);

            geometryEditorViewModel.AddPoint(new PointD(385, 415), 10);
            geometryEditorViewModel.AddPoint(new PointD(415, 415), 10);
            geometryEditorViewModel.AddShape(3, new EllipseViewModel
            {
                Point = new PointD(400, 400),
                Width = 80,
                Height = 80
            });

            geometryEditorViewModel.AddLine(new PointD(300, 400), new PointD(500, 400), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(400, 300), new PointD(400, 500), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 330), new PointD(470, 470), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 470), new PointD(470, 330), 2, sunRayBrush);
        }

        private void DrawACurve(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Coordinate System
            var coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
            var coordinateSystemThickness = 0.05;

            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-4, 0), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(3.7, -0.2), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(3.7, 0.2), new PointD(4, 0), coordinateSystemThickness, coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -3), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 2.7), new PointD(0, 3), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 3), new PointD(0.2, 2.7), coordinateSystemThickness, coordinateSystemBrush);

            // Curve
            var curveBrush = new SolidColorBrush(Colors.Black);
            var curveThickness = 0.05;

            var points = new List<PointD>();

            for (var x = -5.0; x <= 5.0; x += 0.1)
            {
                // Hjemmeskole for Anton
                //var point = new PointD(x, x);
                //var point = new PointD(x, 0.5 * x);
                //var point = new PointD(x, -x);
                //var point = new PointD(x, 0);
                //var point = new PointD(x, 2);
                //var point = new PointD(x, x * x);
                //var point = new PointD(x, -x * x);
                //var point = new PointD(x, 0.5 * x - 1);
                //var point = new PointD(x, Math.Pow(x - 2, 2) - 3);
                //var point = new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) /4 - 3 * x / 2 - 2);
                points.Add(new PointD(x, Math.Sin(x)));
            }

            // If we draw a bunch of lines (presumably quite heavy if we have many points)
            //points.AdjacenPairs().ToList().ForEach(pair =>
            //{
            //    geometryEditorViewModel.AddLine(pair.Item1, pair.Item2, curveThickness, curveBrush);
            //});

            geometryEditorViewModel.AddPolyline(points, curveThickness, curveBrush);
        }

        private void ZoomInForGeometryEditor1()
        {
            GeometryEditorViewModel.ChangeScaling(1.2);
        }

        private void ZoomOutForGeometryEditor1()
        {
            GeometryEditorViewModel.ChangeScaling(1 / 1.2);
        }
    }
}
