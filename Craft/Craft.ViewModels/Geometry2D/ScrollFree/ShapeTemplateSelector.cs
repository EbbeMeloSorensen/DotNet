using System;
using System.Windows;
using System.Windows.Controls;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            switch (item)
            {
                case RectangleViewModel rectangleViewModel:
                    {
                        return element.FindResource("Rectangle") as DataTemplate;
                    }
                case EllipseViewModel ellipseViewModel:
                    {
                        return element.FindResource("Ellipse") as DataTemplate;
                    }
                default:
                    {
                        throw new ArgumentException("item doesn't correspond to any DataTemplate");
                    }
            }
        }
    }
}
