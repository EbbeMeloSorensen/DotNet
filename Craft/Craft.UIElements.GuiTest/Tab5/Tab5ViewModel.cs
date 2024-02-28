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

            GraphViewModel.PlacePoint(13, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(14, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(15, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(16, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(17, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(18, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(19, new Utils.PointD(200, 500));
            GraphViewModel.PlacePoint(20, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(21, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(22, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(23, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(24, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(25, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(26, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(27, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(28, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(29, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(30, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(31, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(32, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(33, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(34, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(35, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(36, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(37, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(38, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(39, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(40, new Utils.PointD(300, 500));
            GraphViewModel.PlacePoint(41, new Utils.PointD(300, 500));
        }
    }
}
