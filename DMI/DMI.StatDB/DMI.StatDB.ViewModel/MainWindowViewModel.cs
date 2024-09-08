using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D.ScrollFree;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Application;

namespace DMI.StatDB.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUIDataProvider _dataProvider;
        private readonly IDialogService _applicationDialogService;
        private string _mainWindowTitle;
        private Brush _pointBrush = new SolidColorBrush(Colors.DarkRed);

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public StationListViewModel StationListViewModel { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; private set; }

        public MainWindowViewModel(
            IUIDataProvider dataProvider,
            IDialogService applicationDialogService)
        {
            _dataProvider = dataProvider;
            _applicationDialogService = applicationDialogService;

            _mainWindowTitle = "StatDB Studio";

            StationListViewModel = new StationListViewModel(
                dataProvider,
                applicationDialogService);

            StationListViewModel.SelectedStations.PropertyChanged += SelectedStations_PropertyChanged;

            GeometryEditorViewModel = new GeometryEditorViewModel(-1);
            GeometryEditorViewModel.InitializeWorldWindow(new Size(120, 120), new Point(11, 55));

            var lineThickness = 0.02;
            var brush = new SolidColorBrush(Colors.Black);

            var fyn_p1 = new PointD(9.8, 55.36);
            var fyn_p2 = new PointD(10.31, 55.62);
            var fyn_p3 = new PointD(10.83, 55.23);
            var fyn_p4 = new PointD(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness, brush);

            var jylland_p1 = new PointD(8.65, 54.92);
            var jylland_p2 = new PointD(8.08, 55.57);
            var jylland_p3 = new PointD(8.13, 56.56);
            var jylland_p4 = new PointD(8.61, 57.12);
            var jylland_p5 = new PointD(9.61, 57.27);
            var jylland_p6 = new PointD(10.62, 57.74);
            var jylland_p7 = new PointD(10.43, 56.52);
            var jylland_p8 = new PointD(10.89, 56.49);
            var jylland_p9 = new PointD(10.76, 56.17);
            var jylland_p10 = new PointD(10.33, 56.26);
            var jylland_p11 = new PointD(10.2, 55.83);
            var jylland_p12 = new PointD(9.6, 55.42);
            var jylland_p13 = new PointD(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness, brush);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness, brush);

            var sjalland_p1 = new PointD(10.98, 55.74);
            var sjalland_p2 = new PointD(12.26, 56.14);
            var sjalland_p3 = new PointD(12.67, 55.6);
            var sjalland_p4 = new PointD(12.05, 54.98);
            var sjalland_p5 = new PointD(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness, brush);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness, brush);
        }

        private void SelectedStations_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            var stations = sender as ObjectCollection<Station>;

            GeometryEditorViewModel.ClearPoints();

            // Identify earliest time
            var startTimes = new List<DateTime>();

            foreach (var station in stations.Objects)
            {
                foreach (var position in station.Positions)
                {
                    startTimes.Add(position.StartTime);
                }
            }

            // Initialize the time interval bars
            var positionCount = 0;
            foreach (var station in stations.Objects)
            {
                foreach (var position in station.Positions)
                {
                    if (position.Long.HasValue &&
                        position.Lat.HasValue)
                    {
                        var point = new PointD(
                            position.Long.Value,
                            position.Lat.Value);

                        GeometryEditorViewModel.AddPoint(point, 20, _pointBrush);
                    }

                    positionCount++;
                }
            }
        }
    }
}
