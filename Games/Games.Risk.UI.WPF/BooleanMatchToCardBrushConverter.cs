using System.Globalization;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Games.Risk.UI.WPF
{
    public class BooleanMatchToCardBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not bool a)
            {
                throw new ArgumentException("value should be of type bool");
            }

            if (parameter is not bool b)
            {
                throw new ArgumentException("parameter should be of type bool");
            }

            return a == b
                ? new SolidColorBrush(Colors.Cornsilk)
                : new SolidColorBrush(Colors.DarkSeaGreen);
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
