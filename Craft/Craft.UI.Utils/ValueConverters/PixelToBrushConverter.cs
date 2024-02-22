using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

            //var brush1 = new ImageBrush 
            //{
            //    ImageSource = new BitmapImage(new Uri(@"C:\Git\GitHub\DotNet\DD\DD.UI.WPF\Images\Water.PNG")),
            //    TileMode = TileMode.Tile
            //};

            var brush2 = new SolidColorBrush(Color.FromArgb(255, pixel.Red, pixel.Green, pixel.Blue));

            return brush2;
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