using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;
using System.Diagnostics;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        private bool _includeGrid = true;
        private bool _includeTicks = false;
        private Brush _coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
        private Brush _gridBrush = new SolidColorBrush(Colors.Gray) {Opacity = 0.25};
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);
        private double _curveThickness = 0.05;

        // Initielt World Window (afgrænset højre og venstre)
        private double _x0 = -3.0;
        private double _x1 = 4.0;
        private double _y0 = -1.0;
        private double _y1 = 1.0;

        private bool _windMillInHouseDrawingsRotates;
        private int _worldWindowUpdateCountForGeometryEditorViewModel4;
        private int _worldWindowMajorUpdateCountForGeometryEditorViewModel4;
        private int _worldWindowUpdateCountForCoordinateSystemViewModel;
        private int _worldWindowMajorUpdateCountForCoordinateSystemViewModel;
        private string _cursorPositionAsText;
        private string _timeAtMousePositionAsText1;
        private string _timeAtMousePositionAsText2;
        private Stopwatch _stopwatch;
        private List<DateTime> _timeStampsOfInterest;

        private RelayCommand _zoomInForGeometryEditor1Command;
        private RelayCommand _zoomOutForGeometryEditor1Command;
        private RelayCommand _zoomInForGeometryEditor2Command;
        private RelayCommand _zoomOutForGeometryEditor2Command;

        public bool WindMillInHouseDrawingsRotates
        {
            get { return _windMillInHouseDrawingsRotates; }
            set
            {
                _windMillInHouseDrawingsRotates = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowUpdateCountForGeometryEditorViewModel4
        {
            get { return _worldWindowUpdateCountForGeometryEditorViewModel4; }
            set
            {
                _worldWindowUpdateCountForGeometryEditorViewModel4 = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowMajorUpdateCountForGeometryEditorViewModel4
        {
            get { return _worldWindowMajorUpdateCountForGeometryEditorViewModel4; }
            set
            {
                _worldWindowMajorUpdateCountForGeometryEditorViewModel4 = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowUpdateCountForCoordinateSystemViewModel
        {
            get { return _worldWindowUpdateCountForCoordinateSystemViewModel; }
            set
            {
                _worldWindowUpdateCountForCoordinateSystemViewModel = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowMajorUpdateCountForCoordinateSystemViewModel
        {
            get { return _worldWindowMajorUpdateCountForCoordinateSystemViewModel; }
            set
            {
                _worldWindowMajorUpdateCountForCoordinateSystemViewModel = value;
                RaisePropertyChanged();
            }
        }

        public string CursorPositionAsText
        {
            get { return _cursorPositionAsText; }
            set
            {
                _cursorPositionAsText = value;
                RaisePropertyChanged();
            }
        }

        public string TimeAtMousePositionAsText1
        {
            get { return _timeAtMousePositionAsText1; }
            set
            {
                _timeAtMousePositionAsText1 = value;
                RaisePropertyChanged();
            }
        }

        public string TimeAtMousePositionAsText2
        {
            get { return _timeAtMousePositionAsText2; }
            set
            {
                _timeAtMousePositionAsText2 = value;
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

        public GeometryEditorViewModel GeometryEditorViewModel1 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel3 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel4 { get; private set; }
        public CoordinateSystemViewModel CoordinateSystemViewModel { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel1 { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel2 { get; private set; }
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel();

            GeometryEditorViewModel2 = new GeometryEditorViewModel(-1);
            GeometryEditorViewModel2.InitializeWorldWindow(new Point(300, 112.5));

            var worldWindowFocus = new Point(
                (_x1 + _x0) / 2,
                (_y1 + _y0) / 2);

            var worldWindowSize = new Size(
                _x1 - _x0,
                _y1 - _y0);

            InitializeGeometryEditorViewModel3(worldWindowFocus, worldWindowSize);
            InitializeGeometryEditorViewModel4(worldWindowFocus, worldWindowSize);
            InitializeCoordinateSysteViewModel(worldWindowFocus, worldWindowSize);
            InitializeTimeSeriesViewModel1();
            InitializeTimeSeriesViewModel2();

            ImageEditorViewModel = new ImageEditorViewModel(1200, 900);

            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);

            DrawACoordinateSystem(GeometryEditorViewModel3);

            var now = DateTime.UtcNow;
            _timeStampsOfInterest = new List<DateTime>
            {
                now,
                now - TimeSpan.FromMinutes(30),
                now - TimeSpan.FromMinutes(45),
            };

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
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
                Height = 145,
                Text = "Door"
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
                Height = 70,
                Text = "Window"
            });

            // Sun
            var sunRayBrush = new SolidColorBrush(Colors.DarkOrange);

            geometryEditorViewModel.AddPoint(new PointD(385, 415), 10);
            geometryEditorViewModel.AddPoint(new PointD(415, 415), 10);
            geometryEditorViewModel.AddShape(3, new EllipseViewModel
            {
                Point = new PointD(400, 400),
                Width = 80,
                Height = 80,
                Text = "Sun"
            });

            geometryEditorViewModel.AddLine(new PointD(300, 400), new PointD(500, 400), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(400, 300), new PointD(400, 500), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 330), new PointD(470, 470), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 470), new PointD(470, 330), 2, sunRayBrush);

            // Label
            geometryEditorViewModel.AddLabel("Danshøjvej 33", new PointD(200, 300), 120, 40, new PointD(0, 20), 0.25);

            // Windmill
            geometryEditorViewModel.AddShape(4, new RectangleViewModel
            {
                Point = new PointD(500, 125),
                Width = 10.0,
                Height = 250.0
            });

            geometryEditorViewModel.AddShape(5, new RotatableEllipseViewModel
            {
                Point = new PointD(500, 250),
                Width = 100.0,
                Height = 10.0,
                Orientation = Math.PI / 4,
            });

            // Make the windmill rotate
            geometryEditorViewModel.UpdateModelCallBack = () =>
            {
                if (!WindMillInHouseDrawingsRotates)
                {
                    return;
                }

                var now = DateTime.Now;
                var fraction = now.Millisecond / 1000.0;
                var orientation = 2 * Math.PI * fraction;

                geometryEditorViewModel.ShapeViewModels
                    .Where(_ => _ is RotatableEllipseViewModel)
                    .Select(_ => _ as RotatableEllipseViewModel)
                    .ToList()
                    .ForEach(_ => _.Orientation = orientation);
            };
        }

        private void DrawACoordinateSystem(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Coordinate System
            var coordinateSystemThickness = 0.05;

            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-6, 0), new PointD(6, 0), coordinateSystemThickness, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, -0.2), new PointD(6, 0), coordinateSystemThickness, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, 0.2), new PointD(6, 0), coordinateSystemThickness, _coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -6), new PointD(0, 6), coordinateSystemThickness, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 5.7), new PointD(0, 6), coordinateSystemThickness, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 6), new PointD(0.2, 5.7), coordinateSystemThickness, _coordinateSystemBrush);

            // Axis ticks
            for (var n = 1; n <= 5; n++)
            {
                geometryEditorViewModel.AddLine(new PointD(n, -0.1), new PointD(n, 0.1), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-n, -0.1), new PointD(-n, 0.1), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLabel(n.ToString(), new PointD(n, -0.1), 40, 40, new PointD(0, 20), 0);
                geometryEditorViewModel.AddLabel((-n).ToString(), new PointD(-n, -0.1), 40, 40, new PointD(0, 20), 0);

                geometryEditorViewModel.AddLine(new PointD(-0.1, n), new PointD(0.1, n), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-0.1, -n), new PointD(0.1, -n), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLabel(n.ToString(), new PointD(-0.1, n), 40, 40, new PointD(-20, 0), 0);
                geometryEditorViewModel.AddLabel((-n).ToString(), new PointD(-0.1, -n), 40, 40, new PointD(-20, 0), 0);
            }

            // Draw a window for diagnostics
            if (false)
            {
                geometryEditorViewModel.AddLine(new PointD(_x0, _y0), new PointD(_x1, _y0), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y0), new PointD(_x1, _y1), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y1), new PointD(_x0, _y1), coordinateSystemThickness, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x0, _y1), new PointD(_x0, _y0), coordinateSystemThickness, _coordinateSystemBrush);
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

        private void UpdateCoordinateSystemForGeometryEditorViewModel4(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            // We want thickness to be independent on scaling
            var dx = 20 / GeometryEditorViewModel4.Scaling.Width;
            var dy = 20 / GeometryEditorViewModel4.Scaling.Height;
            var thickness = 1 / GeometryEditorViewModel4.Scaling.Width;

            GeometryEditorViewModel4.ClearLines();
            GeometryEditorViewModel4.ClearLabels();

            // 1: Find ud af spacing af ticks for x-aksen
            var spacingX = 1.0;
            var labelWidth = spacingX * GeometryEditorViewModel4.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi
            var x = Math.Floor(x0 / spacingX) * spacingX;

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            thickness,
                            _gridBrush);
                    }
                    
                    if(_includeTicks)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x, y0 + dy * 0.8),
                            new PointD(x, y0 + dy * 1.2),
                            thickness,
                            _coordinateSystemBrush);
                    }

                    var text = x.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel4.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);
                }

                x += spacingX;
            }

            // 1: Find ud af spacing af ticks for y-aksen
            var spacingY = 1.0;

            // Find ud af første y-værdi
            var y = Math.Floor(y0 / spacingY) * spacingY;

            while (y < y1)
            {
                if (y > y0 + dy)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x0 + dx, y),
                            new PointD(x1, y),
                            thickness,
                            _gridBrush);
                    }

                    if (_includeTicks)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x0 + dx * 0.8, y),
                            new PointD(x0 + dx * 1.2, y),
                            thickness,
                            _coordinateSystemBrush);
                    }

                    var text = y.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel4.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0);
                }

                y += spacingY;
            }
        }

        private void InitializeGeometryEditorViewModel3(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            GeometryEditorViewModel3 = new GeometryEditorViewModel(-1);

            GeometryEditorViewModel3.InitializeWorldWindow(
                worldWindowFocus,
                worldWindowSize,
                false);

            GeometryEditorViewModel3.WorldWindowMajorUpdateOccured += (s, e) =>
            {
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
                    //points.Add(new PointD(x, x * x * x));                                                 // y = x^3
                    //points.Add(new PointD(x, Math.Abs(x)));                                               // y = |x|
                    //points.Add(new PointD(x, -x * x));                                                    // y = -x^2
                    //points.Add(new PointD(x, Math.Pow(x - 2, 2) - 3));                                    // y = (x - 2)^2 - 3 = x^2 - 4x + 1
                    points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                                                                                                            //points.Add(new PointD(x, Math.Sin(x)));                                               // y = sin(x)
                }

                GeometryEditorViewModel3.ClearPolylines();
                GeometryEditorViewModel3.AddPolyline(points, _curveThickness, _curveBrush);
            };

            GeometryEditorViewModel3.MousePositionWorld.PropertyChanged += (s, e) =>
            {
                CursorPositionAsText = GeometryEditorViewModel3.MousePositionWorld.Object.HasValue
                    ? $"({GeometryEditorViewModel3.MousePositionWorld.Object.Value.X:N2}, {-GeometryEditorViewModel3.MousePositionWorld.Object.Value.Y:N2})"
                    : "";
            };
        }

        private void InitializeGeometryEditorViewModel4(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            GeometryEditorViewModel4 = new GeometryEditorViewModel(-1);

            GeometryEditorViewModel4.InitializeWorldWindow(
                worldWindowFocus,
                worldWindowSize,
                false);

            GeometryEditorViewModel4.WorldWindowUpdateOccured += (s, e) => 
            {
                WorldWindowUpdateCountForGeometryEditorViewModel4++;
                GeometryEditorViewModel4.ClearLabels();
            };

            GeometryEditorViewModel4.WorldWindowMajorUpdateOccured += (s, e) => 
            {
                WorldWindowMajorUpdateCountForGeometryEditorViewModel4++;

                // Notice that world window coordinates are always given in "non-inverted" coordinates,
                // so we need to invert the y coordinate
                UpdateCoordinateSystemForGeometryEditorViewModel4(
                    e.WorldWindowUpperLeft.X,
                    e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width,
                    -e.WorldWindowUpperLeft.Y - e.WorldWindowSize.Height,
                    -e.WorldWindowUpperLeft.Y);

                var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
                var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

                var points = new List<PointD>();
                for (var x = x0; x <= x1; x += 0.1)
                {
                    //points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                    points.Add(new PointD(x, Math.Exp(-0.01 * x * x) * Math.Sin(3 * x))); // (gaussian and sinus)
                }

                GeometryEditorViewModel4.ClearPolylines();
                GeometryEditorViewModel4.AddPolyline(points, _curveThickness, _curveBrush);
            };
        }

        private void InitializeCoordinateSysteViewModel(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            CoordinateSystemViewModel = new CoordinateSystemViewModel(
                worldWindowFocus,
                worldWindowSize,
                false,
                25,
                25,
                1)
            {
                LockWorldWindowOnDynamicXValue = false,
                StaticXValue = 4.0,
                ShowXAxisLabels = true,
                ShowYAxisLabels = true
            };

            CoordinateSystemViewModel.GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
            {
                WorldWindowUpdateCountForCoordinateSystemViewModel++;
                CoordinateSystemViewModel.LockWorldWindowOnDynamicXValue = false;
            };

            CoordinateSystemViewModel.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                WorldWindowMajorUpdateCountForCoordinateSystemViewModel++;

                // Få lige grid tegning på plads, og tag så kurven bagefter
                //return;

                // Update the function curve
                // Todo: Use the expanded world window owned by the coordinate system view model instead of just assuming it is expanded by a factor of 1
                var x0 = e.WorldWindowUpperLeft.X - e.WorldWindowSize.Width;
                var x1 = e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width * 2;

                var points = new List<PointD>();

                for (var x = x0; x <= x1; x += 0.1)
                {
                    //points.Add(new PointD(x, Math.Exp(-0.01 * x * x) * Math.Sin(3 * x))); // (gaussian and sinus)
                    points.Add(new PointD(x, Math.Sin(0.1 * x) * Math.Sin(3 * x))); // (high frequency sinus enveloped by low frequency sinus)
                }

                CoordinateSystemViewModel.GeometryEditorViewModel.ClearPolylines();
                CoordinateSystemViewModel.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
            };

            CoordinateSystemViewModel.GeometryEditorViewModel.UpdateModelCallBack = () =>
            {
                /////////////////////////////////////////////////////////////////////////////////////////
                // Her er vi, når der fra User Controllen kommer en anmodning om at der skal gentegnes //
                // dvs det sker ret tit...                                                             //
                // NÅR det sker, har man mulighed for at opdatere DynamicXValue, hvilket så kan        //
                // udvirke, at WorldWindow flyttes                                                     //
                /////////////////////////////////////////////////////////////////////////////////////////

                // Update the x value of interest
                var secondsElapsed = 0.001 * _stopwatch.Elapsed.TotalMilliseconds;
                CoordinateSystemViewModel.DynamicXValue = -2.0 + secondsElapsed;
            };

            CoordinateSystemViewModel.GeometryEditorViewModel.MouseClickOccured += (s, e) =>
            {
                CoordinateSystemViewModel.StaticXValue = e.CursorWorldPosition.X;
            };
        }

        private void InitializeTimeSeriesViewModel1()
        {
            var timeSpan = TimeSpan.FromDays(7);
            var utcNow = DateTime.UtcNow;
            var timeAtOrigo = utcNow.Date;
            var tFocus = utcNow - timeSpan / 2;
            var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);
            var worldWindowHeight = 3;

            TimeSeriesViewModel1 = new TimeSeriesViewModel(
                new Point(xFocus, 0),
                new Size(timeSpan.TotalDays, worldWindowHeight),
                true,
                25,
                60,
                1,
                timeAtOrigo);

            TimeSeriesViewModel1.GeometryEditorViewModel.YAxisLocked = true;

            TimeSeriesViewModel1.TimeAtMousePosition.PropertyChanged += (s, e) =>
            {
                TimeAtMousePositionAsText1 = TimeSeriesViewModel1.TimeAtMousePosition.Object.HasValue
                    ? TimeSeriesViewModel1.TimeAtMousePosition.Object.Value.ToString()
                    : "";
            };

            TimeSeriesViewModel1.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                var x0 = Math.Floor(e.WorldWindowUpperLeft.X);
                var x1 = Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

                var points = new List<PointD>();
                for (var x = x0; x <= x1; x += 0.1)
                {
                    points.Add(new PointD(x, Math.Sin(3 * x))); // (sinus)
                }

                TimeSeriesViewModel1.GeometryEditorViewModel.ClearPolylines();
                TimeSeriesViewModel1.GeometryEditorViewModel.AddPolyline(points, _curveThickness, _curveBrush);
            };
        }

        private void InitializeTimeSeriesViewModel2()
        {
            var timeSpan = TimeSpan.FromDays(1);
            //var timeSpan = TimeSpan.FromHours(1);
            //var timeSpan = TimeSpan.FromMinutes(1);
            var utcNow = DateTime.UtcNow;
            var timeAtOrigo = utcNow.Date;
            var tFocus = utcNow - timeSpan / 2 + TimeSpan.FromMinutes(1);
            var xFocus = (tFocus - timeAtOrigo) / TimeSpan.FromDays(1.0);

            var thirtySecondsFromNowAsScalar = (DateTime.UtcNow + TimeSpan.FromSeconds(30) - timeAtOrigo).TotalDays;

            TimeSeriesViewModel2 = new TimeSeriesViewModel(
                new Point(xFocus, 0),
                new Size(timeSpan.TotalDays, 3),
                true,
                0,
                40,
                1,
                timeAtOrigo)
            {
                //LockWorldWindowOnDynamicXValue = true,
                LockWorldWindowOnDynamicXValue = false,
                StaticXValue = thirtySecondsFromNowAsScalar
            };

            TimeSeriesViewModel2.GeometryEditorViewModel.YAxisLocked = true;
            TimeSeriesViewModel2.ShowVerticalGridLines = true;
            TimeSeriesViewModel2.ShowHorizontalGridLines = false;
            TimeSeriesViewModel2.ShowVerticalAxis = false;

            TimeSeriesViewModel2.TimeAtMousePosition.PropertyChanged += (s, e) =>
            {
                TimeAtMousePositionAsText2 = TimeSeriesViewModel2.TimeAtMousePosition.Object.HasValue
                    ? TimeSeriesViewModel2.TimeAtMousePosition.Object.Value.ToString()
                    : "";
            };

            TimeSeriesViewModel2.GeometryEditorViewModel.WorldWindowUpdateOccured += (s, e) =>
            {
                // Når man dragger, så skal man træde ud af det mode, hvor den følger tiden
                TimeSeriesViewModel2.LockWorldWindowOnDynamicXValue = false;
            };

            TimeSeriesViewModel2.GeometryEditorViewModel.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                var x0 = e.WorldWindowUpperLeft.X;
                var x1 = e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width;
                var y0 = e.WorldWindowUpperLeft.Y;
                var y1 = e.WorldWindowUpperLeft.Y + e.WorldWindowSize.Height;

                TimeSeriesViewModel2.GeometryEditorViewModel.ClearLines();

                // Det er ikke helt uproblematisk det her - linien forsvinder, hvis man zoomer nok ind...
                // DU bør nok generelt lave det sådan at man konverteer til punkter i viewporten og så tegner det med en liniebredde defineret der.
                var lineThickness = 2.0 / TimeSeriesViewModel2.GeometryEditorViewModel.Scaling.Width;

                var lineViewModels = _timeStampsOfInterest
                    .Select(_ => (_ - TimeSeriesViewModel2.TimeAtOrigo).TotalDays)
                    .Where(_ => _ > x0 && _ < x1)
                    .Select(_ => new LineViewModel(new PointD(_, y0), new PointD(_, y1), lineThickness, _curveBrush))
                    .ToList();

                lineViewModels.ForEach(_ => TimeSeriesViewModel2.GeometryEditorViewModel.LineViewModels.Add(_));
            };

            TimeSeriesViewModel2.GeometryEditorViewModel.UpdateModelCallBack = () =>
            {
                /////////////////////////////////////////////////////////////////////////////////////////
                // Her er vi, når der fra User Controllen kommer en anmodning om at der skal gentegnes //
                // dvs det sker ret tit...                                                             //
                // NÅR det sker, har man mulighed for at opdatere DynamicXValue, hvilket så kan        //
                // udvirke, at WorldWindow flyttes                                                     //
                /////////////////////////////////////////////////////////////////////////////////////////

                // Update the x value of interest
                // Sæt den til current time
                var nowAsScalar = (DateTime.UtcNow - TimeSeriesViewModel2.TimeAtOrigo).TotalDays;
                TimeSeriesViewModel2.DynamicXValue = nowAsScalar;
            };
        }
    }
}
