using Craft.ViewModels.Geometry2D.ScrollFree;
using DMI.Data.Studio.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DMI.Data.Studio.UI.WPF;

public class CustomizedLabelTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(
        object item,
        DependencyObject container)
    {
        var element = container as FrameworkElement;

        //return element.FindResource("TransactionTimeIntervalLabel") as DataTemplate;

        return item switch
        {
            PositionDifferenceLabel => element.FindResource("PositionDifferenceLabel") as DataTemplate,
            TransactionTimeIntervalLabel => element.FindResource("TransactionTimeIntervalLabel") as DataTemplate,
            LabelViewModel => element.FindResource("Label") as DataTemplate,
            _ => throw new ArgumentException("item doesn't correspond to any DataTemplate")
        };

    }
}