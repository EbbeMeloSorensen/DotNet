using System.Linq;
using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Graph;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab5
{
    public class Tab5ViewModel : ViewModelBase
    {
        private bool _allowMovingVertices;
        //private int _selectedVertexCanvasPositionLeft;
        //private int _selectedVertexCanvasPositionTop;
        private PointD _selectedVertexCanvasPosition;

        private readonly Brush _northAmericaBrush = new SolidColorBrush(Colors.Yellow);
        private readonly Brush _southAmericaBrush = new SolidColorBrush(Colors.DarkOrange);
        private readonly Brush _europeBrush = new SolidColorBrush(Colors.CornflowerBlue);
        private readonly Brush _africaBrush = new SolidColorBrush(Colors.Red);
        private readonly Brush _asiaBrush = new SolidColorBrush(Colors.MediumSeaGreen);
        private readonly Brush _oceaniaBrush = new SolidColorBrush(Colors.MediumPurple);

        public bool AllowMovingVertices
        {
            get => _allowMovingVertices;
            set
            {
                _allowMovingVertices = value;
                GraphViewModel.AllowMovingVertices = _allowMovingVertices;
                RaisePropertyChanged();
            }
        }

        public PointD SelectedVertexCanvasPosition
        {
            get => _selectedVertexCanvasPosition;
            set
            {
                _selectedVertexCanvasPosition = value;
                RaisePropertyChanged();
            }
        }

        public GraphViewModel GraphViewModel { get; }

        public Tab5ViewModel()
        {
            var graph = GraphGenerator.GenerateGraphOfRiskTerritories();

            GraphViewModel = new GraphViewModel(graph, 1200, 900){ AllowMovingVertices = false };
            AllowMovingVertices = GraphViewModel.AllowMovingVertices;

            StyleGraph();

            GraphViewModel.VertexClicked += (s, e) =>
            {
                var point = GraphViewModel.PointViewModels[e.ElementId].Point;
                SelectedVertexCanvasPosition = new PointD(point.X - 16, point.Y - 16);
            };
        }

        private void StyleGraph()
        {
            // North America
            GraphViewModel.PlacePoint(0, new Utils.PointD(50, 50 + 50));
            GraphViewModel.PlacePoint(1, new Utils.PointD(150, 50 + 50));
            GraphViewModel.PlacePoint(2, new Utils.PointD(300, 50 + 50));
            GraphViewModel.PlacePoint(3, new Utils.PointD(100, 100 + 50));
            GraphViewModel.PlacePoint(4, new Utils.PointD(200, 100 + 50));
            GraphViewModel.PlacePoint(5, new Utils.PointD(300, 100 + 50));
            GraphViewModel.PlacePoint(6, new Utils.PointD(100, 150 + 50));
            GraphViewModel.PlacePoint(7, new Utils.PointD(250, 150 + 50));
            GraphViewModel.PlacePoint(8, new Utils.PointD(175, 200 + 50));

            // South America
            GraphViewModel.PlacePoint(9, new Utils.PointD(175, 300 + 50));
            GraphViewModel.PlacePoint(10, new Utils.PointD(125, 350 + 50));
            GraphViewModel.PlacePoint(11, new Utils.PointD(175, 400 + 50));
            GraphViewModel.PlacePoint(12, new Utils.PointD(225, 350 + 50));

            // Europe
            GraphViewModel.PlacePoint(13, new Utils.PointD(400, 50 + 50));
            GraphViewModel.PlacePoint(14, new Utils.PointD(500, 50 + 50));
            GraphViewModel.PlacePoint(15, new Utils.PointD(450, 100 + 50));
            GraphViewModel.PlacePoint(16, new Utils.PointD(500, 150 + 50));
            GraphViewModel.PlacePoint(17, new Utils.PointD(550, 100 + 50));
            GraphViewModel.PlacePoint(18, new Utils.PointD(450, 200 + 50));
            GraphViewModel.PlacePoint(19, new Utils.PointD(550, 200 + 50));

            // Africa
            GraphViewModel.PlacePoint(20, new Utils.PointD(450, 300 + 50));
            GraphViewModel.PlacePoint(21, new Utils.PointD(550, 300 + 50));
            GraphViewModel.PlacePoint(22, new Utils.PointD(600, 350 + 50));
            GraphViewModel.PlacePoint(23, new Utils.PointD(500, 350 + 50));
            GraphViewModel.PlacePoint(24, new Utils.PointD(500, 400 + 50));
            GraphViewModel.PlacePoint(25, new Utils.PointD(600, 400 + 50));

            // Asia
            GraphViewModel.PlacePoint(26, new Utils.PointD(900, 100 + 50));
            GraphViewModel.PlacePoint(27, new Utils.PointD(800, 100 + 50));
            GraphViewModel.PlacePoint(28, new Utils.PointD(1000, 100 + 50));
            GraphViewModel.PlacePoint(29, new Utils.PointD(1000, 150 + 50));
            GraphViewModel.PlacePoint(30, new Utils.PointD(900, 150 + 50));
            GraphViewModel.PlacePoint(31, new Utils.PointD(725, 175 + 50));
            GraphViewModel.PlacePoint(32, new Utils.PointD(900, 200 + 50));
            GraphViewModel.PlacePoint(33, new Utils.PointD(1000, 200 + 50));
            GraphViewModel.PlacePoint(34, new Utils.PointD(825, 175 + 50));
            GraphViewModel.PlacePoint(35, new Utils.PointD(675, 250 + 50));
            GraphViewModel.PlacePoint(36, new Utils.PointD(775, 250 + 50));
            GraphViewModel.PlacePoint(37, new Utils.PointD(850, 250 + 50));

            // Oceania
            GraphViewModel.PlacePoint(38, new Utils.PointD(850, 350 + 50));
            GraphViewModel.PlacePoint(39, new Utils.PointD(950, 350 + 50));
            GraphViewModel.PlacePoint(40, new Utils.PointD(850, 400 + 50));
            GraphViewModel.PlacePoint(41, new Utils.PointD(950, 400 + 50));

            Enumerable
                .Range(0, 9)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _northAmericaBrush));

            Enumerable
                .Range(9, 4)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _southAmericaBrush));

            Enumerable
                .Range(13, 7)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _europeBrush));

            Enumerable
                .Range(20, 6)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _africaBrush));

            Enumerable
                .Range(26, 12)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _asiaBrush));

            Enumerable
                .Range(38, 4)
                .ToList()
                .ForEach(index => GraphViewModel.AssignBrushToPoint(index, _oceaniaBrush));
        }
    }
}
