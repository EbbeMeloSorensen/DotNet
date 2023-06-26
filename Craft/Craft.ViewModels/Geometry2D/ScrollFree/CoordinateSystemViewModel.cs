using System.Windows;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class CoordinateSystemViewModel : GeometryEditorViewModel
    {
        public CoordinateSystemViewModel(
            Point worldWindowFocus,
            Size worldWindowSize) : base(-1, worldWindowFocus, worldWindowSize)
        {
        }
    }
}
