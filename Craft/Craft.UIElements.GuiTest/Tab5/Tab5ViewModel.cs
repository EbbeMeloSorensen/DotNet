using Craft.ViewModels.Graph;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab5
{
    public class Tab5ViewModel : ViewModelBase
    {
        public GraphViewModel GraphViewModel { get; set; }

        public Tab5ViewModel()
        {
            GraphViewModel = new GraphViewModel(1200, 900);

            GraphViewModel.PlacePoint(0, new Utils.PointD(50, 50));
            GraphViewModel.PlacePoint(1, new Utils.PointD(150, 50));
            GraphViewModel.PlacePoint(2, new Utils.PointD(300, 50));
            GraphViewModel.PlacePoint(3, new Utils.PointD(100, 100));
            GraphViewModel.PlacePoint(4, new Utils.PointD(200, 100));
            GraphViewModel.PlacePoint(5, new Utils.PointD(300, 100));
            GraphViewModel.PlacePoint(6, new Utils.PointD(100, 150));
            GraphViewModel.PlacePoint(7, new Utils.PointD(250, 150));
            GraphViewModel.PlacePoint(8, new Utils.PointD(175, 200));
            GraphViewModel.PlacePoint(9, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(10, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(11, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(12, new Utils.PointD(200, 500));
        }
    }
}
