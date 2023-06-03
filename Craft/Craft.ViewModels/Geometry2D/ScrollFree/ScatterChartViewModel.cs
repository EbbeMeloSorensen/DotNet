using Craft.Utils;
using System.Windows;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class ScatterChartViewModel : MathematicalGeometryEditorViewModel
    {
        public ScatterChartViewModel(
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
    }
}
