using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        public GeometryEditorViewModel GeometryEditorViewModel1 { get; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; }
        public ImageEditorViewModel ImageEditorViewModel { get; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel(1, 250, 75);
            GeometryEditorViewModel2 = new MathematicalGeometryEditorViewModel(1, 250, 75);
            ImageEditorViewModel = new ImageEditorViewModel(1200, 900);

            //LineViewModel.Thickness = 0.5;
            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);
        }

        private void DrawAHouse(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Frame
            geometryEditorViewModel.AddLine(new PointD(0, 0), new PointD(0, 200), 1);
            geometryEditorViewModel.AddLine(new PointD(0, 200), new PointD(200, 300), 1);
            geometryEditorViewModel.AddLine(new PointD(200, 300), new PointD(400, 200), 1);
            geometryEditorViewModel.AddLine(new PointD(400, 200), new PointD(400, 0), 1);
            geometryEditorViewModel.AddLine(new PointD(400, 0), new PointD(0, 0), 1);

            // Door
            geometryEditorViewModel.AddLine(new PointD(50, 0), new PointD(50, 150), 0.5);
            geometryEditorViewModel.AddLine(new PointD(50, 150), new PointD(150, 150), 0.5);
            geometryEditorViewModel.AddLine(new PointD(150, 150), new PointD(150, 0), 0.5);
            geometryEditorViewModel.AddShape(1, new RectangleViewModel
            {
                Point = new PointD(100, 75),
                Width = 95,
                Height = 145
            });

            // Window
            geometryEditorViewModel.AddLine(new PointD(250, 75), new PointD(250, 150), 0.5);
            geometryEditorViewModel.AddLine(new PointD(250, 150), new PointD(350, 150), 0.5);
            geometryEditorViewModel.AddLine(new PointD(350, 150), new PointD(350, 75), 0.5);
            geometryEditorViewModel.AddLine(new PointD(350, 75), new PointD(250, 75), 0.5);
            geometryEditorViewModel.AddShape(2, new RectangleViewModel
            {
                Point = new PointD(300, 112.5),
                Width = 95,
                Height = 70
            });

            // Sun
            geometryEditorViewModel.AddPoint(new PointD(385, 415), 10);
            geometryEditorViewModel.AddPoint(new PointD(415, 415), 10);
            geometryEditorViewModel.AddShape(3, new EllipseViewModel
            {
                Point = new PointD(400, 400),
                Width = 80,
                Height = 80
            });

            geometryEditorViewModel.AddLine(new PointD(300, 400), new PointD(500, 400), 2);
            geometryEditorViewModel.AddLine(new PointD(400, 300), new PointD(400, 500), 2);
            geometryEditorViewModel.AddLine(new PointD(330, 330), new PointD(470, 470), 2);
            geometryEditorViewModel.AddLine(new PointD(330, 470), new PointD(470, 330), 2);
        }
    }
}
