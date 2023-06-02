using System.Collections.Generic;
using System.Windows.Media;
using Craft.Utils;
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

        public GeometryEditorViewModel GeometryEditorViewModel1 { get; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; }
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel(1, 250, 75);
            GeometryEditorViewModel2 = new MathematicalGeometryEditorViewModel(1, 250, 75);
            ImageEditorViewModel = new ImageEditorViewModel(1200, 900);

            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);
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

            geometryEditorViewModel.AddLine(new PointD(50, 0), new PointD(50, 150), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddLine(new PointD(50, 150), new PointD(150, 150), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
            geometryEditorViewModel.AddLine(new PointD(150, 150), new PointD(150, 0), doorAndWindowFrameThickness, doorAndWindowFrameBrush);
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

        private void ZoomInForGeometryEditor1()
        {
            GeometryEditorViewModel1.Magnification *= 1.1;
        }

        private void ZoomOutForGeometryEditor1()
        {
            GeometryEditorViewModel1.Magnification /= 1.1;
        }
    }
}
