using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace DD.ViewModel
{
    public class PixelViewModelHex : PixelViewModel
    {
        public static PointCollection Points { get; }

        static PixelViewModelHex()
        {
            Points = new PointCollection
            {
                new (0,10.39230485),
                new (18,0),
                new (36,10.39230485),
                new (36,31.17691454),
                new (18,41.56921938),
                new (0,31.17691454)
            };
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
