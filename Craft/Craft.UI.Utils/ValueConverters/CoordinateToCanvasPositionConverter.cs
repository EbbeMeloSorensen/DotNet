using System;
using System.Globalization;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    public class CoordinateToCanvasPositionConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            var coordinate = (double)values[0];
            var diameter = (double)values[1];
            var upperLeft = (double)values[2];
            var scale = (double)values[3];

            return (coordinate - upperLeft) * scale - diameter / 2;
        }

        public object[] ConvertBack(
            object value, 
            Type[] targetTypes, 
            object parameter, 
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
