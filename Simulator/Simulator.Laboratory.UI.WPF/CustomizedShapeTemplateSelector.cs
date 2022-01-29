using System;
using System.Windows;
using System.Windows.Controls;
using Simulator.Laboratory.ViewModel;
using Craft.ViewModels.Geometry2D;

namespace Simulator.Laboratory.UI.WPF
{
    public class CustomizedShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            switch (item)
            {
                case TaggedEllipseViewModel taggedEllipseViewModel:
                    {
                        return element.FindResource("TaggedEllipse") as DataTemplate;
                    }
                case RotatableEllipseViewModel rotatableEllipseViewModel:
                    {
                        return element.FindResource("RotatableEllipse") as DataTemplate;
                    }
                case EllipseViewModel ellipseViewModel:
                    {
                        return element.FindResource("Ellipse") as DataTemplate;
                    }
                case RectangleViewModel rectangleViewModel:
                    {
                        return element.FindResource("Rectangle") as DataTemplate;
                    }
                default:
                {
                    throw new ArgumentException("item doesn't correspond to any DataTemplate");
                }
            }
        }
    }
}
