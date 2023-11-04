using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Simulator.Application;

namespace Game.Rocket.UI.WPF.ValueConverters
{
    public class ApplicationStateToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (!(value is Craft.DataStructures.State))
            {
                return Visibility.Hidden;
            }

            var valueAsApplicationState = (Craft.DataStructures.State)value;
            var parameterAsString = parameter as string;

            return valueAsApplicationState != null && valueAsApplicationState.Name == parameterAsString
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
