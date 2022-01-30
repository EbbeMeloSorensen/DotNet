using System;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.ViewModels.Geometry2D;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application.Application _application;
        private readonly IDialogService _applicationDialogService;
        private string _mainWindowTitle;
        private Brush _pointBrush = new SolidColorBrush(Colors.DarkRed);
        private bool _classifyRecordsWithCondition;
        private readonly ObservableObject<bool> _observableForClassifyRecordsWithCondition;

        private RelayCommand<object> _createStationInformationCommand;
        private RelayCommand _deleteSelectedStationInformationsCommand;
        private RelayCommand _exportDataCommand;
        private RelayCommand<object> _extractFrieDataStationListCommand;
        private RelayCommand _importDataCommand;
        private RelayCommand<object> _openSettingsDialogCommand;

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                _mainWindowTitle = value;
                RaisePropertyChanged();
            }
        }

        public bool ClassifyRecordsWithCondition
        {
            get { return _classifyRecordsWithCondition; }
            set
            {
                _classifyRecordsWithCondition = value;
                RaisePropertyChanged();

                _observableForClassifyRecordsWithCondition.Object = value;
            }
        }

        public RelayCommand<object> CreateStationInformationCommand
        {
            get { return _createStationInformationCommand ?? (_createStationInformationCommand = new RelayCommand<object>(CreateStationInformation)); }
        }

        public RelayCommand DeleteSelectedStationInformationsCommand
        {
            get { return _deleteSelectedStationInformationsCommand ?? (_deleteSelectedStationInformationsCommand = new RelayCommand(DeleteSelectedStationInformations, CanDeleteSelectedStationInformations)); }
        }

        public RelayCommand ExportDataCommand
        {
            get { return _exportDataCommand ?? (_exportDataCommand = new RelayCommand(ExportData, CanExportData)); }
        }

        public RelayCommand<object> ExtractFrieDataStationListCommand
        {
            get { return _extractFrieDataStationListCommand ?? (_extractFrieDataStationListCommand = new RelayCommand<object>(ExtractFrieDataStationList)); }
        }

        public RelayCommand ImportDataCommand
        {
            get { return _importDataCommand ?? (_importDataCommand = new RelayCommand(ImportData, CanImportData)); }
        }

        public RelayCommand<object> OpenSettingsDialogCommand
        {
            get { return _openSettingsDialogCommand ?? (_openSettingsDialogCommand = new RelayCommand<object>(OpenSettingsDialog)); }
        }

        public StationInformationListViewModel StationInformationListViewModel { get; private set; }
        public StationInformationDetailsViewModel StationInformationDetailsViewModel { get; private set; }
        public StationInformationCollectionViewModel StationInformationCollectionViewModel { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; private set; }

        public MainWindowViewModel(
            Application.Application application,
            IDialogService applicationDialogService)
        {
            _application = application;
            _applicationDialogService = applicationDialogService;

            _mainWindowTitle = "SMS Studio";

            _observableForClassifyRecordsWithCondition = new ObservableObject<bool>();
            _observableForClassifyRecordsWithCondition.Object = true;

            StationInformationListViewModel = new StationInformationListViewModel(
                _application.UIDataProvider,
                applicationDialogService,
                _observableForClassifyRecordsWithCondition);

            StationInformationDetailsViewModel = new StationInformationDetailsViewModel(
                _application.UIDataProvider,
                applicationDialogService,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            StationInformationCollectionViewModel = new StationInformationCollectionViewModel(
                _application.UIDataProvider,
                StationInformationListViewModel.SelectedStationInformations,
                StationInformationListViewModel.RowCharacteristicsMap);

            _classifyRecordsWithCondition = true;

            StationInformationCollectionViewModel.ImageWidth = 1200;
            StationInformationCollectionViewModel.ImageHeight = 900;

            StationInformationListViewModel.SelectedStationInformations.PropertyChanged += SelectedStationInformations_PropertyChanged;

            GeometryEditorViewModel = new GeometryEditorViewModel();
            GeometryEditorViewModel.Magnification = 240;
            GeometryEditorViewModel.WorldWindowUpperLeft = new System.Windows.Point(7.4, 53.9);

            var lineThickness = 0.02;

            var fyn_p1 = new Point2D(9.8, 55.36);
            var fyn_p2 = new Point2D(10.31, 55.62);
            var fyn_p3 = new Point2D(10.83, 55.23);
            var fyn_p4 = new Point2D(10.3, 55.05);
            GeometryEditorViewModel.AddLine(fyn_p1, fyn_p2, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p2, fyn_p3, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p3, fyn_p4, lineThickness);
            GeometryEditorViewModel.AddLine(fyn_p4, fyn_p1, lineThickness);

            var jylland_p1 = new Point2D(8.65, 54.92);
            var jylland_p2 = new Point2D(8.08, 55.57);
            var jylland_p3 = new Point2D(8.13, 56.56);
            var jylland_p4 = new Point2D(8.61, 57.12);
            var jylland_p5 = new Point2D(9.61, 57.27);
            var jylland_p6 = new Point2D(10.62, 57.74);
            var jylland_p7 = new Point2D(10.43, 56.52);
            var jylland_p8 = new Point2D(10.89, 56.49);
            var jylland_p9 = new Point2D(10.76, 56.17);
            var jylland_p10 = new Point2D(10.33, 56.26);
            var jylland_p11 = new Point2D(10.2, 55.83);
            var jylland_p12 = new Point2D(9.6, 55.42);
            var jylland_p13 = new Point2D(9.46, 54.84);
            GeometryEditorViewModel.AddLine(jylland_p1, jylland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p2, jylland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p3, jylland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p4, jylland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p5, jylland_p6, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p6, jylland_p7, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p7, jylland_p8, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p8, jylland_p9, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p9, jylland_p10, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p10, jylland_p11, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p11, jylland_p12, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p12, jylland_p13, lineThickness);
            GeometryEditorViewModel.AddLine(jylland_p13, jylland_p1, lineThickness);

            var sjalland_p1 = new Point2D(10.98, 55.74);
            var sjalland_p2 = new Point2D(12.26, 56.14);
            var sjalland_p3 = new Point2D(12.67, 55.6);
            var sjalland_p4 = new Point2D(12.05, 54.98);
            var sjalland_p5 = new Point2D(11.22, 55.2);
            GeometryEditorViewModel.AddLine(sjalland_p1, sjalland_p2, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p2, sjalland_p3, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p3, sjalland_p4, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p4, sjalland_p5, lineThickness);
            GeometryEditorViewModel.AddLine(sjalland_p5, sjalland_p1, lineThickness);
        }

        private void CreateStationInformation(
            object owner)
        {
            var dialogViewModel = new CreateStationInformationDialogViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

            _application.UIDataProvider.CreateStationInformation(new StationInformation
            {
                StationName = dialogViewModel.StationName,
                GdbFromDate = currentTime,
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            }, true);
        }

        public void DeleteSelectedStationInformations()
        {
            throw new NotImplementedException();
            //_dataProvider.DeletePeople(PersonListViewModel.SelectedPeople.Objects.ToList());
        }

        private bool CanDeleteSelectedStationInformations()
        {
            return StationInformationListViewModel.SelectedStationInformations.Objects != null &&
                   StationInformationListViewModel.SelectedStationInformations.Objects.Any();
        }

        private void ExportData()
        {
            //var dialog = new SaveFileDialog
            //{
            //    Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            //};

            //if (dialog.ShowDialog() == false)
            //{
            //    return;
            //}

            //_dataProvider.ExportPeople(dialog.FileName);
            _application.UIDataProvider.ExportData(@"C:\Temp\SMSData.xml");
        }

        private void ExtractFrieDataStationList(
            object owner)
        {
            var dialogViewModel = new ExtractFrieDataStationListViewModel();

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            throw new NotImplementedException();
        }

        private bool CanExportData()
        {
            return true;
        }

        private void ImportData()
        {
            //var dialog = new OpenFileDialog
            //{
            //    Filter = "Xml Files(*.xml)|*.xml|Json Files(*.json)|*.json|All(*.*)|*"
            //};

            //if (dialog.ShowDialog() == false)
            //{
            //    return;
            //}

            _application.UIDataProvider.ImportData(@"C:\Temp\SMSData.xml");
        }

        private bool CanImportData()
        {
            return true;
        }

        private void OpenSettingsDialog(
            object owner)
        {
            var dialogViewModel = new SettingsDialogViewModel(
                _application.UIDataProvider);

            _applicationDialogService.ShowDialog(dialogViewModel, owner as Window);
        }

        private void SelectedStationInformations_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            var stationInformations = sender as ObjectCollection<StationInformation>;

            GeometryEditorViewModel.ClearPoints();

            if (stationInformations == null || !stationInformations.Objects.Any())
            {
                return;
            }

            foreach (var stationInformation in stationInformations.Objects)
            {
                if (!stationInformation.Wgs_lat.HasValue ||
                    !stationInformation.Wgs_long.HasValue)
                {
                    continue;
                }

                var point = new Point2D(
                    stationInformation.Wgs_long.Value,
                    stationInformation.Wgs_lat.Value);

                GeometryEditorViewModel.AddPoint(point, 20, _pointBrush);
            }
        }
    }
}
