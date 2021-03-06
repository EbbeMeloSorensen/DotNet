using System;
using System.Globalization;
using System.Windows.Data;

namespace Craft.UI.Utils.ValueConverters
{
    public class CoordinateToMagnifiableCanvasPositionConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            try
            {
                var coordinate = (double)values[0];
                var diameter = (double)values[1];
                var magnification = (double)values[2];

                return (coordinate - diameter / 2) * magnification;
            }
            catch (Exception)
            {
                // Dette er en lidt hacky måde at løse et problem på med at denne converter kan blive kaldt uden at Magnification er blevet sat.
                // Magnification ejes vel at mærke af parenten
                //throw e;
                return 0;
            }
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
