using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.UI.Utils.ValueConverters
{
    public class PixelToBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (!(value is Pixel))
                return Brushes.Transparent;

            var pixel = (Pixel) value;
            return new SolidColorBrush(Color.FromArgb(255, pixel.Red, pixel.Green, pixel.Blue));
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