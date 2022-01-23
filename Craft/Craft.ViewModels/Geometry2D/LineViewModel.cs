using Craft.Math;

namespace Craft.ViewModels.Geometry2D
{
    public class LineViewModel
    {
        public Point2D Point1 { get; }
        public Point2D Point2 { get; }
        public double Thickness { get; }

        public LineViewModel(
            Point2D point1, 
            Point2D point2,
            double thickness)
        {
            Point1 = point1;
            Point2 = point2;
            Thickness = thickness;
        }

        public LineViewModel(
            LineSegment2D line,
            double thickness)
        {
            Point1 = line.Point1;
            Point2 = line.Point2;
            Thickness = thickness;
        }
    }
}
