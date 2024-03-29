﻿using System;
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

            if (string.IsNullOrEmpty(pixel.ImagePath))
            {
                return new SolidColorBrush(Color.FromArgb(255, pixel.Red, pixel.Green, pixel.Blue));
            }

            // Bemærk lige her, hvordan vi her returnerer en ImageBrush, som benytter et billede,
            // der indgår som en ressource i WPF-applikationen

            // Eksempel:
            //var imagePath = "Images/Wall.jpg";

            return new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri($"pack://application:,,,/DD.UI.WPF;component/{pixel.ImagePath}")),
                TileMode = TileMode.Tile
            };
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