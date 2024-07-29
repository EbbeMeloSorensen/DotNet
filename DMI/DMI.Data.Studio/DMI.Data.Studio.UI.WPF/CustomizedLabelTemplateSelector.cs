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

        return element.FindResource("TransactionTimeIntervalLabel") as DataTemplate;
    }
}