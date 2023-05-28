using System.Collections.Generic;
using System.Linq;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class PolygonViewModel
    {
        public string Points { get; set; }

        public PolygonViewModel(
            IEnumerable<PointD> points)
        {
            Points = string.Join(" ", points.Select(point => $"{point.X},{point.Y}"));
        }
    }
}
