using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    // En variant af GeometryEditorViewModel, hvor figurer spejles i x-aksen, før de sættes ind, således at det fremstår korrekt
    // i henhold til et matematisk 2-dimensionalt koordinatsystem, hvor y-aksen peger opad
    public class MathematicalGeometryEditorViewModel : GeometryEditorViewModel
    {

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

        public override void AddPolyline(
            IEnumerable<PointD> points,
            double thickness,
            Brush brush)
        {
            PolylineViewModels.Add(
                new PolylineViewModel(points.Select(p => new PointD(p.X, -p.Y)), thickness, brush));
        }

        public MathematicalGeometryEditorViewModel(
            int yAxisFactor, 
            double scalingX, 
            double scalingY) : base(yAxisFactor, scalingX, scalingY)
        {
        }

        public MathematicalGeometryEditorViewModel(
            int yAxisFactor, 
            Point worldWindowUpperLeft, 
            Size worldWindowSize) : base(yAxisFactor, worldWindowUpperLeft, worldWindowSize)
        {
        }

        protected MathematicalGeometryEditorViewModel(int yAxisFactor) : base(yAxisFactor)
        {
        }
    }
}