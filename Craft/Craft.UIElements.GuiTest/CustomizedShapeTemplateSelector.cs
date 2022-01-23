using System;
using System.Windows;
using System.Windows.Controls;
using Craft.ViewModels.Geometry2D;

namespace Craft.UIElements.GuiTest
{
    // This DataTemplateSelector may be used for overriding the one in the Craft.ViewModels assembly
    public class CustomizedShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (item is RectangleViewModel)
            {
                return element.FindResource("DoorAndWindow") as DataTemplate;
            }

            if (item is EllipseViewModel)
            {
                return element.FindResource("Sun") as DataTemplate;
            }

            throw new ArgumentException("item doesn't correspond to any DataTemplate");
        }
    }
}
