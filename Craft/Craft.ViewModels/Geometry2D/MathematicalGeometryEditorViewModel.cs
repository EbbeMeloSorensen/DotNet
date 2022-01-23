using System;
using System.Windows;
using System.Windows.Media;
using Craft.Math;

namespace Craft.ViewModels.Geometry2D
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
            double initialMagnification = 1,
            double initialWorldWindowUpperLeftX = 0,
            double initialWorldWindowUpperLeftY = 0) : base(initialMagnification, 
                                                        initialWorldWindowUpperLeftX, 
                                                        initialWorldWindowUpperLeftY)
        {
            _worldWindowLowerLeft = new Point(
                initialWorldWindowUpperLeftX, 
                initialWorldWindowUpperLeftY);
        }

        public override void AddPoint(
            Point2D point,
            double diameter,
            Brush brush = null)
        {
            base.AddPoint(new Point2D(point.X, -point.Y), diameter, brush);
        }

        public override void AddShape(
            int id,
            ShapeViewModel shapeViewModel)
        {
            shapeViewModel.Point = new Point2D(shapeViewModel.Point.X, -shapeViewModel.Point.Y);
            base.AddShape(id, shapeViewModel);
        }

        public override void AddLine(
            Point2D point1,
            Point2D point2,
            double thickness)
        {
            LineViewModels.Add(new LineViewModel(
                new Point2D(point1.X, -point1.Y),
                new Point2D(point2.X, -point2.Y),
                thickness));
        }
    }
}