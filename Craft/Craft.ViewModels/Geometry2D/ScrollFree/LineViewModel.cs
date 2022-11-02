using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class LineViewModel
    {
        public PointD Point1 { get; }
        public PointD Point2 { get; }
        public double Thickness { get; }

        public LineViewModel(
            PointD point1, 
            PointD point2,
            double thickness)
        {
            Point1 = point1;
            Point2 = point2;
            Thickness = thickness;
        }
    }
}
