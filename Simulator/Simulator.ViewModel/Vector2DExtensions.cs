using Craft.Math;
using Craft.Utils;

namespace Simulator.ViewModel
{
    public static class Vector2DExtensions
    {
        public static PointD AsPointD(
            this Vector2D vector)
        {
            return new PointD(vector.X, vector.Y);
        }
    }
}
