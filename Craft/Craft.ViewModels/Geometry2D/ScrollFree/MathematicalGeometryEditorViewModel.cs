using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class MathematicalGeometryEditorViewModel : GeometryEditorViewModel
    {
        private Point _worldWindowLowerLeft;

        public override Size ViewPortSize
        {
            get { return _viewPortSize; }
            set
            {
                _viewPortSize = value;
                UpdateWorldWindowSize();
                _worldWindowUpperLeft = new Point(_worldWindowLowerLeft.X, -_worldWindowLowerLeft.Y - _worldWindowSize.Height);
                UpdateTransformationMatrix();
                RaisePropertyChanged();
            }
        }

        public MathematicalGeometryEditorViewModel(
            double initialMagnificationX = 1,
            double initialMagnificationY = 1,
            double initialWorldWindowUpperLeftX = 0,
            double initialWorldWindowUpperLeftY = 0) : base(initialMagnificationX,
                                                        initialMagnificationY,
                                                        initialWorldWindowUpperLeftX, 
                                                        initialWorldWindowUpperLeftY)
        {
            _worldWindowLowerLeft = new Point(
                initialWorldWindowUpperLeftX, 
                initialWorldWindowUpperLeftY);
        }

        public override void AddPoint(
            PointD point,
            double diameter,
            Brush brush = null)
        {
            base.AddPoint(new PointD(point.X, -point.Y), diameter, brush);
        }

        public override void AddShape(
            int id,
            ShapeViewModel shapeViewModel)
        {
            shapeViewModel.Point = new PointD(shapeViewModel.Point.X, -shapeViewModel.Point.Y);
            base.AddShape(id, shapeViewModel);
        }

        public override void AddLine(
            PointD point1,
            PointD point2,
            double thickness,
            Brush brush)
        {
            LineViewModels.Add(new LineViewModel(
                new PointD(point1.X, -point1.Y),
                new PointD(point2.X, -point2.Y),
                thickness,
                brush));
        }

        public override void AddPolygon(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolygonViewModels.Add(
                new PolygonViewModel(points.Select(p => new PointD(p.X, -p.Y)), thickness, brush));
        }
    }
}