using System.Windows;
using GalaSoft.MvvmLight;
using Craft.ViewModels.Geometry2D;
using Craft.Math;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        public GeometryEditorViewModel GeometryEditorViewModel1 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; private set; }

        public Tab3ViewModel()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel(1, 250, 75);
            GeometryEditorViewModel2 = new MathematicalGeometryEditorViewModel(1, 250, 75);

            //LineViewModel.Thickness = 0.5;
            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);
        }

        private void DrawAHouse(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // Frame
            geometryEditorViewModel.AddLine(new Point2D(0, 0), new Point2D(0, 200), 1);
            geometryEditorViewModel.AddLine(new Point2D(0, 200), new Point2D(200, 300), 1);
            geometryEditorViewModel.AddLine(new Point2D(200, 300), new Point2D(400, 200), 1);
            geometryEditorViewModel.AddLine(new Point2D(400, 200), new Point2D(400, 0), 1);
            geometryEditorViewModel.AddLine(new Point2D(400, 0), new Point2D(0, 0), 1);

            // Door
            geometryEditorViewModel.AddLine(new Point2D(50, 0), new Point2D(50, 150), 0.5);
            geometryEditorViewModel.AddLine(new Point2D(50, 150), new Point2D(150, 150), 0.5);
            geometryEditorViewModel.AddLine(new Point2D(150, 150), new Point2D(150, 0), 0.5);
            geometryEditorViewModel.AddShape(1, new RectangleViewModel
            {
                Point = new Point2D(100, 75),
                Width = 95,
                Height = 145
            });

            // Window
            geometryEditorViewModel.AddLine(new Point2D(250, 75), new Point2D(250, 150), 0.5);
            geometryEditorViewModel.AddLine(new Point2D(250, 150), new Point2D(350, 150), 0.5);
            geometryEditorViewModel.AddLine(new Point2D(350, 150), new Point2D(350, 75), 0.5);
            geometryEditorViewModel.AddLine(new Point2D(350, 75), new Point2D(250, 75), 0.5);
            geometryEditorViewModel.AddShape(2, new RectangleViewModel
            {
                Point = new Point2D(300, 112.5),
                Width = 95,
                Height = 70
            });

            // Sun
            geometryEditorViewModel.AddPoint(new Point2D(385, 415), 10);
            geometryEditorViewModel.AddPoint(new Point2D(415, 415), 10);
            geometryEditorViewModel.AddShape(3, new EllipseViewModel
            {
                Point = new Point2D(400, 400),
                Width = 80,
                Height = 80
            });

            geometryEditorViewModel.AddLine(new Point2D(300, 400), new Point2D(500, 400), 2);
            geometryEditorViewModel.AddLine(new Point2D(400, 300), new Point2D(400, 500), 2);
            geometryEditorViewModel.AddLine(new Point2D(330, 330), new Point2D(470, 470), 2);
            geometryEditorViewModel.AddLine(new Point2D(330, 470), new Point2D(470, 330), 2);
        }
    }
}
