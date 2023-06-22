using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;

        // Afgrænset øverst og nederst
        //private double _x0 = -1.0;
        //private double _x1 = 2.0;
        //private double _y0 = -3.0;
        //private double _y1 = 4.0;

        // Afgrænset højre og venstre
        private double _x0 = -3.0;
        private double _x1 = 4.0;
        private double _y0 = -2.0;
        private double _y1 = 1.0;

        private int _worldWindowUpdateCount;
        private int _worldWindowMajorUpdateCount;

        private RelayCommand _zoomInForGeometryEditor1Command;
        private RelayCommand _zoomOutForGeometryEditor1Command;
        private RelayCommand _zoomInForGeometryEditor2Command;
        private RelayCommand _zoomOutForGeometryEditor2Command;

        public int WorldWindowUpdateCount
        {
            get { return _worldWindowUpdateCount; }
            set
            {
                _worldWindowUpdateCount = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowMajorUpdateCount
        {
            get { return _worldWindowMajorUpdateCount; }
            set
            {
                _worldWindowMajorUpdateCount = value;
                RaisePropertyChanged();
            }
        }

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

        public RelayCommand ZoomInForGeometryEditor2Command
        {
            get
            {
                return _zoomInForGeometryEditor2Command ?? (_zoomInForGeometryEditor2Command = new RelayCommand(ZoomInForGeometryEditor2));
            }
        }

        public RelayCommand ZoomOutForGeometryEditor2Command
        {
            get
            {
                return _zoomOutForGeometryEditor2Command ?? (_zoomOutForGeometryEditor2Command = new RelayCommand(ZoomOutForGeometryEditor2));
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel1 { get; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; }
        public GeometryEditorViewModel GeometryEditorViewModel3 { get; }
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel(1, 1, 1);

            // Denne bruger vi, hvis vi er ok med at koordinatsystemets origo er sammenfaldende med viewets nederste venstre hjørne
            //GeometryEditorViewModel2 = new GeometryEditorViewModel(
            //    -1, 
            //    1, 
            //    1);

            // Denne bruger vi, hvis vi gerne vil specificere, hvilket World punkt man skal fokusere på
            GeometryEditorViewModel2 = new GeometryEditorViewModel(
                -1,
                1,
                1,
                new Point(300, 112.5));

            var worldWindowFocus = new Point(
                (_x1 + _x0) / 2,
                (_y1 + _y0) / 2);

            var worldWindowSize = new Size(
                _x1 - _x0,
                _y1 - _y0);

            GeometryEditorViewModel3 = new GeometryEditorViewModel(
                -1,
                worldWindowFocus,
                worldWindowSize);

            ImageEditorViewModel = new ImageEditorViewModel(1200, 900);

            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);

            DrawACoordinateSystem(GeometryEditorViewModel3);

            GeometryEditorViewModel3.WorldWindowUpdateOccured += GeometryEditorViewModel3_WorldWindowUpdateOccured;
            GeometryEditorViewModel3.WorldWindowMajorUpdateOccured += GeometryEditorViewModel3_WorldWindowMajorUpdateOccured;
        }

        private void GeometryEditorViewModel3_WorldWindowUpdateOccured(
            object? sender, 
            WorldWindowUpdatedEventArgs e)
        {
            WorldWindowUpdateCount++;
        }

        private void GeometryEditorViewModel3_WorldWindowMajorUpdateOccured(
            object? sender,
            WorldWindowUpdatedEventArgs e)
        {
            WorldWindowMajorUpdateCount++;

            var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
            var x1 = Math.Ceiling(x0 + e.WorldWindowSize.Width);

            var points = new List<PointD>();
            for (var x = x0; x <= x1; x += 0.1)
            {
                //points.Add(new PointD(x, x));                                                         // y = x
                //points.Add(new PointD(x, 0.5 * x));                                                   // y = 0.5x
                //points.Add(new PointD(x, 0.5 * x - 1));                                               // y = 0.5x - 1
                //points.Add(new PointD(x, -x));                                                        // y = -x
                //points.Add(new PointD(x, 0));                                                         // y = 0
                //points.Add(new PointD(x, 2));                                                         // y = 2
                //points.Add(new PointD(x, x * x));                                                     // y = x^2
                //points.Add(new PointD(x, -x * x));                                                    // y = -x^2
                //points.Add(new PointD(x, Math.Pow(x - 2, 2) - 3));                                    // y = (x - 2)^2 - 3 = x^2 - 4x + 1
                points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                //points.Add(new PointD(x, Math.Sin(x)));                                               // y = sin(x)
            }

            GeometryEditorViewModel3.ClearPolylines();
            GeometryEditorViewModel3.AddPolyline(points, _curveThickness, _curveBrush);
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

            // House number
            geometryEditorViewModel.AddLabel(new PointD(0, 0), 100);
        }

        private void DrawACoordinateSystem(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Coordinate System
            var coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
            var coordinateSystemThickness = 0.05;

            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-6, 0), new PointD(6, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, -0.2), new PointD(6, 0), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, 0.2), new PointD(6, 0), coordinateSystemThickness, coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -6), new PointD(0, 6), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 5.7), new PointD(0, 6), coordinateSystemThickness, coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 6), new PointD(0.2, 5.7), coordinateSystemThickness, coordinateSystemBrush);

            // Axis ticks
            for (var n = 1; n <= 5; n++)
            {
                geometryEditorViewModel.AddLine(new PointD(n, -0.1), new PointD(n, 0.1), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-n, -0.1), new PointD(-n, 0.1), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-0.1, n), new PointD(0.1, n), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-0.1, -n), new PointD(0.1, -n), coordinateSystemThickness, coordinateSystemBrush);
            }

            // Draw a window for diagnostics
            if (false)
            {
                geometryEditorViewModel.AddLine(new PointD(_x0, _y0), new PointD(_x1, _y0), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y0), new PointD(_x1, _y1), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y1), new PointD(_x0, _y1), coordinateSystemThickness, coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x0, _y1), new PointD(_x0, _y0), coordinateSystemThickness, coordinateSystemBrush);
            }
        }
        
        private void ZoomInForGeometryEditor1()
        {
            GeometryEditorViewModel1.ChangeScaling(1.2);
        }

        private void ZoomOutForGeometryEditor1()
        {
            GeometryEditorViewModel1.ChangeScaling(1 / 1.2);
        }

        private void ZoomInForGeometryEditor2()
        {
            GeometryEditorViewModel2.ChangeScaling(1.2);
        }

        private void ZoomOutForGeometryEditor2()
        {
            GeometryEditorViewModel2.ChangeScaling(1 / 1.2);
        }
    }
}
