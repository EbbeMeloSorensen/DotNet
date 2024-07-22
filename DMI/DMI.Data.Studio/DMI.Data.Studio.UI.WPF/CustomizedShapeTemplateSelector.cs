using Craft.ViewModels.Geometry2D.ScrollFree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                GreenBar => element.FindResource("GreenBar") as DataTemplate,
                YellowBar => element.FindResource("YellowBar") as DataTemplate,
                _ => throw new ArgumentException("item doesn't correspond to any DataTemplate")
            };
        }
    }
}
