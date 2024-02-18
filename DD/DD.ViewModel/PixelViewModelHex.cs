using System;
using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace DD.ViewModel
{
    // A specialization of the PixelViewModel that also has a point collection for drawing polygons
    public class PixelViewModelHex : PixelViewModel
    {
        public static PointCollection Points { get; private set; }

        public static double ShiftX { get; private set; }
        public static double ShiftY { get; private set; }

        public static void InitializePoints(
            double tileCenterSpacing)
        {
            Points = new PointCollection
            {
                new (0, tileCenterSpacing * Math.Sqrt(3) / 6),
                new (tileCenterSpacing / 2,0),
                new (tileCenterSpacing,tileCenterSpacing * Math.Sqrt(3) / 6),
                new (tileCenterSpacing,tileCenterSpacing * Math.Sqrt(3) / 2),
                new (tileCenterSpacing / 2,tileCenterSpacing * Math.Sqrt(3) * 2 / 3),
                new (0,tileCenterSpacing * Math.Sqrt(3) / 2)
            };

            ShiftX = tileCenterSpacing / 2;
            ShiftY = tileCenterSpacing * Math.Sqrt(3) / 2;
        }

        public PixelViewModelHex(
            int id, 
            Pixel pixel) : base(
                id, 
                pixel)
        {
        }
    }
}
