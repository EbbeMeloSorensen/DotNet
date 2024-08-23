using System;
using System.Windows;
using System.Windows.Controls;
using DMI.Data.Studio.ViewModel;

namespace DMI.Data.Studio.UI.WPF
{
    public class CustomizedShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            return item switch
            {
                LightGreenBar => element.FindResource("LightGreenBar") as DataTemplate,
                LightRedBar => element.FindResource("LightRedBar") as DataTemplate,
                GrayBar => element.FindResource("GrayBar") as DataTemplate,
                RedBar => element.FindResource("RedBar") as DataTemplate,
                OrangeBar => element.FindResource("OrangeBar") as DataTemplate,
                GreenBar => element.FindResource("GreenBar") as DataTemplate,
                YellowBar => element.FindResource("YellowBar") as DataTemplate,
                _ => throw new ArgumentException("item doesn't correspond to any DataTemplate")
            };
        }
    }
}
